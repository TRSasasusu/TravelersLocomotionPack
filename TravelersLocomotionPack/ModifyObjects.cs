using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace TravelersLocomotionPack {
    public class ModifyObjects {
        Coroutine _initializeBody;

        public void Initialize() {
            _initializeBody = TravelersLocomotionPack.Instance.StartCoroutine(InitializeBody());
        }

        public void DestroyResources() {
            if (_initializeBody != null) {
                TravelersLocomotionPack.Instance.StopCoroutine(_initializeBody);
                _initializeBody = null;
            }
        }

        IEnumerator InitializeBody() {
            GameObject gabbroLP = null;
            yield break; // TODO: caution!!!
            while(true) {
                gabbroLP = GameObject.Find("Traveller_HEA_Gabbro_ANIM_IdleFlute_LP");
                if(gabbroLP) {
                    break;
                }
                yield return null;
            }
            while(true) {
                if(gabbroLP.transform.parent.name == "Traveller_HEA_Gabbro_ANIM_IdleFlute_LP_Body") {
                    Gabbro._gabbroLPBody = gabbroLP.transform.parent.gameObject;
                    Gabbro._gabbroLPBody.SetActive(false);

                    var alignWithTargetBody = gabbroLP.transform.parent.gameObject.AddComponent<AlignWithTargetBody>();
                    //alignWithTargetBody._targetBody = giandtsdeep; // it should be tuned in each location
                    alignWithTargetBody._adjustedSlerpRate = 1;
                    alignWithTargetBody._interpolationRate = 2;
                    // AddVelocityChangeを使えばいけそう
                    break;
                }
                yield return null;
            }
        }
    }
}
