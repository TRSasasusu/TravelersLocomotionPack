using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using NewHorizons.Utility;

namespace TravelersLocomotionPack {
    public class Riebeck : Traveler {
        public static bool IsStanding { get { return Instance != null && Instance._isStanding; } }
        public static bool IsStopPlaying { get { return Instance != null && !Instance._playing; } }
        public static Riebeck Instance { get; private set; }

        CharacterDialogueTree _conversationZoneDialogueTree;

        public override void Initialize() {
            base.Initialize();
            Instance = this;

            _animator.runtimeAnimatorController = TravelersLocomotionPack.RiebeckAnimatorController;
            GetComponent<CapsuleCollider>().height = 2;
        }

        public override void StandUp() {
            var owRigidbody = GetComponent<OWRigidbody>();
            //owRigidbody._kinematicSimulation = true;
            var cachedParent = transform.parent;
            //TravelersLocomotionPack.Log($"cachedParent: {cachedParent.GetPath()}");
            owRigidbody.Suspend(); // Suspend makes its parent BrittleHollow_Body
            transform.parent = cachedParent;

            var leftCollider = cachedParent.Find("CapsuleCollider");
            if (leftCollider != null) {
                leftCollider.gameObject.SetActive(false);
            }

            base.StandUp();

            if(!_isStanding) {
                _travelerController._animator = _animator;
                _conversationZoneDialogueTree = GetComponentInChildren<CharacterDialogueTree>(true);
            }

            _isStanding = true;
            _animator.SetBool("NoSitting", true);
            _animator.SetBool("GroundSitting", false);

            if(AudioSignal != null) {
                AudioSignal.transform.localPosition = new Vector3(0.045f, 1.214f, 0.5696f);
            }
            if(_conversationZoneDialogueTree != null) {
                _conversationZoneDialogueTree.transform.localPosition = new Vector3(0, 1.511f, 0);
                _conversationZoneDialogueTree._attentionPoint.localPosition = new Vector3(0, 1.511f, 0);
            }

            if(_playing) {
                _animator.SetTrigger("Playing");
            }
            else {
                _animator.SetTrigger("Talking");
            }

            transform.DOLocalMoveZ(1.02f, 0.5f).OnComplete(() => {
                //owRigidbody._kinematicSimulation = false;
                owRigidbody.Unsuspend();
            }).SetLink(gameObject);

            _conversationZone = GetComponentInChildren<CharacterDialogueTree>(true).GetComponent<SphereCollider>();
        }

        public override void Sitting() {
            _animator.SetBool("NoSitting", false);
            _animator.SetBool("GroundSitting", true);
            base.Sitting();

            if(AudioSignal != null) {
                AudioSignal.transform.localPosition = new Vector3(-0.0226f, 0.5882f, 0.5058f);
            }
            if(_conversationZoneDialogueTree != null) {
                _conversationZoneDialogueTree.transform.localPosition = new Vector3(0, 0.9348f, 0);
                _conversationZoneDialogueTree._attentionPoint.localPosition = new Vector3(0, 0.9348f, 0);
            }
        }
    }
}
