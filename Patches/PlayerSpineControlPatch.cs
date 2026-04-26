using HarmonyLib;

namespace ViewmodelOffset;

[HarmonyPatch(typeof(PlayerSpineControl))]
public static class PlayerSpineControlPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("CorrectSpine")]
    public static void CorrectSpine_Prefix(PlayerSpineControl __instance)
    {
        PlayerMain pm = __instance.GetComponentInParent<PlayerMain>();
        if (pm != null && !pm.ForeignPlayer && ViewmodelOffset.shouldFlip)
        {
            __instance.deviationY = -__instance.deviationY;
        }
    }
}
