using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelersLocomotionPack {
    [HarmonyPatch]
    public static class TravelerPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.StartConversation))]
        public static bool TravelerController_StartConversation_Prefix(TravelerController __instance) {
            if (__instance._animator == Riebeck.Instance._animator) {
                if (Riebeck.IsStanding) {
                    if (__instance._animator.enabled) {
                        __instance._animator.SetTrigger("Talking");
                    }
                    Locator.GetTravelerAudioManager().StopAllTravelerAudio();
                    return false;
                }
            }
            // Feldspar
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.EndConversation))]
        public static bool TravelerController_EndConversation_Prefix(TravelerController __instance, float audioDelay) {
            if (__instance._animator == Riebeck.Instance._animator) {
                if (Riebeck.IsStanding) {
                    if (__instance._animator.enabled) {
                        if (Riebeck.IsStopPlaying) {
                            __instance._animator.SetTrigger("Talking");
                        }
                        else {
                            __instance._animator.SetTrigger("Playing");
                        }
                    }
                    Locator.GetTravelerAudioManager().PlayAllTravelerAudio(audioDelay);
                    return false;
                }
            }
            // Feldspar
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.OnUnpause))]
        public static bool TravelerController_OnUnpause_Prefix(TravelerController __instance) {
            if (Riebeck.Instance != null && __instance._animator == Riebeck.Instance._animator) {
                if (Riebeck.IsStanding) {
                    if (__instance._animator.enabled) {
                        if (Riebeck.IsStopPlaying) {
                            __instance._animator.SetTrigger("Talking");
                        }
                        else {
                            __instance._animator.SetTrigger("Playing");
                        }
                    }
                    return false;
                }
            }
            // Feldspar
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.OnEndFastForward))]
        public static bool TravelerController_OnEndFastForward_Prefix(TravelerController __instance) {
            if (__instance._animator == Gabbro.Instance._animator) {
                if(Gabbro.IsStanding) {
                    __instance._animator.enabled = true;
                    if (Gabbro.IsStopPlaying) {
                        __instance._animator.SetTrigger("Talking");
                    }
                    else {
                        __instance._animator.SetTrigger("Playing");
                    }
                    return false;
                }
            }
            else if (__instance._animator == Riebeck.Instance._animator) {
                if(Riebeck.IsStanding) {
                    __instance._animator.enabled = true;
                    if (Riebeck.IsStopPlaying) {
                        __instance._animator.SetTrigger("Talking");
                    }
                    else {
                        __instance._animator.SetTrigger("Playing");
                    }
                    return false;
                }
            }
            // Feldspar
            return true;
        }
    }
}
