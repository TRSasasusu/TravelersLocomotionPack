using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public class Chert : MonoBehaviour {
        public AudioSignal AudioSignal { get; private set; }
        public static bool IsStopPlaying { get { return Instance != null && !Instance._playing; } }
        public static Chert Instance { get; private set; }

        public TravelerController TravelerController { get; private set; }

        Animator _animator;
        Transform _drum;
        bool _playing = true;

        public void Initialize() {
            Instance = this;

            _animator = GetComponentInChildren<Animator>();
            _animator.runtimeAnimatorController = TravelersLocomotionPack.ChertAnimatorController;
            _animator.SetBool("OnFloor", false);

            TravelerController = transform.parent.GetComponentInChildren<TravelerController>(true);
            TravelerController._animator = _animator;

            _drum = transform.Find("Traveller_HEA_Chert_ANIM_Chatter_Chipper/NewDrum:pCylinder1");
            AudioSignal = GetComponentInChildren<AudioSignal>(true);
        }

        public void SitDown() {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.forward * 0f + transform.up * 0.5f, -transform.up, out hit, 0.4f)) {
                TravelersLocomotionPack.Log($"hit to {hit.collider.name}");
                StartOnFloor();
            }
            else {
                EndOnFloor();
            }
        }

        public void EndOnFloor() {
            _animator.SetBool("OnFloor", false);
            if(_playing) {
                _animator.SetTrigger("Playing");
            }
            else {
                _animator.SetTrigger("Talking");
            }
            _drum.localPosition = new Vector3(-0.02959965f, 0.2917893f, 0.004725051f);
            _drum.localEulerAngles = Vector3.zero;
        }

        public void StartOnFloor() {
            _animator.SetBool("OnFloor", true);
            if(_playing) {
                _animator.SetTrigger("Playing");
            }
            else {
                _animator.SetTrigger("Talking");
            }
            _drum.localPosition = new Vector3(0.008f, 0.477f, 0.069f);
            _drum.localEulerAngles = new Vector3(2.929f, 10.203f, -13.912f);
        }

        public void StopPlaying() {
            _playing = false;
            AudioSignal._active = false;
            AudioSignal.GetOWAudioSource().FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
            _animator.SetTrigger("Talking");
        }

        public void StartPlaying() {
            _playing = true;
            AudioSignal._active = true;
            AudioSignal.GetOWAudioSource().FadeIn(0.5f, false, false, 1f);
            AudioSignal.GetOWAudioSource().timeSamples = 0;
            _animator.SetTrigger("Playing");
        }

        void Update() {
            if(_animator != null && !_animator.enabled) {
                _animator.enabled = true;
            }
        }
    }
}
