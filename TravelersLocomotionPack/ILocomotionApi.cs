using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public interface ILocomotion {
        public void RiebeckStandUp();

        public void ChertInitialize(GameObject chert);

        public void ChertSitDown();

        public void ChertEndOnFloor();
    }
}
