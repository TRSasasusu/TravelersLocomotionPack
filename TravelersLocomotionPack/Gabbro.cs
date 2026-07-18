using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UniRx;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace TravelersLocomotionPack {
    public class Gabbro : Traveler {
        public static bool IsStanding { get { return Instance != null && Instance._isStanding; } }
        public static bool IsStopPlaying { get { return Instance != null && !Instance._playing; } }
        public static Gabbro Instance { get; private set; }

        Transform _flute;
        //TweenerCore<Vector3, Vector3, VectorOptions> _sittingTween;
        CharacterDialogueTree _conversationZoneDialogueTree;

        public override void Initialize() {
            base.Initialize();
            Instance = this;

            _animator.runtimeAnimatorController = TravelersLocomotionPack.GabbroAnimatorController;
            _flute = transform.Find("Traveller_HEA_Gabbro_ANIM_IdleFlute_LP2/gabbro_OW_V02:gabbro_rig_v01:Trajectory_Jnt/gabbro_OW_V02:gabbro_rig_v01:ROOT_Jnt/gabbro_OW_V02:gabbro_rig_v01:Spine_01_Jnt/gabbro_OW_V02:gabbro_rig_v01:Spine_02_Jnt/gabbro_OW_V02:gabbro_rig_v01:Spine_Top_Jnt/gabbro_OW_V02:gabbro_rig_v01:RT_Arm_Clavicle_Jnt/gabbro_OW_V02:gabbro_rig_v01:RT_Arm_Shoulder_Jnt/gabbro_OW_V02:gabbro_rig_v01:RT_Arm_Elbow_Jnt/gabbro_OW_V02:gabbro_rig_v01:RT_Arm_Wrist_Jnt/Props_HEA_Flute");
        }

        public override void StandUp() {
            base.StandUp();
            if(IsStanding) {
                _animator.SetTrigger("StandUp");
                _animator.SetBool("Sitting", false);
                _animator.transform.localPosition = Vector3.zero;
                if(AudioSignal) {
                    AudioSignal.transform.localPosition = new Vector3(0, 1.4859f, 0.6699f);
                }
                if(_conversationZoneDialogueTree != null) {
                    _conversationZoneDialogueTree.transform.localPosition = new Vector3(0, 1.511f, 0);//new Vector3(-2.6768f, -3.5163f, 1.1796f);
                    _conversationZoneDialogueTree._attentionPoint.localPosition = new Vector3(0, 1.511f, 0);//new Vector3(-2.6768f, -3.5163f, 1.1796f);
                }

                _flute.localPosition = new Vector3(0.222f, 0.0504f, -0.1457f);
                _flute.localEulerAngles = new Vector3(40.745f, 122.8411f, 285.6188f);
                _flute.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    if(_playing) {
                        _animator.SetTrigger("Playing");
                    }
                    else {
                        _animator.SetTrigger("Talking");
                    }
                }).AddTo(this);
                return;
            }

            _conversationZoneDialogueTree = GetComponentInChildren<CharacterDialogueTree>(true);
            _conversationZoneDialogueTree.gameObject.SetActive(false);
            Observable.NextFrame().Subscribe(_ => {
                _conversationZoneDialogueTree.gameObject.SetActive(true);

                _travelerController._animator = _animator;

                _animator.SetTrigger("StandUp");
                _animator.SetBool("NoHammock", true);

                transform.DOLocalMove(new Vector3(0.5932f, 0.131f, 0), 1.5f);
                transform.DOLocalRotate(new Vector3(0, 91.6766f, 0), 1.5f);

                _isStanding = true;

                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    transform.parent = null;
                    _animator.SetTrigger("Playing");
                }).AddTo(this);

                _conversationZone = GetComponentInChildren<CharacterDialogueTree>(true).GetComponent<SphereCollider>();
                AudioSignal = GetComponentInChildren<AudioSignal>(true);
            }).AddTo(this);
        }

        public override void Sitting() {
            _animator.SetBool("Sitting", true);
            base.Sitting();
            _flute.localPosition = new Vector3(0.2564f, 0.0652f, 0.0344f);
            _flute.localEulerAngles = new Vector3(-217.918f, -83.78799f, 90.05f);
            _flute.localScale = Vector3.one;

            AudioSignal.transform.localPosition = new Vector3(-0.0264f, 0.7121f, -0.4652f);
            _conversationZoneDialogueTree.transform.localPosition = new Vector3(-0.0254f, 0.9379f, -1.1304f);
            _conversationZoneDialogueTree._attentionPoint.localPosition = new Vector3(-0.0254f, 0.9379f, -1.1304f);
        }

        public override void StartPlaying() {
            base.StartPlaying();
        }

        public override void StopPlaying() {
            base.StopPlaying();
        }
    }
}
