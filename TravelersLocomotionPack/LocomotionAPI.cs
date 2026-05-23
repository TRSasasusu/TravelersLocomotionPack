using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public class LocomotionAPI : ILocomotion {
        Chert _chert;
        Gabbro _gabbro;

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

        public void GabbroInitialize(GameObject gabbro) {
            _gabbro = gabbro.AddComponent<Gabbro>();
            _gabbro.Initialize();
        }

        public void GabbroStandUp() {
            _gabbro.StandUp();
        }

        public void GabbroMoveTo(Vector3 position) {
            throw new NotImplementedException();
        }
    }
}
