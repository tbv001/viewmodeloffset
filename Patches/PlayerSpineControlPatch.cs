using HarmonyLib;

namespace ViewmodelOffset;

[HarmonyPatch(typeof(PlayerSpineControl))]
public static class PlayerSpineControlPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("CorrectSpine")]
    public static void CorrectSpine_Prefix(PlayerSpineControl __instance)
    {
        if (ViewmodelOffset.shouldFlip)
        {
            __instance.deviationY = -__instance.deviationY;
        }
    }
}
