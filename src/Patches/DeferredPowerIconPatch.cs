using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace NextTurntheSpire.Patches;

[HarmonyPatch(typeof(PowerModel), "get_PackedIconPath")]
internal static class DeferredPowerPackedIconPatch
{
    private static bool Prefix(PowerModel __instance, ref string __result)
    {
        if (__instance is not DeferredCardPlayPower)
        {
            return true;
        }

        __result = ImageHelper.GetImagePath("atlases/power_atlas.sprites/retain_hand_power.tres");
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), "get_ResolvedBigIconPath")]
internal static class DeferredPowerBigIconPatch
{
    private static bool Prefix(PowerModel __instance, ref string __result)
    {
        if (__instance is not DeferredCardPlayPower)
        {
            return true;
        }

        // Keep tooltip portrait consistent with Retain Hand so missing icon fallback is never used.
        __result = ResolveRetainHandBigIconPath();
        return false;
    }

    private static string ResolveRetainHandBigIconPath()
    {
        string normal = ImageHelper.GetImagePath("powers/retain_hand_power.png");
        if (Godot.ResourceLoader.Exists(normal))
        {
            return normal;
        }

        string beta = ImageHelper.GetImagePath("powers/beta/retain_hand_power.png");
        if (Godot.ResourceLoader.Exists(beta))
        {
            return beta;
        }

        return ImageHelper.GetImagePath("powers/missing_power.png");
    }
}
