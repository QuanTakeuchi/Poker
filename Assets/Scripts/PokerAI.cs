using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerAI : MonoBehaviour
{
    public enum HandStrenth  { ROYALFLUSH, STRAIGHTFLUSH, FOURKIND, FULLHOUSE, FLUSH, 
                               STRAIGHT, THREEKIND, TWOPAIR, PAIR, HIGH };

    public List<HandStrenth> playerHands { get; set; }

    public string handTotext(HandStrenth hs)
    {
        switch(hs)
        {
            case HandStrenth.ROYALFLUSH:
                return "Royal Flush";
            case HandStrenth.STRAIGHTFLUSH:
                return "Straight Flush";
            case HandStrenth.FOURKIND:
                return "Four of a Kind";
            case HandStrenth.FULLHOUSE:
                return "Full House";
            case HandStrenth.FLUSH:
                return "Flush";
            case HandStrenth.STRAIGHT:
                return "Straight";
            case HandStrenth.THREEKIND:
                return "Three of a kind";
            case HandStrenth.TWOPAIR:
                return "Two Pair";
            case HandStrenth.PAIR:
                return "One Pair";
            case HandStrenth.HIGH:
                return "High Card";
            default:
                throw new System.Exception("Incorrect HandStrength index");
        }
    }
}
