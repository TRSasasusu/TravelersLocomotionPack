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
                    __instance._animator.SetTrigger("Playing");
                }
                Locator.GetTravelerAudioManager().PlayAllTravelerAudio(audioDelay);
                return false;
            }
            return true;
        }
    }
}
