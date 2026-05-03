using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BetManager : MonoBehaviour
{
    public TMP_Text potChips;

    public Player playerScript;
    public Opponent opponentScript;

    public enum Bettor
    {
        Player = 0,
        Opponent = 1
    }

    int nRound = 1;
    int roundMultyPlayer = 10;

    int pot = 0;
    int highRoller = 0;

    int smallBlind = 1;
    int bigBlind = 2;

    int playerBet = 0;
    int opponentBet = 0;

    bool blinType = false;

    public int GetPot()
    {
        return pot;
    }

    public int MakeBlinds(Bettor type)
    {
        int blind = GetBlindType();

        if (type == Bettor.Player)
        {
            pot += blind;
            playerBet += blind;
            highRoller = playerBet > highRoller ? playerBet : highRoller;
            return blind;
        }

        pot += blind;
        opponentBet += blind;
        highRoller = opponentBet > highRoller ? opponentBet : highRoller;
        return blind;
    }

    int GetBlindType()
    {
        if (!blinType)
        {
            blinType = !blinType;
            return smallBlind;
        }

        blinType = !blinType;
        return bigBlind;
    }

    public void ReverseBlind()
    {
        blinType = !blinType;
    }

    public void GivePot(Bettor winner)
    {
        int currentPot = pot;
        pot = 0;  // Обнуляем банк после выдачи

        if (winner == Bettor.Player)
        {
            playerScript.AddPlayerChips(currentPot);
        }
        else
        {
            opponentScript.AddOpponentChips(currentPot);
        }
    }

    public void CallPot(int chips, Bettor type)
    {
        pot += chips;

        if (type == Bettor.Player)
        {
            playerBet += chips;
        }
        else
        {
            opponentBet += chips;
        }
    }

    public void RaisePot(int chips, Bettor type)
    {
        pot += chips;

        if (type == Bettor.Player)
        {
            playerBet += chips;
            highRoller = playerBet > highRoller ? playerBet : highRoller;
        }
        else
        {
            opponentBet += chips;
            highRoller = opponentBet > highRoller ? opponentBet : highRoller;
        }
    }

    public int GetLastBet(Bettor type)
    {
        if (type == Bettor.Player)
        {
            return highRoller - playerBet;
        }

        return highRoller - opponentBet;
    }

    public void UpdateTextPot()
    {
        potChips.text = pot.ToString();
    }

    void DoubleBets()
    {
        float tmp = (float)nRound / (float)roundMultyPlayer;

        if (tmp % 1 == 0)
        {
            smallBlind *= 2;
            bigBlind *= 2;
        }
    }

    public void IncreaseRound()
    {
        nRound++;
        DoubleBets();
    }

    public void RestartPot()
    {
        pot = 0;
        playerBet = 0;
        opponentBet = 0;
        highRoller = 0; 
    }
}
