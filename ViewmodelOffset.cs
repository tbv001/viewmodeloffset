using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System;

namespace ViewmodelOffset;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class ViewmodelOffset : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public const string PLUGIN_GUID = "com.theblackvoid.viewmodeloffset";
    public const string PLUGIN_NAME = "Viewmodel Offset";
    public const string PLUGIN_VERSION = "1.1.0";
    private Harmony HarmonyInstance = new Harmony(PLUGIN_GUID);
    public static Vector3 viewmodelOffset = Vector3.zero;
    public static bool shouldFlip = false;

    private void Awake()
    {
        Logger = base.Logger;
        try
        {
            ConfigEntry<float> offsetX = Config.Bind("Offset", "X (Right/Left)", -0.05f, new ConfigDescription("X viewmodel offset. Positive = right, negative = left.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
            ConfigEntry<float> offsetY = Config.Bind("Offset", "Y (Up/Down)", -0.1f, new ConfigDescription("Y viewmodel offset. Positive = up, negative = down.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
            ConfigEntry<float> offsetZ = Config.Bind("Offset", "Z (Forward/Backward)", -0.05f, new ConfigDescription("Z viewmodel offset. Positive = forward, negative = backward.", new AcceptableValueRange<float>(-0.5f, 0.5f)));
            ConfigEntry<bool> flip = Config.Bind("Offset", "Flip", false, new ConfigDescription("Whether the viewmodel should be flipped (mirrored) or not."));

            viewmodelOffset = new Vector3(offsetX.Value, offsetY.Value, offsetZ.Value);
            shouldFlip = flip.Value;

            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Successfully loaded!");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to load: {ex}");
        }
    }
}
