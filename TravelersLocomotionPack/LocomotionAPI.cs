using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace TravelersLocomotionPack {
    public class LocomotionAPI : ILocomotion {
        Chert _chert;

        GameObject _originalGabbro;
        Gabbro _gabbro;

        public void RiebeckStandUp() {
            throw new NotImplementedException();
        }

        public void ChertInitialize(GameObject chert) {
            _chert = chert.AddComponent<Chert>();
            _chert.Initialize();
        }

        public void ChertSitDown() {
            _chert.SitDown();
        }

        public void ChertEndOnFloor() {
            _chert.EndOnFloor();
        }

        public void GabbroInitialize(GameObject gabbro) {
            _originalGabbro = gabbro;

            var gabbroLP = TravelersLocomotionPack.Instance.NewHorizons.SpawnObject(TravelersLocomotionPack.Instance, gabbro.transform.root.gameObject, null, gabbro.transform.GetPath(), gabbro.transform.localPosition, gabbro.transform.localEulerAngles, 1, false);
            gabbroLP.name = gabbro.name + "_LP2";
            gabbroLP.AddComponent<NewHorizons.Components.AddPhysics>(); // this requires around 0.1 sec.

            Coroutine coroutine = TravelersLocomotionPack.Instance.StartCoroutine(SetAlignment(gabbroLP, obj => {
                obj.transform.parent = gabbro.transform.parent;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;

                _gabbro = obj.AddComponent<Gabbro>();
                _gabbro.Initialize();
            }));
            gabbro.OnDestroyAsObservable().Subscribe(_ => {
                if(coroutine != null) {
                    TravelersLocomotionPack.Instance.StopCoroutine(coroutine);
                }
            });
        }

        public bool GabbroIsInitialized() {
            return _gabbro != null;
        }

        public GameObject GetGabbro() {
            return _gabbro.gameObject;
        }

        public void GabbroStandUp() {
            _originalGabbro.SetActive(false);
            _gabbro.gameObject.SetActive(true);
            _gabbro.StandUp();
        }

        public void GabbroMoveTo(Transform target, float radius, float speed, Vector3 offset) {
            _gabbro.MoveTo(target, radius, speed, offset);
        }

        IEnumerator SetAlignment(GameObject obj, Action<GameObject> callback) {
            var generatedParentName = obj.name + "_Body";
            while (true) {
                if(obj.transform.parent.name == generatedParentName) {
                    break;
                }
                yield return null;
            }

            obj = obj.transform.parent.gameObject;
            //GameObject.Destroy(obj.GetComponent<DynamicForceDetector>());
            var alignmentForceDetector = obj.AddComponent<AlignmentForceDetector>();
            var alignWithForce = obj.AddComponent<AlignWithForce>();
            alignWithForce._adjustedSlerpRate = 1;
            alignWithForce._interpolationRate = 2;
            alignWithForce._forceDetector = alignmentForceDetector;
            alignWithForce._allowAlignment = true;
            alignWithForce._doAlignment = true;

            obj.SetActive(false);
            callback(obj);
        }
    }
}
