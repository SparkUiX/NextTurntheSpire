using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace NextTurntheSpire;

public sealed class DeferredCardPlayPower : PowerModel
{
    private CardModel? _cardToReplay;

    private Creature? _targetToReplay;

    private string _cardDescription = string.Empty;

    public override bool IsInstanced => true;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.None;

    public CardModel? CardToReplay => _cardToReplay;

    public Creature? TargetToReplay => _targetToReplay;

    public ResourceInfo Resources { get; private set; }

    public bool IsAutoPlay { get; private set; }

    public int PlayIndex { get; private set; }

    public int PlayCount { get; private set; }

    public PileType ResultPile { get; private set; }

    public override LocString Title
    {
        get
        {
            if (_cardToReplay != null)
            {
                return _cardToReplay.TitleLocString;
            }

            return base.Title;
        }
    }

    public override LocString Description
    {
        get
        {
            LocString description = new LocString("powers", "NEXT_TURN_DEFERRED_CARD.description");
            description.Add("CardEffect", _cardDescription);
            return description;
        }
    }

    public DeferredCardPlayPower InitializeFrom(CardModel card, CardPlay cardPlay, string cardDescription)
    {
        AssertMutable();

        _cardToReplay = card;
        _targetToReplay = cardPlay.Target;
        _cardDescription = cardDescription;
        Resources = cardPlay.Resources;
        IsAutoPlay = cardPlay.IsAutoPlay;
        PlayIndex = cardPlay.PlayIndex;
        PlayCount = cardPlay.PlayCount;
        ResultPile = cardPlay.ResultPile;

        return this;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || base.AmountOnTurnStart == 0)
        {
            return;
        }

        Flash();
        await DeferredCardPlayRuntime.ExecuteDeferredCardPlay(choiceContext, this);
        await PowerCmd.Remove(this);
    }
}
