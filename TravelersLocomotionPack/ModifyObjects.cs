using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace TravelersLocomotionPack {
    public class ModifyObjects {
        public static ModifyObjects Instance { get; private set; }

        public GameObject Jetpack { get; private set; }
        public GameObject PlayerVFX { get; private set; }

        Coroutine _initializeBody;

        public void Initialize() {
            Instance = this;
            _initializeBody = TravelersLocomotionPack.Instance.StartCoroutine(InitializeBody());
        }

        public void DestroyResources() {
            Instance = null;
            if (_initializeBody != null) {
                TravelersLocomotionPack.Instance.StopCoroutine(_initializeBody);
                _initializeBody = null;
            }
        }

        IEnumerator InitializeBody() {
            while(true) {
                var ship = Locator.GetShipBody();
                if (ship != null) {
                    var jetpack = ship.transform.Find("Module_Supplies/Systems_Supplies/ExpeditionGear/EquipmentGeo/Props_HEA_PlayerSuit_Hanging/PlayerSuit_Jetpack");
                    if (jetpack != null) {
                        Jetpack = jetpack.gameObject;
                        break;
                    }
                }
                yield return null;
            }

            while(true) {
                var player = Locator.GetPlayerBody();
                if (player != null) {
                    var playerVFX = player.transform.Find("PlayerVFX");
                    if (playerVFX != null) {
                        PlayerVFX = playerVFX.gameObject;
                        break;
                    }
                }
                yield return null;
            }
        }
    }
}
