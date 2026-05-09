using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace TravelersLocomotionPack {
    public class TravelersLocomotionPack : ModBehaviour {
        public static TravelersLocomotionPack Instance { get; private set; }

        public static RuntimeAnimatorController RiebeckAnimatorController { get; private set; }
        public static RuntimeAnimatorController ChertAnimatorController { get; private set; }

        public static void Log(string text, MessageType messageType = MessageType.Message) {
            Instance.ModHelper.Console.WriteLine(text, messageType);
        }

        public override object GetApi() {
            return new LocomotionAPI();
        }

        //public async void ExportAnimation(string rootLevelNodePath, string exportName) {
        //    var rootLevelNode = GameObject.Find(rootLevelNodePath);
        //    var export = new GLTFast.Export.GameObjectExport();
        //    export.AddScene(new GameObject[] { rootLevelNode });
        //    bool success = await export.SaveToFileAndDispose($"export/{exportName}");
        //    if (!success) {

        //    }
        //}
        private void Awake() {
            Instance = this;
        }

        private void Start() {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"{nameof(TravelersLocomotionPack)} is loaded!", MessageType.Success);

            var bundle = ModHelper.Assets.LoadBundle("assets/assetbundles/travelerslocomotions");
            RiebeckAnimatorController = bundle.LoadAsset<RuntimeAnimatorController>("Assets/MyAssets/Animators/riebeck/riebeck.controller");
            ChertAnimatorController = bundle.LoadAsset<RuntimeAnimatorController>("Assets/MyAssets/Animators/chert/chert.controller");
            Log($"{RiebeckAnimatorController}");

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) => {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
            };
        }
    }

}
