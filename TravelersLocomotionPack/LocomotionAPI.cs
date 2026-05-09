using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public class LocomotionAPI : ILocomotion {
        Chert _chert;

        public void RiebeckStandUp() {
            throw new NotImplementedException();
        }

        public void ChertInitialize(GameObject chert) {
            _chert = chert.AddComponent<Chert>();
            _chert.Initialize();
        }

        public void ChertSitDown() {
            _chert.SitDown();
        }

        public void ChertEndOnFloor() {
            _chert.EndOnFloor();
        }
    }
}
