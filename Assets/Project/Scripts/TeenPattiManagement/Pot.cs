
using System;
using UnityEngine;

[Serializable]
public class Pot
{
    public int totalPot;
    public static int currentBet;
    public static int currentBlindBet;

    public Pot()
    {
        totalPot = 0;
        currentBet = 0;
    }

    public void AddToPot(int chips)
    {
        totalPot += chips;
    }

    public bool MakeBet(int bet)
    {
        if (bet < currentBet)
        {
            return false;
        }

        AddToPot(bet);
        currentBet = bet;
        return true;
    }

    public void ResetPot()
    {
        totalPot = 0;
    }
}

public static class Chips
{
    public const int Ante = 10;
    public const int Call = 20;
}
