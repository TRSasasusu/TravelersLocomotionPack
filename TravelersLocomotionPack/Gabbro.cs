using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
//using UnityEngine.InputSystem.Utilities;
using UniRx;

namespace TravelersLocomotionPack {
    public class Gabbro : MonoBehaviour {
        public static bool IsGabbroStanding { get; private set; }
        public static GameObject _gabbroLPBody;

        const float Speed = 1f;

        DynamicForceDetector _dynamicForceDetector;
        AlignmentForceDetector _alignmentForceDetector;
        AlignWithForce _alignWithForce;
        Animator _animator;
        Vector3? _targetPosition = null;
        Collider[] _collidersForForceVolumes;
        GabbroTravelerController _gabbroTravelerController;
        OWRigidbody _owRigidbody;

        public void Initialize() {
            _animator = GetComponentInChildren<Animator>();
            _animator.runtimeAnimatorController = TravelersLocomotionPack.GabbroAnimatorController;

            _gabbroTravelerController = transform.parent.GetComponent<GabbroTravelerController>();

            _alignmentForceDetector = GetComponentInChildren<AlignmentForceDetector>();
            _dynamicForceDetector = GetComponentInChildren<DynamicForceDetector>();
            //_collidersForForceVolumes = new Collider[10];
            //var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 1, _collidersForForceVolumes);
            //_alignmentForceDetector._activeVolumes = _collidersForForceVolumes.Take(numColliders).Where(c => c.GetComponent<ForceVolume>() != null).Select(v => v.GetComponent<EffectVolume>()).ToList();

            Destroy(GetComponent<SphereCollider>());
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.8f;

            _alignmentForceDetector._collider = capsuleCollider;

            _owRigidbody = GetComponent<OWRigidbody>();
        }

        public void StandUp() {
            if(IsGabbroStanding) {
                return;
            }

            _gabbroTravelerController._animator = _animator;

            _animator.SetTrigger("StandUp");
            _animator.SetBool("NoHammock", true);

            transform.DOLocalMove(new Vector3(0.5932f, 0.131f, 0), 1.5f);
            transform.DOLocalRotate(new Vector3(0, 91.6766f, 0), 1.5f);

            IsGabbroStanding = true;

            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                transform.parent = null;
            });
        }

        public void MoveTo(Vector3 position) {
            _targetPosition = position;
        }

        void Update() {
            //if(_targetPosition.HasValue) {
            //    transform.LookAt(_targetPosition.Value);
            //    //transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value, Time.deltaTime * 0.5f);
            //}
        }

        void FixedUpdate() {
            if(_targetPosition.HasValue) {
                transform.LookAt(_targetPosition.Value);
                _owRigidbody.AddVelocityChange((_targetPosition.Value - transform.position).normalized * Speed);
                _animator.SetFloat("Walk", Speed);
                if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.1f) {
                    _targetPosition = null;
                }
            }
            else {
                _animator.SetFloat("Walk", 0);
            }

            if(_alignmentForceDetector) {
                //var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 1, _collidersForForceVolumes);
                //foreach(var c in _collidersForForceVolumes) {
                //    TravelersLocomotionPack.Log($"Collider: {c?.name}");
                //}
                //_alignmentForceDetector._activeVolumes = _collidersForForceVolumes.Take(numColliders).Where(c => c.GetComponent<ForceVolume>() != null).Select(v => v.GetComponent<EffectVolume>()).ToList();
                _alignmentForceDetector._activeVolumes = _dynamicForceDetector._activeVolumes;
                _alignmentForceDetector._dirty = true;
            }
        }
    }
}
