using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [Header("Scripts References")]
    public Player player;
    public Opponent opponent;
    public DeckManager deckManager;
    public BetManager betManager;

    public void StartRound()
    {
        opponent.deckManager = deckManager;
        opponent.StartNewRound();

        player.InitialBlind();
        player.UpdateText();

        opponent.InitialBlind();
        opponent.UpdateText();

        betManager.UpdateTextPot();

        // Check if any player is all-in (0 chips after blinds)
        if (player.isPlayerBroke() || opponent.isOpponentBroke())
        {
            StartCoroutine(AutoPlayAllCards());
            return;
        }

        NextState();
    }

    System.Collections.IEnumerator AutoPlayAllCards()
    {
        // Show player cards
        yield return new WaitForSeconds(0.5f);
        NextState(); // Start -> ShowPlayerCards

        // Show Flop (3 cards)
        yield return new WaitForSeconds(0.8f);
        NextState(); // ShowPlayerCards -> Flop

        // Show Turn (4th card)
        yield return new WaitForSeconds(0.8f);
        NextState(); // Flop -> Turn

        // Show River (5th card)
        yield return new WaitForSeconds(0.8f);
        NextState(); // Turn -> River

        // End of round
        yield return new WaitForSeconds(0.5f);
        NextState(); // River -> EndOfRound
    }

    public void CallAction()
    {
        player.CallBet();
        player.UpdateText();

        int currentBet = betManager.GetLastBet(BetManager.Bettor.Opponent);
        int potSize = betManager.GetPot();
        Opponent.AIAction aiAction = opponent.MakeDecision(currentBet, potSize);

        if (aiAction == Opponent.AIAction.Fold)
        {
            deckManager.OpponentFold();
            betManager.UpdateTextPot();
            player.UpdateText();
            opponent.UpdateText();
            StartCoroutine(RestartAfterDelay());
            return;
        }

        opponent.ExecuteAction(aiAction);
        opponent.UpdateText();
        betManager.UpdateTextPot();

        int newBet = betManager.GetLastBet(BetManager.Bettor.Player);
        if (newBet > 0)
        {
            return;
        }

        NextState();
    }

    public void RaiseAction()
    {
        player.RaiseBet();
        player.UpdateText();
        betManager.UpdateTextPot();

        int currentBet = betManager.GetLastBet(BetManager.Bettor.Opponent);
        int potSize = betManager.GetPot();

        Opponent.AIAction aiAction = opponent.MakeDecision(currentBet, potSize);

        if (aiAction == Opponent.AIAction.Fold)
        {
            deckManager.OpponentFold();
            betManager.UpdateTextPot();
            player.UpdateText();
            opponent.UpdateText();
            StartCoroutine(RestartAfterDelay());
            return;
        }

        opponent.ExecuteAction(aiAction);
        opponent.UpdateText();
        betManager.UpdateTextPot();

        int newBet = betManager.GetLastBet(BetManager.Bettor.Player);
        if (newBet > 0)
        {
            return;
        }

        NextState();
    }

    public void FoldAction()
    {
        deckManager.PlayerFold();
        betManager.RestartPot();
        betManager.UpdateTextPot();
        opponent.UpdateText();

        NextState();
    }

    public void EndRound(PokerHands.Results winner)
    {
        GivePot(winner);
        player.UpdateText();
        opponent.UpdateText();
        betManager.UpdateTextPot();
        betManager.RestartPot();

        if (player.isPlayerBroke() || opponent.isOpponentBroke())
        {
            deckManager.EndGame();
        }
    }

    void GivePot(PokerHands.Results winner)
    {
        int pot = betManager.GetPot();

        if (winner.winner == 0)
        {
            betManager.GivePot(BetManager.Bettor.Player);
            return;
        }

        betManager.GivePot(BetManager.Bettor.Opponent);
    }

    void NextState()
    {
        deckManager.ChangeToNextState();
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        NextState();
    }
}
