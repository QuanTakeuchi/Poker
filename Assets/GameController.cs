using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;


public class CardData
{
    public string Suite { get; set; }
    public int Number { get; set; }

    public CardData(string s, int n)
    {
        Suite = s;
        Number = n;
    }
}


public class GameController : MonoBehaviour
{

    private GameObject []player1Cards;
    private string[] cardList;
    private List<int> nextDealIndices;
    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    List<List<CardData>> playerCardsList;


    void DealCards(int nPlayers)
    {
        // Validate nPlayers
        if(nPlayers<1)
        {
            // TODO : Reload nPlayers choice selection
            logger.Error("Atleast 1 player should be selected. Current choice : " + nPlayers);
            return;
        }

        playerCardsList = new List<List<CardData>>();
        // Dealing cards to player 1 
        Vector3 playerPos = new Vector3(-0.4f, -1.4f, 0.0f);

        player1Cards = new GameObject[2];

        System.Random random = new System.Random();
        int randIndex;

        string cardName;
        string suite;
        int number;
        List<CardData> dealtCards = new List<CardData>();

        for (int c = 0; c < 2; c++)
        {
            randIndex = random.Next(0, nextDealIndices.Count);

            // Card 1
            cardName = Path.Combine("Prefabs", Path.GetFileNameWithoutExtension(cardList[randIndex]));
            nextDealIndices.RemoveAt(randIndex);

            player1Cards[c] = Resources.Load(cardName) as GameObject;
            GameObject.Instantiate(player1Cards[c], playerPos, transform.rotation);
            // Update CardData

            if (cardName.Contains("clover"))
            {
                suite = "clover";
            }
            else if (cardName.Contains("diamond"))
            {
                suite = "diamond";
            }
            else if (cardName.Contains("heart"))
            {
                suite = "heart";
            }
            else if (cardName.Contains("spade"))
            {
                suite = "spade";
            }
            else
            {
                suite = null;
                logger.Error("Prefab folder infected. Please remove extra files.");
                throw new Exception("Runtime Error");
            }
            var intString = System.Text.RegularExpressions.Regex.Match(cardName, @"\d+").Value;
            number = Int32.Parse(intString);

            CardData cardData = new CardData(suite, number);

            dealtCards.Add(cardData);

            // Update playerPos
            playerPos.x += 0.7f;
        }

        playerCardsList.Add(dealtCards);

        // Update information about cards dealt to other players
        for(int i=1; i<nPlayers; i++)
        {
            List<CardData> otherDealtCards = new List<CardData>();
            for (int c = 0; c < 2; c++)
            {
                randIndex = random.Next(0, nextDealIndices.Count);
                nextDealIndices.RemoveAt(randIndex);
                cardName = cardList[randIndex];
                
                if (cardName.Contains("clover"))
                {
                    suite = "clover";
                }
                else if (cardName.Contains("diamond"))
                {
                    suite = "diamond";
                }
                else if (cardName.Contains("heart"))
                {
                    suite = "heart";
                }
                else if (cardName.Contains("spade"))
                {
                    suite = "spade";
                }
                else
                {
                    suite = null;
                    logger.Error("Prefab folder infected. Please remove extra files.");
                    throw new Exception("Runtime Error");
                }
                var intString = System.Text.RegularExpressions.Regex.Match(cardName, @"\d+").Value;
                number = Int32.Parse(intString);

                CardData cardData = new CardData(suite, number);
                otherDealtCards.Add(cardData);
            }
            playerCardsList.Add(otherDealtCards);
        }
        
    }

    void DealFlop()
    {

    }


    private void Init()
    {
        try
        {
            cardList = Directory.GetFiles("Assets/Resources/Prefabs").Where(name => !name.Contains(".meta")).ToArray();
            nextDealIndices = new List<int>(52);

            for (int i=0; i<52; i++)
            {
                nextDealIndices.Add(i);
            }
            DealCards(1);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in Init");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
