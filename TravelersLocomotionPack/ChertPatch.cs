using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace TravelersLocomotionPack {
    [HarmonyPatch]
    public static class ChertPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChertTravelerController), nameof(ChertTravelerController.OnSectorOccupantsUpdated))]
        public static bool ChertTravelerController_OnSectorOccupantsUpdated_Prefix(ChertTravelerController __instance) {
            if (Chert.Instance != null) {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TravelerController), nameof(TravelerController.EndConversation))]
        public static bool TravelerController_EndConversation_Prefix(TravelerController __instance, float audioDelay) {
            //TravelersLocomotionPack.Log("called TravelerController_EndConversation_Prefix");
            if (Chert.Instance != null && __instance == Chert.Instance.TravelerController) {
                if (__instance._animator.enabled) {
                    if (Chert.IsStopPlaying) {
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
    }
}
