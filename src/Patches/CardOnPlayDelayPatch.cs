using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace NextTurntheSpire.Patches;

[HarmonyPatch]
internal static class CardOnPlayDelayPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        return AccessTools.AllTypes()
            .Where(t => t != null && !t.IsAbstract && typeof(CardModel).IsAssignableFrom(t))
            .Select(t => AccessTools.DeclaredMethod(t, "OnPlay", new[]
            {
                typeof(PlayerChoiceContext),
                typeof(CardPlay)
            }))
            .Where(m => m != null)
            .Cast<MethodBase>();
    }

    private static bool Prefix(CardModel __instance, CardPlay cardPlay, ref Task __result)
    {
        if (DeferredCardPlayRuntime.IsExecutingDeferredEffect)
        {
            return true;
        }

        Creature owner = __instance.Owner.Creature;

        string currentCardDescription = __instance.Description.GetFormattedText();
        string rawCardEffectDescription = DeferredCardPlayRuntime.StripNextTurnPrefix(currentCardDescription);

        DeferredCardPlayPower deferredPower = ((DeferredCardPlayPower)ModelDb.Power<DeferredCardPlayPower>()
            .ToMutable())
            .InitializeFrom(__instance, cardPlay, rawCardEffectDescription);

        __result = PowerCmd.Apply(deferredPower, owner, 1m, owner, __instance);
        return false;
    }
}
