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
        float _rotationDamping = 1f;

        DynamicForceDetector _dynamicForceDetector;
        AlignmentForceDetector _alignmentForceDetector;
        Animator _animator;
        Transform _target;
        float _targetRadius;
        float _targetSpeed;
        Vector3 _targetOffset;
        GabbroTravelerController _gabbroTravelerController;
        OWRigidbody _owRigidbody;
        GameObject _jetpack;

        public void Initialize() {
            _animator = GetComponentInChildren<Animator>();
            _animator.runtimeAnimatorController = TravelersLocomotionPack.GabbroAnimatorController;

            _gabbroTravelerController = transform.parent.GetComponent<GabbroTravelerController>();

            _alignmentForceDetector = GetComponentInChildren<AlignmentForceDetector>();
            _dynamicForceDetector = GetComponentInChildren<DynamicForceDetector>();

            Destroy(GetComponent<SphereCollider>());
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.8f;

            _alignmentForceDetector._collider = capsuleCollider;

            _owRigidbody = GetComponent<OWRigidbody>();

            gameObject.AddComponent<SectorDetector>();

            _jetpack = Instantiate(ModifyObjects.Instance.Jetpack);
            _jetpack.transform.parent = transform;
            _jetpack.transform.localPosition = new Vector3(0, 1.4834f, -0.3484f);
            //_jetpack.transform.localEulerAngles = new Vector3(0, 299.7281f, 0);
            _jetpack.transform.localEulerAngles = new Vector3(351.186f, 299.7281f, 11.0537f);
            _jetpack.GetComponent<MeshRenderer>().enabled = true;
            _jetpack.SetActive(false);

            var vfx = Instantiate(ModifyObjects.Instance.PlayerVFX);
            Destroy(vfx.GetComponent<PlayerParticlesController>());
            foreach(var component in vfx.GetComponentsInChildren<ThrusterWashController>(true)) {
                Destroy(component);
            }
            foreach(var component in vfx.GetComponentsInChildren<ThrusterLightTracker>(true)) {
                Destroy(component);
            }
            foreach(var component in vfx.GetComponentsInChildren<ThrusterFlameColorSwapper>(true)) {
                Destroy(component);
            }
            foreach(var component in vfx.GetComponentsInChildren<ThrusterFlameController>(true)) {
                Destroy(component);
            }
            foreach(var component in vfx.GetComponentsInChildren<ThrusterParticlesBehavior>(true)) {
                Destroy(component);
            }
            foreach(var component in vfx.GetComponentsInChildren<RelativisticParticleSystem>(true)) {
                Destroy(component);
            }
            vfx.transform.parent = _jetpack.transform;
            vfx.transform.localPosition = new Vector3(0.4202f, -0.2888f, 0.3999f);
            vfx.transform.localEulerAngles = new Vector3(0, 42.6509f, 0);
        }

        public void StandUp() {
            if(IsGabbroStanding) {
                return;
            }

            var conversationZone = GetComponentInChildren<InteractReceiver>(true);
            conversationZone.gameObject.SetActive(false);
            Observable.NextFrame().Subscribe(_ => {
                conversationZone.gameObject.SetActive(true);

                _gabbroTravelerController._animator = _animator;

                _animator.SetTrigger("StandUp");
                _animator.SetBool("NoHammock", true);

                transform.DOLocalMove(new Vector3(0.5932f, 0.131f, 0), 1.5f);
                transform.DOLocalRotate(new Vector3(0, 91.6766f, 0), 1.5f);

                IsGabbroStanding = true;

                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    transform.parent = null;
                }).AddTo(this);
            }).AddTo(this);
        }

        public void MoveTo(Transform targetTransform, float radius, float speed, Vector3 offset) {
            _target = targetTransform;
            _targetRadius = radius;
            _targetSpeed = speed;
            _targetOffset = offset;
        }

        public void MoveStop() {
            if(!_target) {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up * 0.1f, -transform.up, out hit, 0.9f)) {
                var referredRigidbody = hit.collider.GetComponentInParent<OWRigidbody>();
                if (referredRigidbody) {
                    _owRigidbody.SetAngularVelocity(referredRigidbody.GetAngularVelocity());
                }
            }
            _target = null;
            _jetpack.SetActive(false);
        }

        void Update() {
            if(!_animator.enabled) {
                _animator.enabled = true;
            }
            //if(_targetPosition.HasValue) {
            //    transform.LookAt(_targetPosition.Value);
            //    //transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value, Time.deltaTime * 0.5f);
            //}
        }

        void FixedUpdate() {
            if(_target) {
                RaycastHit hit;
                var direction = _target.position + _targetOffset - transform.position;

                bool useJetpack = false;
                var dot = Vector3.Dot(direction.normalized, transform.forward);
                //TravelersLocomotionPack.Log($"Dot: {dot}");
                if(dot < 1f/1.414f) {
                    useJetpack = true;
                }

                Vector3? baseAngularVelocity = null;
                if(!useJetpack && Physics.Raycast(transform.position + transform.up * 0.1f, -transform.up, out hit, 0.9f)) {
                    //TravelersLocomotionPack.Log($"Grounded on: {hit.collider.name}");
                    // grounded
                    var referredRigidbody = hit.collider.GetComponentInParent<OWRigidbody>();
                    if(referredRigidbody) { 
                        var referredVelocity = referredRigidbody.GetPointVelocity(transform.position);
                        var normal = hit.normal.normalized;
                        direction -= Vector3.Dot(direction, normal) * normal;
                        //_owRigidbody.SetVelocity(referredVelocity + (_target.position + _targetOffset - transform.position).normalized * _targetSpeed);
                        _owRigidbody.SetVelocity(referredVelocity + direction.normalized * _targetSpeed);

                        baseAngularVelocity = referredRigidbody.GetAngularVelocity();

                        _jetpack.SetActive(false);
                    }
                }
                else {
                    var referredRigidbody = _target.GetComponentInParent<OWRigidbody>();
                    if(referredRigidbody) { 
                        var referredVelocity = referredRigidbody.GetPointVelocity(transform.position);
                        //_owRigidbody.SetVelocity(referredVelocity + (_target.position + _targetOffset - transform.position).normalized * _targetSpeed);
                        _owRigidbody.SetVelocity(referredVelocity + direction.normalized * _targetSpeed);

                        baseAngularVelocity = referredRigidbody.GetAngularVelocity();

                        _jetpack.SetActive(true);
                    }
                }

                var lookDirection = direction;//_target.position + _targetOffset - transform.position;
                var cross = Vector3.Cross(transform.forward, lookDirection.normalized);
                var torque = cross * _rotationSpeed;
                torque -= _owRigidbody.GetAngularVelocity() * _rotationDamping;
                //_owRigidbody.AddTorque(torque);
                //TravelersLocomotionPack.Log($"Torque: {torque}");
                _owRigidbody.AddAngularVelocityChange(torque);

                //transform.LookAt(_targetPosition.Value);
                //_owRigidbody.AddVelocityChange((_targetPosition.Value - transform.position).normalized * Speed);
                _animator.SetFloat("Walk", _targetSpeed);
                if (Vector3.Distance(transform.position, _target.position + _targetOffset) < _targetRadius) {
                    _target = null;
                    if(baseAngularVelocity != null) {
                        _owRigidbody.SetAngularVelocity(baseAngularVelocity.Value);
                    }
                    _jetpack.SetActive(false);
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
