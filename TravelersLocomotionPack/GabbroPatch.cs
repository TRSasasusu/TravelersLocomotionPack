using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelersLocomotionPack {
    [HarmonyPatch]
    public static class GabbroPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.Update))]
        public static bool GabbroTravelerController_Update_Prefix() {
            if (Gabbro.IsGabbroStanding) {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.StartConversation))]
        public static bool GabbroTravelerController_StartConversation_Prefix(GabbroTravelerController __instance) {
            if (Gabbro.IsGabbroStanding) {
                if(__instance._animator.enabled) {
                    __instance._animator.SetTrigger("Talking");
                }
                Locator.GetTravelerAudioManager().StopAllTravelerAudio();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.EndConversation))]
        public static bool GabbroTravelerController_EndConversation_Prefix(GabbroTravelerController __instance, float audioDelay) {
            if (Gabbro.IsGabbroStanding) {
                if (__instance._animator.enabled) {
                    if(Gabbro.IsStopPlaying) {
                        __instance._animator.SetTrigger("Talking");
                    }
                    else {
                        __instance._animator.SetTrigger("Playing");
                    }
                }
                Locator.GetTravelerAudioManager().PlayAllTravelerAudio(audioDelay);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.OnUnpause))]
        public static bool GabbroTravelerController_OnUnpause_Prefix(GabbroTravelerController __instance) {
            if(Gabbro.IsGabbroStanding) {
                if(__instance._animator.enabled) {
                    if(Gabbro.IsStopPlaying) {
                        __instance._animator.SetTrigger("Talking");
                    }
                    else {
                        __instance._animator.SetTrigger("Playing");
                    }
                }
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.OnIslandSplashEvent))]
        public static bool GabbroTravelerController_OnIslandSplashEvent_Prefix() {
            if(Gabbro.IsGabbroStanding) {
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.OnIslandApexEvent))]
        public static bool GabbroTravelerController_OnIslandApexEvent_Prefix() {
            if(Gabbro.IsGabbroStanding) {
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GabbroTravelerController), nameof(GabbroTravelerController.OnSectorOccupantsUpdated))]
        public static bool GabbroTravelerController_OnSectorOccupantsUpdated_Prefix() {
            if(Gabbro.IsGabbroStanding) {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TravelerAudioManager), nameof(TravelerAudioManager.OnUnpause))]
        public static void TravelerAudioManager_OnUnpause_Postfix() {
            if(Gabbro.IsStopPlaying) {
                Gabbro.Instance.StopPlaying();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OWAudioSource), nameof(OWAudioSource.FadeIn))]
        public static bool OWAudioSource_FadeIn_Prefix(OWAudioSource __instance) {
            if(Gabbro.IsStopPlaying && Gabbro.Instance.AudioSignal != null && __instance == Gabbro.Instance.AudioSignal.GetOWAudioSource()) {
                return false;
            }
            return true;
        }
    }
}
