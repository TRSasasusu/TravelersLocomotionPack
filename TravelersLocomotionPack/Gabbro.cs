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
        public static bool IsGabbroStanding { get { return Instance != null && Instance._isStanding; } }
        public static bool IsStopPlaying { get { return Instance != null && !Instance._playing; } }
        public static Gabbro Instance { get; private set; }
        public static GameObject _gabbroLPBody;

        public AudioSignal AudioSignal { get; private set; }

        bool _playing;
        float _speed = 2f;
        float _rotationSpeed = 5f;
        float _rotationDamping = 1f;
        bool _isStanding;

        DynamicForceDetector _dynamicForceDetector;
        AlignmentForceDetector _alignmentForceDetector;
        Animator _animator;
        Transform _target;
        float _targetRadius;
        float _targetSpeed;
        Vector3 _targetOffset;
        GabbroTravelerController _gabbroTravelerController;
        OWRigidbody _owRigidbody;
        Collider _conversationZone;

        GameObject _jetpack;
        float _jetpackInitialDistanceToTarget = -1;
        float _jetpackVelocityCoeff = 0.01f;
        float _jetpackPosCoeff = 0.01f;
        float _jetpackAccelCoeff = 0.01f;
        MeshRenderer _jetpackUpThruster;
        MeshRenderer _jetpackDownThruster;
        MeshRenderer _jetpackLeftThruster;
        MeshRenderer _jetpackRightThruster;
        MeshRenderer _jetpackBackwardLeftThruster;
        MeshRenderer _jetpackBackwardRightThruster;
        MeshRenderer _jetpackForwardLeftThruster;
        MeshRenderer _jetpackForwardRightThruster;

        public void Initialize() {
            Instance = this;

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

            ModifyObjects.Instance.PlayerVFX.SetActive(false);
            var vfx = Instantiate(ModifyObjects.Instance.PlayerVFX);
            ModifyObjects.Instance.PlayerVFX.SetActive(true);
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
            vfx.SetActive(true);
            _jetpackUpThruster = vfx.transform.Find("Thrusters/UpThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackDownThruster = vfx.transform.Find("Thrusters/DownThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackLeftThruster = vfx.transform.Find("Thrusters/LeftThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackRightThruster = vfx.transform.Find("Thrusters/RightThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackBackwardLeftThruster = vfx.transform.Find("Thrusters/BackwardLeftThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackBackwardRightThruster = vfx.transform.Find("Thrusters/BackwardRightThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackForwardLeftThruster = vfx.transform.Find("Thrusters/ForwardLeftThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
            _jetpackForwardRightThruster = vfx.transform.Find("Thrusters/ForwardRightThrust/Effects_HEA_ThrusterFlame").GetComponent<MeshRenderer>();
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

                _isStanding = true;

                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => {
                    transform.parent = null;
                    _animator.SetTrigger("Playing");
                }).AddTo(this);

                _conversationZone = GetComponentInChildren<CharacterDialogueTree>(true).GetComponent<SphereCollider>();
                AudioSignal = GetComponentInChildren<AudioSignal>(true);
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
                    //_owRigidbody.SetVelocity(referredRigidbody.GetPointVelocity(transform.position));
                }
            }
            else {
                var referredRigidbody = _target.GetComponentInParent<OWRigidbody>();
                if(referredRigidbody) {
                    _owRigidbody.SetAngularVelocity(referredRigidbody.GetAngularVelocity());
                    _owRigidbody.SetVelocity(referredRigidbody.GetPointVelocity(transform.position));
                }
            }

            _target = null;
            _jetpack.SetActive(false);
            _jetpackInitialDistanceToTarget = -1;
        }

        void Update() {
            if(!_animator.enabled) {
                _animator.enabled = true;
            }
            if(_conversationZone != null && !_conversationZone.enabled) {
                _conversationZone.enabled = true;
            }
            //if(_targetPosition.HasValue) {
            //    transform.LookAt(_targetPosition.Value);
            //    //transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value, Time.deltaTime * 0.5f);
            //}
        }

        void FixedUpdate() {
            if(_target) {
                RaycastHit hit;
                var direction = _target.position + _target.TransformDirection(_targetOffset) - transform.position;

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

                        _jetpackInitialDistanceToTarget = -1;
                    }
                }
                else {
                    var referredRigidbody = _target.GetComponentInParent<OWRigidbody>();
                    if(referredRigidbody) { 
                        var diffPos = _target.position + _target.TransformDirection(_targetOffset) - transform.position;
                        diffPos -= diffPos.normalized * _targetRadius;
                        var currentDistanceToTarget = diffPos.magnitude;

                        if(_jetpackInitialDistanceToTarget < 0) {
                            _jetpackInitialDistanceToTarget = currentDistanceToTarget;
                        }

                        var diffVelocity = referredRigidbody.GetPointVelocity(_target.position + _target.TransformDirection(_targetOffset)) - _owRigidbody.GetVelocity();
                        diffVelocity -= Vector3.Dot(diffVelocity, diffPos.normalized) * diffPos.normalized;

                        var currentAccel = _dynamicForceDetector.GetForceAcceleration(); //_owRigidbody._currentAccel;
                        //TravelersLocomotionPack.Log($"currentAccel: {currentAccel}");
                        //diffVelocity -= Vector3.Dot(diffVelocity, currentAccel.normalized) * currentAccel.normalized;
                        currentAccel -= Vector3.Dot(currentAccel, diffPos.normalized) * diffPos.normalized;

                        Vector3 force = Vector3.zero;
                        if(diffVelocity.magnitude > 0.5f) {
                            force += diffVelocity.normalized * _jetpackVelocityCoeff;
                        }

                        if(currentDistanceToTarget > _jetpackInitialDistanceToTarget * 0.5f) {
                            force += diffPos.normalized * _jetpackPosCoeff;
                            force -= currentAccel.normalized * _jetpackAccelCoeff;
                        }
                        else {
                            force -= diffPos.normalized * _jetpackPosCoeff;
                        }

                        _owRigidbody.AddForce(force);

                        var normalizedForce = force.normalized;
                        _jetpackUpThruster.enabled = Vector3.Dot(normalizedForce, transform.up) > 0;
                        _jetpackDownThruster.enabled = Vector3.Dot(normalizedForce, transform.up) < 0;
                        _jetpackLeftThruster.enabled = Vector3.Dot(normalizedForce, transform.right) < 0;
                        _jetpackRightThruster.enabled = Vector3.Dot(normalizedForce, transform.right) > 0;
                        _jetpackBackwardLeftThruster.enabled = Vector3.Dot(normalizedForce, transform.forward) > 0;
                        _jetpackBackwardRightThruster.enabled = Vector3.Dot(normalizedForce, transform.forward) > 0;
                        _jetpackForwardLeftThruster.enabled = Vector3.Dot(normalizedForce, transform.forward) < 0;
                        _jetpackForwardRightThruster.enabled = Vector3.Dot(normalizedForce, transform.forward) < 0;

                        ////_owRigidbody.SetVelocity(referredVelocity + (_target.position + _targetOffset - transform.position).normalized * _targetSpeed);
                        //_owRigidbody.SetVelocity(referredVelocity + direction.normalized * _targetSpeed);

                        //baseAngularVelocity = referredRigidbody.GetAngularVelocity();

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
                if (Vector3.Distance(transform.position, _target.position + _target.TransformDirection(_targetOffset)) < _targetRadius) {
                    if(baseAngularVelocity != null) {
                        _owRigidbody.SetAngularVelocity(baseAngularVelocity.Value);
                    }
                    else {
                        var referredRigidbody = _target.GetComponentInParent<OWRigidbody>();
                        if(referredRigidbody) {
                            //_owRigidbody.SetAngularVelocity(referredRigidbody.GetAngularVelocity());
                            _owRigidbody.SetVelocity(referredRigidbody.GetPointVelocity(transform.position));
                        }
                    }
                    _target = null;
                    _jetpack.SetActive(false);
                    _jetpackInitialDistanceToTarget = -1;
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
    }
}
