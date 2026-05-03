using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private int playerChips = 10;
    int raiseAmount = 1;

    public TMP_Text playerChipsText;
    public TMP_Text raiseAmountText;
    public Slider betSlider;

    public BetManager betManager;

    public void InitialBlind()
    {
        int blind = betManager.MakeBlinds(BetManager.Bettor.Player);
        playerChips -= blind;
    }

    public void CallBet()
    {
        int bet = betManager.GetLastBet(BetManager.Bettor.Player);

        if (playerChips >= bet)
        {
            betManager.CallPot(bet, BetManager.Bettor.Player);
            playerChips -= bet;
        }
        else if (playerChips > 0)
        {
            AllIn();
        }
    }

    public void AllIn()
    {
        if (playerChips > 0)
        {
            betManager.RaisePot(playerChips, BetManager.Bettor.Player);
            playerChips = 0;
        }
    }

    public void RaiseBet()
    {
        int bet = betManager.GetLastBet(BetManager.Bettor.Player);
        bet += raiseAmount;

        if (playerChips >= bet)
        {
            betManager.RaisePot(bet, BetManager.Bettor.Player);
            playerChips -= bet;
        }
    }

    public void SetRaiseAmount(float value)
    {
        raiseAmount = Mathf.RoundToInt(value);
        if (raiseAmountText != null)
        {
            raiseAmountText.text = $"Raise: {raiseAmount}";
        }
    }

    public void UpdateSliderMaxValue()
    {
        if (betSlider != null)
        {
            betSlider.maxValue = playerChips;
            betSlider.value = Mathf.Min(raiseAmount, playerChips);
        }
    }

    public bool isPlayerBroke()
    {
        bool broke = playerChips <= 0 ? true : false;
        return broke;
    }

    public void UpdateText()
    {
        playerChipsText.text = playerChips.ToString();
        UpdateSliderMaxValue();
    }

    public void AddPlayerChips(int chips)
    {
        playerChips += chips;
    }

    void ClampChips()
    {
        if (playerChips < 0) playerChips = 0;
    }
}
