using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace NextTurntheSpire;

internal static class DeferredCardPlayRuntime
{
    private static readonly Dictionary<Type, MethodInfo?> OnPlayMethodCache = new();

    public const string NextTurnPrefixText = "在下回合，";

    public static bool IsExecutingDeferredEffect { get; private set; }

    public static string StripNextTurnPrefix(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (text.StartsWith(NextTurnPrefixText, StringComparison.Ordinal))
        {
            return text[NextTurnPrefixText.Length..];
        }

        return text;
    }

    public static async Task ExecuteDeferredCardPlay(PlayerChoiceContext choiceContext, DeferredCardPlayPower power)
    {
        CardModel? card = power.CardToReplay;
        if (card == null)
        {
            return;
        }

        CardPlay replay = new CardPlay
        {
            Card = card,
            Target = power.TargetToReplay,
            ResultPile = power.ResultPile,
            Resources = power.Resources,
            IsAutoPlay = power.IsAutoPlay,
            PlayIndex = power.PlayIndex,
            PlayCount = power.PlayCount
        };

        IsExecutingDeferredEffect = true;
        try
        {
            MethodInfo? method = ResolveOnPlayMethod(card.GetType());
            if (method == null)
            {
                Log.Warn($"[NextTurntheSpire] Could not resolve OnPlay for card type {card.GetType().FullName}");
                return;
            }

            object? maybeTask = method.Invoke(card, new object[] { choiceContext, replay });
            if (maybeTask is Task task)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[NextTurntheSpire] Deferred card execution failed for {card.Id}: {ex}");
        }
        finally
        {
            IsExecutingDeferredEffect = false;
        }
    }

    private static MethodInfo? ResolveOnPlayMethod(Type cardType)
    {
        if (OnPlayMethodCache.TryGetValue(cardType, out MethodInfo? cached))
        {
            return cached;
        }

        MethodInfo? method = AccessTools.Method(cardType, "OnPlay", new[]
        {
            typeof(PlayerChoiceContext),
            typeof(CardPlay)
        });

        OnPlayMethodCache[cardType] = method;
        return method;
    }
}
