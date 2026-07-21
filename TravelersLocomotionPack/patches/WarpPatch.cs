using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelersLocomotionPack.patches {
    [HarmonyPatch]
    public static class WarpPatch {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlackHoleVolume), nameof(BlackHoleVolume.Vanish))]
        public static bool BlackHoleVolume_Vanish_Prefix(BlackHoleVolume __instance, OWRigidbody bodyToVanish, RelativeLocationData entryLocation) {
            if(__instance._whiteHole == null) {
                return true;
            }

            if(Gabbro.Instance != null && bodyToVanish.gameObject == Gabbro.Instance.gameObject) {
                __instance._whiteHole.ReceiveWarpedBody(bodyToVanish, entryLocation);
                return false;
            }
            if(Riebeck.Instance != null && bodyToVanish.gameObject == Riebeck.Instance.gameObject) {
                __instance._whiteHole.ReceiveWarpedBody(bodyToVanish, entryLocation);
                return false;
            }
            // Feldspar
            return true;
        }
    }
}
