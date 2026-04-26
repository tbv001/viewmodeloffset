using HarmonyLib;
using UnityEngine;

namespace ViewmodelOffset;

[HarmonyPatch(typeof(PlayerCamera))]
public static class PlayerCameraPatch
{
    private static Vector3 _currentOffset;
    private static readonly Vector3 _gameBaseOffset = Vector3.up * 0.05f;
    private const float _lerpSpeed = 15f;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerCamera.UpdateCamera))]
    public static void UpdateCamera_Postfix(PlayerCamera __instance)
    {
        if (__instance.playerMain == null || __instance.playerMain.ForeignPlayer)
            return;

        if (__instance.mode != PlayerCamera.Mode.FirstPerson)
        {
            if (__instance.playerMain.arms != null && __instance.playerMain.arms.transform.localScale.x < 0f)
            {
                Flip(__instance.playerMain.arms);
            }
            return;
        }

        CharacterSkin skin = __instance.playerMain.SpawnedSkin;
        if (skin == null || skin.transform == null) return;

        PlayerArms arms = __instance.playerMain.arms;
        if (arms == null) return;

        bool isADS = arms.ads;

        if (isADS || ViewmodelOffset.viewmodelOffset == Vector3.zero)
        {
            _currentOffset = Vector3.Lerp(_currentOffset, _gameBaseOffset, Time.deltaTime * _lerpSpeed);
        }
        else
        {
            Transform parent = skin.transform.parent;
            Vector3 worldOffset = __instance.camTransform.TransformDirection(ViewmodelOffset.viewmodelOffset);
            Vector3 localOffset = (parent != null) ? parent.InverseTransformDirection(worldOffset) : worldOffset;

            _currentOffset = Vector3.Lerp(_currentOffset, _gameBaseOffset + localOffset, Time.deltaTime * _lerpSpeed);
        }

        skin.transform.localPosition = _currentOffset;

        if (arms.transform.localScale.x > 0f && ViewmodelOffset.shouldFlip)
        {
            Flip(arms);
        }
        else if (arms.transform.localScale.x < 0f && !ViewmodelOffset.shouldFlip)
        {
            Flip(arms);
        }
    }

    private static void Flip(PlayerArms arms)
    {
        Vector3 curScale = arms.transform.localScale;
        curScale.x *= -1f;
        arms.transform.localScale = curScale;
    }
}
