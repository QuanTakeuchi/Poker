using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combinatorics.Collections;
using System.Linq;
using System;
using TMPro;

public class PokerAI : MonoBehaviour
{
    public GameObject gameController;
    public enum HandStrength  { ROYALFLUSH, STRAIGHTFLUSH, FOURKIND, FULLHOUSE, FLUSH, 
                               STRAIGHT, THREEKIND, TWOPAIR, PAIR, HIGH };



    public static List<float> PlayerHandsProb { get; set; }

    public static HandStrength highestHand(List<HandStrength> handStrengths)
    {
        if(handStrengths.Count == 0)
        {
            throw new InvalidOperationException("Empty List");
        }
        HandStrength bestHand = HandStrength.HIGH;
        foreach(HandStrength h in handStrengths)
        {
            if(h<bestHand)
            {
                bestHand = h;
            }
        }
        return bestHand;
    }
    public static string HandTotext(HandStrength hs)
    {
        switch(hs)
        {
            case HandStrength.ROYALFLUSH:
                return "Royal Flush";
            case HandStrength.STRAIGHTFLUSH:
                return "Straight Flush";
            case HandStrength.FOURKIND:
                return "Four of a Kind";
            case HandStrength.FULLHOUSE:
                return "Full House";
            case HandStrength.FLUSH:
                return "Flush";
            case HandStrength.STRAIGHT:
                return "Straight";
            case HandStrength.THREEKIND:
                return "Three of a kind";
            case HandStrength.TWOPAIR:
                return "Two Pair";
            case HandStrength.PAIR:
                return "One Pair";
            case HandStrength.HIGH:
                return "High Card";
            default:
                throw new System.Exception("Incorrect HandStrength index");
        }
        
    }
    // Module to list all the possible hands and find the best
    public static HandStrength BestHand(List<CardData> playerCards)
    {
        if(playerCards.Count<5) { return HandStrength.HIGH; }
        Combinations<CardData> cardDatas = new Combinations<CardData>(playerCards, 5);
        List<HandStrength> handStrenths = new List<HandStrength>();
        foreach(IList<CardData> c in cardDatas)
        {
            // Call HandStrengthComputer for all combinations
            handStrenths.Add(HandStrengthComputer(new List<CardData>(c)));
        }
        return highestHand(handStrenths);
    }

    // Module to find the hand strength given five cards
    public static HandStrength HandStrengthComputer(List<CardData> fiveCards)
    {
        // Validations
        if (fiveCards.Count != 5)
        {
            throw new System.Exception("Incorrect number of cards to compute the hand strength");
        }

        string lastCardSuite = fiveCards[0].Suite;
        bool isFlush = true;
        bool isStraight = true;
        List<int> cardNumbers = new List<int>();

        // Get a sorted list of card numbers
        foreach(CardData card in fiveCards)
        {
            //Update Ace number
            if(card.Number==1) { card.Number = 14; }
            cardNumbers.Add(card.Number);
        }
        cardNumbers.Sort();

        // Check Flush
        foreach (CardData card in fiveCards)
        {
            if (card.Suite == lastCardSuite)
            {
                lastCardSuite = card.Suite;
                isFlush = true;
            }
            else
            {
                isFlush = false;
                break;
            }
        }

        // Check straight
        for(int i=1; i<cardNumbers.Count; i++)
        {
            if(cardNumbers[i]!=14 && cardNumbers[i]-cardNumbers[i-1]==1)
            {
                isStraight = true;
            }
            else if(cardNumbers[i]==14 && cardNumbers[i-1]==5) // A, 2, 3, 4, 5 straight
            {
                isStraight = true;
            }
            else
            {
                isStraight = false;
                break;
            }
        }

        if(isStraight && isFlush)
        {
            if(cardNumbers[0]==10) { return HandStrength.ROYALFLUSH; }
            else { return HandStrength.STRAIGHTFLUSH; }
        }
        if(isFlush) { return HandStrength.FLUSH; }
        if(isStraight) { return HandStrength.STRAIGHT; }

        // Check of-a-kind hands and full house
        List<int> matchedCards = new List<int>();
        for(int i=1; i<cardNumbers.Count; i++)
        {
            if(cardNumbers[i]-cardNumbers[i-1]==0 )
            {
                matchedCards.Add(cardNumbers[i]);
            }
        }

        if(matchedCards.Count==1) { return HandStrength.PAIR; }
        if(matchedCards.Count==2)
        {
            if(matchedCards[0]==matchedCards[1]) { return HandStrength.THREEKIND; }
            else { return HandStrength.TWOPAIR; }
        }
        if(matchedCards.Count==3)
        {
            if(matchedCards[0]==matchedCards[2]) { return HandStrength.FOURKIND; }
            else { return HandStrength.FULLHOUSE; }
        }
        return HandStrength.HIGH;
    }

    public void simulateCards(List<int> nextDealIndices, List<CardData> dealtCards, string []cardList)
    {
        GameController gC = gameController.GetComponent<GameController>();
        try
        {
            int maxSimCount = 100;

            System.Random random = new System.Random();
            int randIndex;
            string cardName;

           

            PlayerHandsProb = new List<float>() { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            for (int sim = 0; sim < maxSimCount; sim++)
            {
                List<CardData> simDealtCards = new List<CardData>(dealtCards);
                List<int> simDealIndices = new List<int>(nextDealIndices);
                for (int i = 0; i < 7 - dealtCards.Count; i++)
                {
                    randIndex = random.Next(0, simDealIndices.Count);
                    cardName = cardList[simDealIndices[randIndex]];
                    simDealIndices.RemoveAt(randIndex);

                    simDealtCards.Add(GameController.getCardData(cardName));
                }
                int bhIndex = (int)BestHand(simDealtCards);
                PlayerHandsProb[bhIndex] += 1.0f / (float)maxSimCount;
            }


            gC.RFProb.text = PlayerHandsProb[0].ToString("0.00");
            gC.SFProb.text = PlayerHandsProb[1].ToString("0.00");
            gC.FKProb.text = PlayerHandsProb[2].ToString("0.00");
            gC.FHProb.text = PlayerHandsProb[3].ToString("0.00");
            gC.FProb.text = PlayerHandsProb[4].ToString("0.00");
            gC.SProb.text = PlayerHandsProb[5].ToString("0.00");
            gC.TKProb.text = PlayerHandsProb[6].ToString("0.00");
            gC.TPProb.text = PlayerHandsProb[7].ToString("0.00");
            gC.OPProb.text = PlayerHandsProb[8].ToString("0.00");
            gC.HCProb.text = PlayerHandsProb[9].ToString("0.00");

        }
        catch(Exception ex)
        {
            GameController.logger.Error("Error at : " + ex.StackTrace);
            throw new Exception("Runtime Error");
        }
    }
}
