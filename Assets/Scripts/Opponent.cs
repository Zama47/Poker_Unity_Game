using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Opponent : MonoBehaviour
{
    private int opponentChips = 10;
    int raiseAmount = 1;

    public TMP_Text opponentChipsText;

    public BetManager betManager;
    public DeckManager deckManager;

    private List<string> opponentCards = new List<string>();
    private List<string> communityCards = new List<string>();

    [Header("AI Personality")]
    [Range(0f, 1f)]
    public float aggressiveness = 0.6f;
    [Range(0f, 1f)]
    public float bluffFrequency = 0.2f;
    [Range(0f, 1f)]
    public float riskTolerance = 0.5f;

    private bool isBluffingThisRound = false;
    private int currentRound = 0;

    public enum AIAction
    {
        Fold,
        Call,
        Raise,
        AllIn
    }

    public void SetCards(List<string> cards)
    {
        opponentCards = new List<string>(cards);
    }

    public void UpdateCommunityCards(List<string> cards)
    {
        communityCards = new List<string>(cards);
    }

    public void InitialBlind()
    {
        int blind = betManager.MakeBlinds(BetManager.Bettor.Opponent);
        opponentChips -= blind;
    }

    public void CallBet()
    {
        int bet = betManager.GetLastBet(BetManager.Bettor.Opponent);

        if (opponentChips >= bet)
        {
            betManager.CallPot(bet, BetManager.Bettor.Opponent);
            opponentChips -= bet;
        }
        else if (opponentChips > 0)
        {
            AllIn();
        }
    }

    public void RaiseBet()
    {
        int bet = betManager.GetLastBet(BetManager.Bettor.Opponent);
        bet += raiseAmount;

        if (opponentChips >= bet)
        {
            betManager.RaisePot(bet, BetManager.Bettor.Opponent);
            opponentChips -= bet;
        }
    }

    public void RaiseBy(int amount)
    {
        int baseBet = betManager.GetLastBet(BetManager.Bettor.Opponent);
        int totalBet = baseBet + amount;

        if (opponentChips >= totalBet)
        {
            betManager.RaisePot(totalBet, BetManager.Bettor.Opponent);
            opponentChips -= totalBet;
        }
        else if (opponentChips > 0)
        {
            AllIn();
        }
    }

    public void AllIn()
    {
        if (opponentChips > 0)
        {
            betManager.RaisePot(opponentChips, BetManager.Bettor.Opponent);
            opponentChips = 0;
        }
    }

    void ClampChips()
    {
        if (opponentChips < 0) opponentChips = 0;
    }

    public bool isOpponentBroke()
    {
        bool broke = opponentChips <= 0 ? true : false;
        return broke;
    }

    public void UpdateText()
    {
        opponentChipsText.text = opponentChips.ToString();
    }

    public void AddOpponentChips(int chips)
    {
        opponentChips += chips;
    }

    public void StartNewRound()
    {
        currentRound++;
        isBluffingThisRound = Random.value < bluffFrequency;

        if (deckManager != null)
        {
            UpdateCardsFromDeck();
        }
    }

    private void UpdateCardsFromDeck()
    {
        var deckField = deckManager.GetType().GetField("opponentCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (deckField != null)
        {
            opponentCards = (List<string>)deckField.GetValue(deckManager);
        }

        var communityField = deckManager.GetType().GetField("communityCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (communityField != null)
        {
            communityCards = (List<string>)communityField.GetValue(deckManager);
        }
    }

    public PokerHands.HandRanking EvaluateHandStrength()
    {
        if (opponentCards == null || opponentCards.Count < 2)
            return PokerHands.HandRanking.HighCard;

        var availableCards = opponentCards.Concat(communityCards).ToList();
        if (availableCards.Count < 5)
            return EstimatePreflopStrength();

        var bestHand = GetBestHandFromAvailableCards(availableCards);
        return bestHand.HandRanking;
    }

    private PokerHands.HandRanking EstimatePreflopStrength()
    {
        if (opponentCards.Count < 2)
            return PokerHands.HandRanking.HighCard;

        string card1 = opponentCards[0];
        string card2 = opponentCards[1];
        int rank1 = PokerHands.CardRanks[card1[0]];
        int rank2 = PokerHands.CardRanks[card2[0]];
        char suit1 = card1[1];
        char suit2 = card2[1];

        bool isPair = card1[0] == card2[0];
        bool isSuited = suit1 == suit2;
        bool isConnected = Mathf.Abs(rank1 - rank2) == 1;
        int highCard = Mathf.Max(rank1, rank2);

        if (isPair)
        {
            if (rank1 >= 10) return PokerHands.HandRanking.OnePair;
            return PokerHands.HandRanking.HighCard;
        }

        if (isSuited && isConnected && highCard >= 10)
        {
            return PokerHands.HandRanking.Straight;
        }

        if ((highCard >= 13 && (rank1 + rank2 >= 23)) || (isSuited && highCard >= 12))
        {
            return PokerHands.HandRanking.HighCard;
        }

        return PokerHands.HandRanking.Invalid;
    }

    private PokerHands.Rankedhand GetBestHandFromAvailableCards(List<string> availableCards)
    {
        int n = availableCards.Count;
        int k = 5;

        var combinations = CreateCombinations(n, k);
        PokerHands.Rankedhand bestHand = new PokerHands.Rankedhand { HandRanking = PokerHands.HandRanking.Invalid, SortedRanks = new List<int>() };

        foreach (var combo in combinations)
        {
            string handString = "";
            foreach (int idx in combo)
            {
                handString += $"{availableCards[idx - 1]} ";
            }
            handString = handString.Trim();

            PokerHands.Rankedhand currentHand = PokerHands.GetHandRanking(handString);
            if ((int)currentHand.HandRanking > (int)bestHand.HandRanking)
            {
                bestHand = currentHand;
            }
            else if ((int)currentHand.HandRanking == (int)bestHand.HandRanking)
            {
                if (PokerHands.CompareLists(currentHand.SortedRanks, bestHand.SortedRanks) > 0)
                {
                    bestHand = currentHand;
                }
            }
        }

        return bestHand;
    }

    private List<List<int>> CreateCombinations(int n, int k)
    {
        var result = new List<List<int>>();
        var combination = new int[k];

        void Generate(int start, int depth)
        {
            if (depth == k)
            {
                result.Add(new List<int>(combination));
                return;
            }

            for (int i = start; i <= n - k + depth + 1; i++)
            {
                combination[depth] = i;
                Generate(i + 1, depth + 1);
            }
        }

        Generate(1, 0);
        return result;
    }

    public AIAction MakeDecision(int currentBet, int potSize)
    {
        UpdateCardsFromDeck();

        PokerHands.HandRanking handStrength = EvaluateHandStrength();
        int handValue = (int)handStrength;

        float potOdds = currentBet > 0 ? (float)potSize / currentBet : 0;

        float adjustedAggressiveness = aggressiveness;
        if (opponentChips < 5) adjustedAggressiveness += 0.3f;
        if (opponentChips > 20) adjustedAggressiveness -= 0.1f;

        int betToCall = betManager.GetLastBet(BetManager.Bettor.Opponent);

        if (isBluffingThisRound)
        {
            float bluffRaiseChance = adjustedAggressiveness * 0.7f;
            if (Random.value < bluffRaiseChance && opponentChips >= betToCall + raiseAmount * 2)
            {
                return AIAction.Raise;
            }
            return AIAction.Call;
        }

        if (handValue <= 1)
        {
            float foldThreshold = 0.5f + (riskTolerance * 0.3f);
            if (Random.value < foldThreshold)
            {
                return AIAction.Fold;
            }

            if (potOdds > 4.0f && opponentChips >= betToCall && currentBet <= 2)
            {
                return AIAction.Call;
            }

            return AIAction.Fold;
        }

        if (handValue >= 7)
        {
            float allInThreshold = 0.15f * adjustedAggressiveness;
            if (Random.value < allInThreshold)
            {
                return AIAction.AllIn;
            }

            if (opponentChips >= betToCall + raiseAmount * 3)
            {
                return AIAction.Raise;
            }
            return AIAction.Call;
        }

        if (handValue >= 4)
        {
            float raiseChance = adjustedAggressiveness * 0.6f;
            if (Random.value < raiseChance && opponentChips >= betToCall + raiseAmount * 2)
            {
                return AIAction.Raise;
            }
            if (opponentChips >= betToCall)
            {
                return AIAction.Call;
            }
            return AIAction.Fold;
        }

        if (handValue >= 2)
        {
            float callChance = 0.5f + (potOdds * 0.2f);
            if (Random.value < callChance && opponentChips >= betToCall)
            {
                return AIAction.Call;
            }
            return AIAction.Fold;
        }

        return AIAction.Fold;
    }

    public void ExecuteAction(AIAction action)
    {
        string actionMessage = "";
        switch (action)
        {
            case AIAction.Fold:
                actionMessage = "Opponent FOLDED";
                break;
            case AIAction.Call:
                CallBet();
                actionMessage = "Opponent CALLED";
                break;
            case AIAction.Raise:
                int raise = raiseAmount * Random.Range(1, 4);
                RaiseBy(raise);
                actionMessage = $"Opponent RAISED by {raise}";
                break;
            case AIAction.AllIn:
                AllIn();
                actionMessage = "Opponent went ALL-IN!";
                break;
        }
        if (deckManager != null)
        {
            deckManager.LogAction(actionMessage);
        }
    }

    public void AutoPlay(int currentBet, int potSize)
    {
        AIAction action = MakeDecision(currentBet, potSize);
        ExecuteAction(action);
    }
}