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
        public static Gabbro Instance { get; private set; } // it would cause bug!!!

        Transform _flute;
        TweenerCore<Vector3, Vector3, VectorOptions> _sittingTween;

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
                if(_sittingTween != null) {
                    _sittingTween.Kill();
                    _sittingTween = null;
                }
                _animator.transform.localPosition = Vector3.zero;
                if(AudioSignal) {
                    AudioSignal.transform.localPosition = new Vector3(0, 1.4859f, 0.6699f);
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

            var conversationZone = GetComponentInChildren<InteractReceiver>(true);
            conversationZone.gameObject.SetActive(false);
            Observable.NextFrame().Subscribe(_ => {
                conversationZone.gameObject.SetActive(true);

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
            base.Sitting();
            _flute.localPosition = new Vector3(0.2564f, 0.0652f, 0.0344f);
            _flute.localEulerAngles = new Vector3(-217.918f, -83.78799f, 90.05f);
            _flute.localScale = Vector3.one;

            AudioSignal.transform.localPosition = new Vector3(0, 0.7121f, 0.2321f);

            SittingTween(null, 1);
        }

        public override void StartPlaying() {
            base.StartPlaying();
            SittingTween();
        }

        public override void StopPlaying() {
            base.StopPlaying();
            SittingTween();
        }

        public void SittingTween(bool? playing = null, float duration = 0.1f) {
            if(!IsSitting) {
                return;
            }
            if(_sittingTween != null) {
                _sittingTween.Kill();
                _sittingTween = null;
            }

            if(!playing.HasValue) {
                playing = _playing;
            }

            if(playing.Value) {
                //_sittingTween = _animator.transform.DOLocalMoveY(-0.78f, 1).SetLink(_animator.gameObject);
                _sittingTween = _animator.transform.DOLocalMove(new Vector3(0, -0.78f, 0), duration).SetLink(_animator.gameObject);
            }
            else {
                _sittingTween = _animator.transform.DOLocalMove(new Vector3(0, 0, 0.53f), duration).SetLink(_animator.gameObject);
            }
        }
    }
}
