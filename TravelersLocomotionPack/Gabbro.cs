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

        float _speed = 2f;
        float _rotationSpeed = 5f;
        float _rotationDamping = 0.1f;

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
                RaycastHit hit;
                if(Physics.Raycast(transform.position, -transform.up, out hit, 0.7f)) {
                    TravelersLocomotionPack.Log($"Grounded on: {hit.collider.name}");
                    // grounded
                    var referredRigidbody = hit.collider.GetComponentInParent<OWRigidbody>();
                    if(referredRigidbody) { 
                        var referredVelocity = referredRigidbody.GetPointVelocity(transform.position);
                        _owRigidbody.SetVelocity(referredVelocity + (_targetPosition.Value - transform.position).normalized * _speed);
                    }
                }
                //transform.LookAt(_targetPosition.Value);
                //_owRigidbody.AddVelocityChange((_targetPosition.Value - transform.position).normalized * Speed);
                _animator.SetFloat("Walk", _speed);
                if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.5f) {
                    _targetPosition = null;
                }

                var lookDirection = _targetPosition.Value - transform.position;
                var cross = Vector3.Cross(transform.forward, lookDirection.normalized);
                var torque = cross * _rotationSpeed;
                torque -= _owRigidbody.GetAngularVelocity() * _rotationDamping;
                //_owRigidbody.AddTorque(torque);
                TravelersLocomotionPack.Log($"Torque: {torque}");
                _owRigidbody.AddAngularVelocityChange(torque);
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
