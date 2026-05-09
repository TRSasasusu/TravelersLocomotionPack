using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public class Chert : MonoBehaviour {
        Animator _animator;
        TravelerController _traverlerController;
        Transform _drum;

        public void Initialize() {
            _animator = GetComponentInChildren<Animator>();
            _animator.runtimeAnimatorController = TravelersLocomotionPack.ChertAnimatorController;
            _animator.SetBool("OnFloor", false);

            _traverlerController = GetComponent<TravelerController>();

            _drum = transform.Find("Traveller_HEA_Chert_ANIM_Chatter_Chipper/NewDrum:pCylinder1");
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
            if(_traverlerController && _traverlerController._talking) {
                _animator.SetTrigger("Talking");
            }
            else {
                _animator.SetTrigger("Playing");
            }
            _drum.localPosition = new Vector3(-0.02959965f, 0.2917893f, 0.004725051f);
            _drum.localEulerAngles = Vector3.zero;
        }

        public void StartOnFloor() {
            _animator.SetBool("OnFloor", true);
            if(_traverlerController && _traverlerController._talking) {
                _animator.SetTrigger("Talking");
            }
            else {
                _animator.SetTrigger("Playing");
            }
            _drum.localPosition = new Vector3(0.008f, 0.477f, 0.069f);
            _drum.localEulerAngles = new Vector3(2.929f, 10.203f, -13.912f);
        }
    }
}
