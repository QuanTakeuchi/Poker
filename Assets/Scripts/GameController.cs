using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

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
    public Button checkButton;
    public  TextMeshProUGUI textMesh;
    private GameObject []player1Cards;
    private GameObject[] boardCards;

    private string[] cardList;
    private List<int> nextDealIndices;
    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    public List<List<CardData>> playerCardsList { get; set; }
    public List<CardData> boardCardData { get; set; }

    private Vector3 cardPos;

    private int gameState; // 0 - Preflop, 1 - Flop, 2 - Turn, 3 - River
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

        List<CardData> dealtCards = new List<CardData>();

        for (int c = 0; c < 2; c++)
        {
            randIndex = random.Next(0, nextDealIndices.Count);

            // Card 1
            cardName = Path.Combine("Prefabs", Path.GetFileNameWithoutExtension(cardList[nextDealIndices[randIndex]]));
            nextDealIndices.RemoveAt(randIndex);

            player1Cards[c] = Resources.Load(cardName) as GameObject;
            GameObject.Instantiate(player1Cards[c], playerPos, transform.rotation);
            // Update CardData

            dealtCards.Add(getCardData(cardName));

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
                cardName = cardList[nextDealIndices[randIndex]];
                
                otherDealtCards.Add(getCardData(cardName));
            }
            playerCardsList.Add(otherDealtCards);
        }
        
    }

    CardData getCardData(string cardName)
    {
        string suite;
        int number;

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

        return new CardData(suite, number);
    }

    void DealFlop()
    {
        DealBoardCards(0, 3);
    }

    void DealTurn()
    {
        DealBoardCards(3, 4);
    }

    void DealRiver()
    {
        DealBoardCards(4, 5);
    }
    void DealBoardCards(int startIndex, int endIndex)
    {
        System.Random random = new System.Random();
        int randIndex;

        string cardName;

        for (int c = startIndex; c < endIndex; c++)
        {
            randIndex = random.Next(0, nextDealIndices.Count);

            // Card 1
            cardName = Path.Combine("Prefabs", Path.GetFileNameWithoutExtension(cardList[nextDealIndices[randIndex]]));
            nextDealIndices.RemoveAt(randIndex);

            boardCards[c] = Resources.Load(cardName) as GameObject;
            GameObject.Instantiate(boardCards[c], cardPos, transform.rotation);
            // Update CardData

            boardCardData.Add(getCardData(cardName));

            // Update playerPos
            cardPos.x += 0.7f;
        }
    }
    private void Init()
    {
        try
        {
            cardList = Directory.GetFiles("Assets/Resources/Prefabs").Where(name => 
                                                                            !name.Contains(".meta")).ToArray();
            nextDealIndices = new List<int>(52);
            gameState = 0;

            // Update button functions
            checkButton.onClick.AddListener(checkButtonClick);


            for (int i=0; i<52; i++)
            {
                nextDealIndices.Add(i);
            }

            cardPos = new Vector3(-1.4f, 0.0f, 0.0f);
            boardCards = new GameObject[5];
            boardCardData = new List<CardData>();


        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error in Init");
        }
    }

    void checkButtonClick()
    {
        List<CardData> playerHand = new List<CardData>();
  
        switch (gameState)
        {
            case 0:
                DealFlop();
                gameState++;
                break;
            case 1:
                DealTurn();
                gameState++;
                break;
            case 2:
                DealRiver();
                gameState++;
                break;
            case 3:
                checkButton.interactable = false;
                gameState++;
                break;
            default:

                logger.Error("Incorrect GameState");
                break;
        }
        foreach (CardData c in playerCardsList[0])
        {
            playerHand.Add(c);
        }
        foreach (CardData c in boardCardData)
        {
            playerHand.Add(c);
        }

        PokerAI.HandStrength handStrength = PokerAI.BestHand(playerHand);
        textMesh.text = PokerAI.HandTotext(handStrength);
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
        DealCards(3);
        //DealFlop();
        //DealTurn();
        //DealRiver();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
