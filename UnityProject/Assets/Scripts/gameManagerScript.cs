using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/**
 * @memo 2022
 * Game manager script
 */
public class gameManagerScript : NetworkBehaviour
{
    public GameObject[] gamePlayers;
    public static gameManagerScript instance;
    /**
* @memo 2022
* Create instance of this script
*/
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    [SyncVar (hook = nameof(turnChange))]
    public int turn = 0;
    [SyncVar(hook = nameof(turnChange))]
    public int round = 0;
    public int maxTurns = 10;
    public cardObject[] allCards;

    [SyncVar(hook = nameof(onShopChanged))]
    public bool areShopsEnabled=true;
    [SyncVar ]
    public bool areTrapsEnabled = true;

    /**
 * @memo 2022
 * Whenever the turn changes over the network do this
 */
    public void turnChange(int old, int newvalue)//do this if the tunr or round changes
    {
        //print("valuechanged");
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().UpdateScoreboard();

    }
    /**
 * @memo 2022
 * Tells the players shops have been enabled or disbales=d on the map
 */
    [ClientRpc]
    public void onShopChanged(bool oldVal, bool newVal)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().enableDisableShopsLocal(newVal);
    }
    /**
 * @memo 2022
 * receives that a new player just joined and adds it to a global list on every client
 */
    [ClientRpc]
    public void addPlayer()
    {
        gamePlayers = GameObject.FindGameObjectsWithTag("Player");
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().CreateScoreBoard();  
    }
    /**
 * @memo 2022
 * moves all players in the network
 */

    [ClientRpc]
    public void allPlayersMove(int tiles)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().move(tiles, false);
    }
    /**
 * @memo 2022
 * moves all players but the one sending the call back
 */
    [ClientRpc]
    public void allPlayersMoveBack(int tiles, uint id)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().callMoveBack(tiles, id);
    }
    /**
 * @memo 2022
 * swaps decks between players (not implemented)
 */

    public void swapDecks(Controller player1)
    {
        if (gamePlayers.Length <=1) { print("only one player abort!");return; }
        int rand = 0;
        uint conn = player1.netId;
        uint otherPlayerID = conn;
        while (otherPlayerID == conn)
        {
            rand = Random.Range(0, gamePlayers.Length);
            otherPlayerID = gamePlayers[rand].GetComponent<Controller>().netId;
        }
        Controller player2 = gamePlayers[rand].GetComponent<Controller>();
        List<int> deckA = getSyncDeck(player1);
        List<int> deckB = getSyncDeck(player2);
        foreach(int temp in deckA)
        {
            print("Got Card from player 1 deck"+ temp);
        }
        foreach (int temp in deckB)
        {
            print("Got Card from player 2 deck" + temp);
        }

        setP1Deck(player1.connectionToClient, player2);
        setP1Deck(player2.connectionToClient, player1);

    }
    /**
 * @memo 2022
 * Targets one connection and swaps cards
 */
    [TargetRpc]//i target player 1 
    public void setP1Deck(NetworkConnection target, Controller oppController)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().setDeckToSpecific(oppController);
    }
    /**
 * @memo 2022
 * Tells the clients to disable black holes locally
 */
    [ClientRpc]
    public void disableBlackholes(bool val)
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().enableDisableBlackHolesLocal(val);
    }
    /**
     * @memo 2022
     * Gets card by card ID
     */
    public cardObject getCardByID(int cardID)
    {
        foreach(cardObject tempCard in allCards)
        {
            if (tempCard.cardNumber == cardID)
            {
                return tempCard;
            }
        }
        return null;
    }
    /**
 * @memo 2022
 * Gets the sync deck from teh player
 */
    public List<int> getSyncDeck(Controller playerController)
    {
        List<int> tempList=new List<int>();
        foreach(int intTemp in playerController.syncDeck)
        {
            tempList.Add(intTemp);
        }
        return tempList;
    }
    /**
* @memo 2022
* Tells everyone on the net that someone won
*/
    [ClientRpc]
    public void playerWon()
    {
        NetworkClient.localPlayer.gameObject.GetComponent<Controller>().calledWin();
    }
    /**
* @memo 2022
* gets the players name for the round
*/
    public string getRoundName()
    {
        foreach(GameObject temp in gamePlayers)
        {
            if(turn== temp.GetComponent<Controller>().turn)
            {
                return temp.GetComponent<Controller>().steamName;
            }
            
        }
        return null;
    }
}

