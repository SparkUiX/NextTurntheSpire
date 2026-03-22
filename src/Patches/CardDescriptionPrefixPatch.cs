using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace NextTurntheSpire.Patches;

[HarmonyPatch(typeof(LocString), nameof(LocString.GetFormattedText))]
internal static class CardDescriptionPrefixPatch
{
    private static void Postfix(LocString __instance, ref string __result)
    {
        if (!string.Equals(__instance.LocTable, "cards", StringComparison.Ordinal))
        {
            return;
        }

        if (!__instance.LocEntryKey.EndsWith(".description", StringComparison.Ordinal))
        {
            return;
        }

        if (__result.StartsWith(DeferredCardPlayRuntime.NextTurnPrefixText, StringComparison.Ordinal))
        {
            return;
        }

        __result = DeferredCardPlayRuntime.NextTurnPrefixText + __result;
    }
}
