using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace ViewmodelOffset;

[BepInPlugin("com.theblackvoid.viewmodeloffset", "Viewmodel Offset", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static Vector3 ViewmodelOffset = Vector3.zero;
    private static ViewmodelOffsetApplier ViewmodelOffsetApplierInstance;

    private void Awake()
    {
        Logger = base.Logger;

        // Config stuff
        ConfigFile cfg = base.Config;

        ConfigEntry<float> offsetX = cfg.Bind("Offset", "X (Right/Left)", -0.05f, new ConfigDescription("X viewmodel offset. Positive = right, negative = left.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        ConfigEntry<float> offsetY = cfg.Bind("Offset", "Y (Up/Down)", -0.1f, new ConfigDescription("Y viewmodel offset. Positive = up, negative = down.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        ConfigEntry<float> offsetZ = cfg.Bind("Offset", "Z (Forward/Backward)", -0.05f, new ConfigDescription("Z viewmodel offset. Positive = forward, negative = backward.", new AcceptableValueRange<float>(-0.5f, 0.5f)));

        ViewmodelOffset = new Vector3(offsetX.Value, offsetY.Value, offsetZ.Value);

        // Create the LateUpdate applier component
        GameObject go = new GameObject("ViewmodelOffsetApplier");
        go.hideFlags = HideFlags.HideAndDontSave;
        ViewmodelOffsetApplierInstance = go.AddComponent<ViewmodelOffsetApplier>();
        DontDestroyOnLoad(go);

        Logger.LogInfo($"Loaded!");
        Logger.LogInfo($"Current offset: {ViewmodelOffset}");
    }

    private class ViewmodelOffsetApplier : MonoBehaviour
    {
        private static Vector3 gameBaseOffset = Vector3.up * 0.05f; // From SetCameraTransform() method in PlayerCamera class
        private static float lerpSpeed = 10f;
        private PlayerCamera playerCam;
        private PlayerMain playerMain;
        private Transform skinTransform;
        private Vector3 currentOffset;

        private void LateUpdate()
        {
            if (playerCam == null)
            {
                if (PlayerCamera.instance == null)
                    return;
                playerCam = PlayerCamera.instance;
                playerMain = playerCam.playerMain;
            }

            if (playerCam.mode != PlayerCamera.Mode.FirstPerson)
                return;

            if (playerMain?.SpawnedSkin == null)
                return;

            if (ViewmodelOffset == Vector3.zero)
                return;

            if (skinTransform == null)
                skinTransform = playerMain.SpawnedSkin.transform;

            if (playerMain.arms == null)
                return;

            bool isADS = playerMain.arms.ads;

            if (isADS)
            {
                // Return to default offset when ADSing
                currentOffset = Vector3.Lerp(currentOffset, gameBaseOffset, Time.deltaTime * lerpSpeed);
            }
            else
            {
                // Apply our custom offset
                Transform parent = skinTransform.parent;
                Vector3 worldOffset = playerCam.transform.TransformDirection(ViewmodelOffset);
                Vector3 localOffset = (parent != null) ? parent.InverseTransformDirection(worldOffset) : worldOffset;

                currentOffset = Vector3.Lerp(currentOffset, gameBaseOffset + localOffset, Time.deltaTime * lerpSpeed);
            }

            skinTransform.localPosition = currentOffset;
        }
    }
}
