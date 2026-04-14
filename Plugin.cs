using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace ViewmodelOffset;

[BepInPlugin("com.theblackvoid.viewmodeloffset", "Viewmodel Offset", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static Harmony HarmonyInstance = new Harmony("com.theblackvoid.viewmodeloffset");
    public static Vector3 ViewmodelOffset = Vector3.zero;
    public static bool shouldFlip = false;
    private static ViewmodelOffsetApplier ViewmodelOffsetApplierInstance;

    private void Awake()
    {
        Logger = base.Logger;

        // Config stuff
        ConfigFile cfg = base.Config;

        ConfigEntry<float> offsetX = cfg.Bind("Offset", "X (Right/Left)", -0.05f, new ConfigDescription("X viewmodel offset. Positive = right, negative = left.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        ConfigEntry<float> offsetY = cfg.Bind("Offset", "Y (Up/Down)", -0.1f, new ConfigDescription("Y viewmodel offset. Positive = up, negative = down.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        ConfigEntry<float> offsetZ = cfg.Bind("Offset", "Z (Forward/Backward)", -0.05f, new ConfigDescription("Z viewmodel offset. Positive = forward, negative = backward.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
        ConfigEntry<bool> flip = cfg.Bind("Offset", "Flip", false, new ConfigDescription("Whether the viewmodel should be flipped (mirrored) or not."));

        ViewmodelOffset = new Vector3(offsetX.Value, offsetY.Value, offsetZ.Value);
        shouldFlip = flip.Value;

        // Create the LateUpdate applier component
        GameObject go = new GameObject("ViewmodelOffsetApplier");
        go.hideFlags = HideFlags.HideAndDontSave;
        ViewmodelOffsetApplierInstance = go.AddComponent<ViewmodelOffsetApplier>();
        DontDestroyOnLoad(go);

        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        Logger.LogInfo($"Loaded!");
        Logger.LogInfo($"Current offset: {ViewmodelOffset}, Flip/mirror: {shouldFlip}");
    }

    [HarmonyPatch(typeof(PlayerSpineControl), "CorrectSpine")]
    public static class SpineCorrectionPatch
    {
        [HarmonyPrefix]
        public static void Prefix(PlayerSpineControl __instance)
        {
            if (shouldFlip)
            {
                __instance.deviationY = -__instance.deviationY;
            }
        }
    }

    private class ViewmodelOffsetApplier : MonoBehaviour
    {
        private readonly Vector3 gameBaseOffset = Vector3.up * 0.05f; // From SetCameraTransform() method in PlayerCamera class
        private const float lerpSpeed = 10f;
        private PlayerCamera playerCam;
        private PlayerMain playerMain;
        private Transform skinTransform;
        private Vector3 currentOffset;

        private void Flip()
        {
            if (playerMain?.arms == null)
                return;

            Vector3 curScale = playerMain.arms.transform.localScale;
            curScale.x = curScale.x * -1f;
            playerMain.arms.transform.localScale = curScale;
        }

        private void LateUpdate()
        {
            if (playerCam == null)
            {
                if (PlayerCamera.instance == null)
                    return;
                playerCam = PlayerCamera.instance;
                playerMain = playerCam.playerMain;
            }

            if (playerMain == null)
                return;

            if (playerCam.mode != PlayerCamera.Mode.FirstPerson)
            {
                if (playerMain.arms != null && playerMain.arms.transform.localScale.x < 0f)
                {
                    Flip();
                }
                return;
            }

            if (playerMain.SpawnedSkin == null)
                return;

            if (skinTransform == null)
                skinTransform = playerMain.SpawnedSkin.transform;

            if (playerMain.arms == null)
                return;

            bool isADS = playerMain.arms.ads;

            if (isADS || ViewmodelOffset == Vector3.zero)
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

            if (playerMain.arms.transform.localScale.x > 0f && shouldFlip)
            {
                Flip();
            }
            else if (playerMain.arms.transform.localScale.x < 0f && !shouldFlip)
            {
                Flip();
            }
        }
    }
}
