using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelersLocomotionPack {
    public class Riebeck : Traveler {
        public static bool IsStanding { get { return Instance != null && Instance._isStanding; } }
        public static Riebeck Instance { get; private set; }

        public override void Initialize() {
            base.Initialize();
            Instance = this;

            _animator.runtimeAnimatorController = TravelersLocomotionPack.RiebeckAnimatorController;
        }

        public override void StandUp() {
            base.StandUp();
            _isStanding = true;
            _animator.SetBool("NoSitting", true);
            _animator.SetBool("GroundSitting", false);

            if(_playing) {
                _animator.SetTrigger("Playing");
            }
            else {
                _animator.SetTrigger("Talking");
            }
        }

        public override void Sitting() {
            _animator.SetBool("NoSitting", false);
            _animator.SetBool("GroundSitting", true);
            base.Sitting();
        }
    }
}
