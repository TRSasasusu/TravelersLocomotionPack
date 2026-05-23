using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace TravelersLocomotionPack {
    public class Gabbro : MonoBehaviour {
        Animator _animator;
        Vector3? _targetPosition = null;

        public void Initialize() {
            _animator = GetComponentInChildren<Animator>();
            _animator.runtimeAnimatorController = TravelersLocomotionPack.GabbroAnimatorController;
        }

        public void StandUp() {
            //transform.DORotate()
            _animator.SetTrigger("StandUp");
            _animator.SetBool("NoHammock", true);

            transform.DOLocalMove(new Vector3(0.5932f, 0.131f, 0), 1.5f);
            transform.DOLocalRotate(new Vector3(0, 91.6766f, 0), 1.5f);
        }

        public void MoveTo(Vector3 position) {
            _targetPosition = position;
        }
    }
}
