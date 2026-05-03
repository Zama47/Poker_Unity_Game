using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [Header("Buttons References")]
    public GameObject startButton;
    public GameObject endButton;
    public GameObject betButton;

    [Header("Text References")]
    public TMP_Text textWinner;
    public TMP_Text textPlayerAHand;
    public TMP_Text textPlayerBHand;
    public TMP_Text actionLogText;

    [Header("Script References")]
    public BetManager betManager;
    public RoundManager roundManager;
    public Opponent opponent;
    public DeckAnimator deckAnimator;

    public GameState currentState;

    public enum GameState
    {
        Start = 0,
        ShowPlayerCards = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        EndOfRound = 5,
        PlayerFold = 6,
    }


    [Header("Images & Sprites References")]
    public Sprite backSprite;
    public Sprite[] cardSprites;

    public Image PlayerACard1;
    public Image PlayerACard2;

    public Image PlayerBCard1;
    public Image PlayerBCard2;

    public Image[] communitySprites;

    private List<string> deck = new List<string>();
    private List<string> playerCards = new List<string>();
    private List<string> opponentCards = new List<string>();
    private List<string> communityCards = new List<string>();
    private Dictionary<string, int> cardToIndex = new Dictionary<string, int>();

    public void ChangeToNextState()
    {
        switch (currentState)
        {
            case GameState.Start:
                ShowCards();
                ActiveBetButtons();
                currentState++;
                break;
            case GameState.ShowPlayerCards:
                StartFlop();
                currentState++;
                break;
            case GameState.Flop:
                StartTurn();
                currentState++;
                break;
            case GameState.Turn:
                StartRiver();
                currentState++;
                break;
            case GameState.River:
                StartEndOfRound();
                ShowOpponentCards();
                EndOfRoundButton();
                currentState++;
                break;
            case GameState.EndOfRound:
                Restart();
                betManager.IncreaseRound();
                currentState = 0;
                break;
            case GameState.PlayerFold:
                Restart();
                currentState = 0;
                break;
        }
    }

    public void CreateDeck()
    {
        List<string> suits = new List<string>() { "D", "H", "C", "S" };
        List<string> ranks = new List<string>()
        {
            "A", "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K"
        };

        cardToIndex.Clear();
        deck.Clear();
        playerCards.Clear();
        communityCards.Clear();
        opponentCards.Clear();

        int currentIndex = 0;

        foreach (var currentSuit in suits)
        {
            foreach (var currentRank in ranks)
            {
                deck.Add($"{currentRank}{currentSuit}");
                cardToIndex.Add($"{currentRank}{currentSuit}", currentIndex);

                currentIndex += 1;
            }
        }


        List<string> shuffleDeck = deck.OrderBy(item => Random.Range(0, deck.Count)).ToList();

        playerCards.AddRange(shuffleDeck.Take(2));
        shuffleDeck.RemoveRange(index: 0, count: 2);

        opponentCards.AddRange(shuffleDeck.Take(2));
        shuffleDeck.RemoveRange(index: 0, count: 2);

        communityCards.AddRange(shuffleDeck.Take(5));

        if (opponent != null)
        {
            opponent.SetCards(opponentCards);
            opponent.UpdateCommunityCards(new List<string>());
        }
        shuffleDeck.RemoveRange(index: 0, count: 5);

    }

    void ShowCards()
    {
        if (deckAnimator != null)
        {
            deckAnimator.StartDealAnimation();
        }
        else
        {
            PlayerACard1.sprite = cardSprites[cardToIndex[playerCards[0]]];
            PlayerACard2.sprite = cardSprites[cardToIndex[playerCards[1]]];
        }
    }

    void ShowOpponentCards()
    {
        if (deckAnimator != null)
        {
            deckAnimator.FlipOpponentCards(
                cardSprites[cardToIndex[opponentCards[0]]],
                cardSprites[cardToIndex[opponentCards[1]]]
            );
        }
        else
        {
            PlayerBCard1.sprite = cardSprites[cardToIndex[opponentCards[0]]];
            PlayerBCard2.sprite = cardSprites[cardToIndex[opponentCards[1]]];
        }
    }

    void StartFlop()
    {
        if (deckAnimator != null)
        {
            deckAnimator.DealCommunityCards(3);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                communitySprites[i].sprite = cardSprites[cardToIndex[communityCards[i]]];
            }
        }
        if (opponent != null)
            opponent.UpdateCommunityCards(communityCards.GetRange(0, 3));
    }

    void StartTurn()
    {
        if (deckAnimator != null)
        {
            deckAnimator.DealCommunityCards(4);
        }
        else
        {
            communitySprites[3].sprite = cardSprites[cardToIndex[communityCards[3]]];
        }
        if (opponent != null)
            opponent.UpdateCommunityCards(communityCards.GetRange(0, 4));
    }

    void StartRiver()
    {
        if (deckAnimator != null)
        {
            deckAnimator.DealCommunityCards(5);
        }
        else
        {
            communitySprites[4].sprite = cardSprites[cardToIndex[communityCards[4]]];
        }
        if (opponent != null)
            opponent.UpdateCommunityCards(communityCards);
    }

    void HideAllCards()
    {
        PlayerACard1.sprite = backSprite;
        PlayerACard2.sprite = backSprite;
        PlayerBCard1.sprite = backSprite;
        PlayerBCard2.sprite = backSprite;

        for (int i = 0; i < 5; i++)
        {
            communitySprites[i].sprite = backSprite;
        }
    }

    public void EndGame()
    {
        DisableAllButtons();
        StartCoroutine(EndGameWithDelay());
    }

    IEnumerator EndGameWithDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }

    PokerHands.Results results;

    private void StartEndOfRound()
    {
        results = PokerHands.GetWinner(playerCards, opponentCards, communityCards);

        textWinner.text = $"Winner: {results.winner}";
        textPlayerAHand.text = $"{results.playerHandRanking}";
        textPlayerBHand.text = $"{results.opponentHandRanking}";
        roundManager.EndRound(results);
    }

    public void PlayerFold()
    {
        currentState = GameState.PlayerFold;
        betManager.GivePot(BetManager.Bettor.Opponent);
        results.winner = PokerHands.Winner.Opponent;
    }

    public void OpponentFold()
    {
        currentState = GameState.PlayerFold;
        betManager.GivePot(BetManager.Bettor.Player);
        results.winner = PokerHands.Winner.Player;
        textWinner.text = "Winner: Player";
        LogAction("Opponent FOLDED");
    }

    public void LogAction(string message)
    {
        if (actionLogText != null)
        {
            actionLogText.text = message;
        }
    }

    void Restart()
    {
        deckAnimator?.ResetAllCards();
        HideAllCards();
        InitializeButtons();
        CreateDeck();
        betManager.RestartPot();
        textPlayerAHand.text = "";
        textPlayerBHand.text = "";
        textWinner.text = "";
        LogAction("");
    }

    void InitializeButtons()
    {
        startButton.SetActive(true);
        betButton.SetActive(false);
        endButton.SetActive(false);
    }

    void ActiveBetButtons()
    {
        startButton.SetActive(false);
        betButton.SetActive(true);
        endButton.SetActive(false);
    }

    void EndOfRoundButton()
    {
        startButton.SetActive(false);
        betButton.SetActive(false);
        endButton.SetActive(true);
    }

    void DisableAllButtons()
    {
        startButton.SetActive(false);
        betButton.SetActive(false);
        endButton.SetActive(false);
    }

    void Start()
    {
        var seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        CreateDeck();
        ShowCards();

        InitializeButtons();

        currentState = GameState.Start;
        HideAllCards();
    }

    void Update()
    {
        
    }
}
