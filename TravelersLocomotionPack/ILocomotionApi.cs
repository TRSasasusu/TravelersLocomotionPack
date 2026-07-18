using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TravelersLocomotionPack {
    public interface ILocomotion {
        public void ChertInitialize(GameObject chert);

        public GameObject GetChert();

        public void ChertSitDown();

        public void ChertEndOnFloor();

        public void ChertStartPlaying();

        public void ChertStopPlaying();

        public void GabbroInitialize(GameObject gabbro);

        public bool GabbroIsInitialized();

        public GameObject GetGabbro();

        public void GabbroStandUp();

        public void GabbroMoveTo(Transform target, float radius, float speed, Vector3 offset);

        public void GabbroMoveStop();

        public void GabbroLookAt(Transform target, Vector3 offset);

        public void GabbroStopPlaying();

        public void GabbroStartPlaying();

        public void GabbroSitting();

        public void RiebeckInitialize(GameObject riebeck);

        public bool RiebeckIsInitialized();

        public GameObject GetRiebeck();

        public void RiebeckStandUp();

        public void RiebeckMoveTo(Transform target, float radius, float speed, Vector3 offset);

        public void RiebeckMoveStop();

        public void RiebeckLookAt(Transform target, Vector3 offset);

        public void RiebeckStopPlaying();

        public void RiebeckStartPlaying();

        public void RiebeckSitting();
    }
}
