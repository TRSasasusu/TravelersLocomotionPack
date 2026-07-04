using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using NewHorizons.Utility;
using IEnumerator = System.Collections.IEnumerator;

namespace TravelersLocomotionPack {
    public class LocomotionAPI : ILocomotion {
        Chert _chert;

        GameObject _originalGabbro;
        Gabbro _gabbro;
        //GameObject _originalJetpack;

        public void RiebeckStandUp() {
            throw new NotImplementedException();
        }

        public void ChertInitialize(GameObject chert) {
            var originalChert = chert;
            var newChert = TravelersLocomotionPack.Instance.NewHorizons.SpawnObject(TravelersLocomotionPack.Instance, chert.transform.root.gameObject, null, chert.transform.GetPath(), chert.transform.localPosition, chert.transform.localEulerAngles, 1, false);
            chert.SetActive(false);

            _chert = newChert.AddComponent<Chert>();
            _chert.transform.parent = chert.transform.parent;
            _chert.transform.localPosition = chert.transform.localPosition;
            _chert.transform.localEulerAngles = chert.transform.localEulerAngles;

            FindAndSetAudioSignalAndConversationZone(_chert.gameObject, originalChert);

            _chert.Initialize();

            //GameObject.Destroy(originalChert);
        }

        public GameObject GetChert() {
            return _chert.gameObject;
        }

        public void ChertSitDown() {
            _chert.SitDown();
        }

        public void ChertEndOnFloor() {
            _chert.EndOnFloor();
        }

        public void ChertStopPlaying() {
            _chert.StopPlaying();
        }

        public void ChertStartPlaying() {
            _chert.StartPlaying();
        }

        public void GabbroInitialize(GameObject gabbro) {
            _originalGabbro = gabbro;

            var gabbroLP = TravelersLocomotionPack.Instance.NewHorizons.SpawnObject(TravelersLocomotionPack.Instance, gabbro.transform.root.gameObject, null, gabbro.transform.GetPath(), gabbro.transform.localPosition, gabbro.transform.localEulerAngles, 1, false);
            gabbroLP.name = gabbro.name + "_LP2";
            gabbroLP.AddComponent<NewHorizons.Components.AddPhysics>(); // this requires around 0.1 sec.

            //var jetpack = TravelersLocomotionPack.Instance.NewHorizons.SpawnObject(TravelersLocomotionPack.Instance, gabbro)

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

            //_originalJetpack = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Props_LowerVillage/OtherComponentsGroup/Architecture_LowerVillage/OtherComponentsGroup/Village_UnderLaunchTowerProps/LaunchTowerSequoiaProps/WorkBench2/PlayerSuit_Jetpack (1)");
        }

        public bool GabbroIsInitialized() {
            return _gabbro != null;
        }

        public GameObject GetGabbro() {
            return _gabbro.gameObject;
        }

        public void GabbroStandUp() {
            if(Gabbro.IsStanding) {
                _gabbro.StandUp(); // only animation
                return;
            }

            _originalGabbro.SetActive(false);
            _gabbro.gameObject.SetActive(true);

            FindAndSetAudioSignalAndConversationZone(_gabbro.gameObject, _originalGabbro, new Vector3(0, 1.4859f, 0.6699f), new Vector3(0, 1.511f, 0));

            _gabbro.StandUp();
        }

        public void GabbroMoveStop() {
            _gabbro.MoveStop();
        }

        public void GabbroMoveTo(Transform target, float radius, float speed, Vector3 offset) {
            _gabbro.MoveTo(target, radius, speed, offset);
        }

        public void GabbroLookAt(Transform target, Vector3 offset) {
            _gabbro.LookAt(target, offset);
        }

        public void GabbroStopPlaying() {
            _gabbro.StopPlaying();
        }

        public void GabbroStartPlaying() {
            _gabbro.StartPlaying();
        }

        public void GabbroSitting() {
            _gabbro.Sitting();
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

        void FindAndSetAudioSignalAndConversationZone(GameObject target, GameObject source, Vector3? audioSignalPos = null, Vector3? conversationZonePos = null) {
            var audiosignal = source.GetComponentInChildren<AudioSignal>(true);
            if (!audiosignal && source.transform.parent) {
                audiosignal = source.transform.parent.GetComponentInChildren<AudioSignal>(true);
                if (!audiosignal && source.transform.parent.parent) {
                    audiosignal = source.transform.parent.parent.GetComponentInChildren<AudioSignal>(true);
                }
            }
            if (audiosignal) {
                var existingAudiosignal = target.GetComponentInChildren<AudioSignal>(true);
                if(existingAudiosignal) {
                    GameObject.Destroy(existingAudiosignal.gameObject);
                }

                audiosignal.transform.parent = target.transform;
                if(audioSignalPos.HasValue) {
                    audiosignal.transform.localPosition = audioSignalPos.Value;
                    audiosignal.transform.localEulerAngles = Vector3.zero;
                }
            }

            var conversationZone = source.GetComponentInChildren<CharacterDialogueTree>(true);
            if (!conversationZone && source.transform.parent) {
                conversationZone = source.transform.parent.GetComponentInChildren<CharacterDialogueTree>(true);
            }
            if (conversationZone != null) {
                var existingConversationZone = target.GetComponentInChildren<CharacterDialogueTree>(true);
                if(existingConversationZone) {
                    GameObject.Destroy(existingConversationZone.gameObject); // because TravelerController controls the original conversation zone as _dialogueSystem
                }

                conversationZone.transform.parent = target.transform;
                if(conversationZonePos.HasValue) {
                    conversationZone.transform.localPosition = conversationZonePos.Value;
                    conversationZone.transform.localEulerAngles = Vector3.zero;
                }
                conversationZone.GetComponent<InteractReceiver>()._usableInShip = true;
                if (conversationZone._attentionPoint != null) {
                    conversationZone._attentionPoint.transform.parent = target.transform;
                    if(conversationZonePos.HasValue) {
                        conversationZone._attentionPoint.transform.localPosition = conversationZonePos.Value;
                        conversationZone._attentionPoint.transform.localEulerAngles = Vector3.zero;
                    }
                }
            }
        }
    }
}
