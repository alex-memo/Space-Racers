using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class Controller : NetworkBehaviour
{
    #region variables
    //every player must have a deck of cards
    public List<cardObject> deck = new List<cardObject>();
    public readonly SyncList<int> syncDeck=new SyncList<int>();
    private int position = 0;
    private readonly float movespeed = 1f;//move speed for moving player around
    private bool hasToMove = false;//if the player has to move
    private int noTiles =0;//number of tiles player has to move in queue
    private GameObject spaceshipFire;
    public GameObject playerUI;
    public GameObject root;

    public cardHolderScript cardHolder;
    public cardHolderScript shopHolder;
    public scoreboardScript ScoreBoard;

    public bool doublePoints = false;

    [SyncVar]
    public int turn;

    //private int cardsToGive = 3;//amount of cards to give at the start of game

    [SyncVar]
    public string steamName;
    [SyncVar]
    public int score = 0;

    private float turnTime=15;
    public float timer = 0;
    public bool isTurn = false;
    private bool isInShop = false;
    private float shopTimer = 0;
    private float shopTime = 12;

    public cardObject selectedCard;

    private bool hasToMoveBack=false;
    //public int id = 0;
    [SyncVar(hook = nameof(HandleSteamIDUpdated))]
    public ulong playerSteamID;

    private bool forcedMovement = false;

    public bool canMove = true;
    public bool trapped = false;
    public int trapPosition;
    float count = 0;
    float spinRate = 0;

    private bool hasWon = false;

    public GameObject winScreen;
    public GameObject loseScreen;

    #endregion


    /**
 * @memo 2022
 * Start Method
 */
    void Start()
    {
        transform.position = waypointsScript.instance.waypoints[position].transform.position;
        spaceshipFire=transform.GetChild(0).gameObject;
        //id = (int)GetComponent<NetworkIdentity>().netId;
        //setSteamName();
    }
    /**
     * @memo 2022
     * Update Method
     */
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (root.activeSelf == false)
            {
                root.SetActive(true);
                if (isLocalPlayer)
                {
                    playerUI.SetActive(true);
                }
                
            }
        }
        if (hasAuthority)
        {

            if (hasToMove||forcedMovement)
            {
                moveTile(1);
            }
            if (hasToMoveBack)
            {
                moveTile(-1);
            }
            if (isTurn)
            {
                //timer += Time.deltaTime;
                if (timer > turnTime)//if player is stalling the game, not playing anything in over 15 sec
                {
                    if (deck.Count > 0) { move(deck[0].cardNumber,true); }
                    else { print("nothing to play, player out!"); }
                    
                }
            }
            if (isInShop)
            {
                shopTimer += Time.deltaTime;
                if (shopTimer > shopTime)
                {
                    //close shop, nothing was bought
                    quitShop();
                }
            }
            if (selectedCard != null)
            {
                //if player has a selected card then 
                //make player click on another player on top
            }
            /**
             * @Denzil 2022
             * if statement
             */
            if (trapped)
            {
                count = count + 0.01f;
                if (count <= 5)
                {
                    spinRate = -0.025f * ((count - 22.36f) * (count - 22.36f)) + 12.5f;
                }
                else
                {
                    spinRate = -0.5f * ((count - 5) * (count - 5)) + 5;
                }
                if (spinRate <= 0)
                {
                    count = 0;
                    trapped = false;
                    canMove = true;
                    //move(0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + (spinRate));
                    transform.position -= (transform.position - waypointsScript.instance.waypoints[trapPosition].position).normalized * 0.005f;
                    transform.RotateAround(waypointsScript.instance.waypoints[trapPosition].position, new Vector3(0, 0, 1), spinRate);
                }
            }

        }
        

    }




    #region movementRegion
    /**
* @memo 2022
* calls moves the player
*/
    public void move(int tilesToMove, bool playerCalled)
    {
        isTurn = false; timer = 0;
        if (position < waypointsScript.instance.waypoints.Count - 1 && hasAuthority)//position + roll;
        {

            noTiles = tilesToMove;//number of tiles to move
            if (playerCalled)//i fplayer requested movement
            {
                hasToMove = true;
            }
            else//other player forced player to move
            {
                forcedMovement = true;
            }
            
            setPlayerRotation(1);
        }
    }
    /**
 * @memo 2022
 * moves the player
 */
    private void moveTile(int direction)//dir should be 1 forward or -1 backward
    {
        if (transform.position != waypointsScript.instance.waypoints[position + direction].transform.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypointsScript.instance.waypoints[position + direction].transform.position, movespeed * Time.deltaTime);
        }
        if (transform.position.x == waypointsScript.instance.waypoints[position + direction].transform.position.x && transform.position.y == waypointsScript.instance.waypoints[position + direction].transform.position.y)//just check x and y
        {
            position+=direction; noTiles--;
            if (position == waypointsScript.instance.waypoints.Count - 1)//if player won 
            {
                hasToMove = false; forcedMovement = false;
                hasWon = true;
                print("won game!");
                winGame();
                return; ;
            }
            else if (position== 0)//has gone so back that is to the begining
            {
                hasToMoveBack = false;  
                print("how are u this unlucky");
            }
            else if (noTiles == 0)//if there are no tiles in queue left
            {

                addScore(waypointsScript.instance.waypoints[position].GetComponent<singleWaypoint>().pointsAwarded);
                if (waypointsScript.instance.waypoints[position].GetComponent<singleWaypoint>().isShop)
                {
                    //if landed on shop
                    setShopPlayer();
                }
                if (hasToMove)//only if the player q'd the move
                {
                    addTurn();
                }
                forcedMovement = false;
                hasToMove = false;
                hasToMoveBack = false;
                
                //print("turn over");//turn over for the player
            }
            else//if there are tiles left to move
            {
                setPlayerRotation(direction);
                //print("There are tiles to move");
            }
        }



    }
    /**
 * @memo 2022
 * moves player backwards a set number of tiles
 */
    public void moveBack(int tilesToMove)
    {
        if (position > 0 && hasAuthority)//position + roll;
        {
            noTiles = tilesToMove;//number of tiles to move
            hasToMoveBack = true;
            setPlayerRotation(-1);
        }
    }
    /**
 * @memo 2022
 * caller for move back for networking purposes
 */
    public void callMoveBack(int tilesToMove, uint id)
    {
        if (netId == id)//means he called to everyone move back but him
        {
            return;
        }
        //not him calling that card
        moveBack(tilesToMove);
    }
    #endregion
    #region utility
    /**
* @memo 2022
* gets a whole deck
*/
    [ClientRpc]
    public void getDeck(int cardsToGive)//gets a whole new deck for player
    {
        getDeckLocal(cardsToGive);
    }
    /**
* @memo 2022
* gets the local deck
*/
    public void getDeckLocal(int cardsToGive)//gets a whole new deck for player
    {
        deck.RemoveRange(0, deck.Count);
        //print("gave cards to player");
        int i = 0;
        while (i < cardsToGive)
        {
            cardObject tempCard = getRandomCard();
            deck.Add(tempCard);
            if (isLocalPlayer)
            {
                addCardOnline(tempCard.cardNumber);
            }
            
            i++;
        }
        setDeckPlayer();
    }
    /**
* @memo 2022
* gets a random new card and adds it to deck
*/
    public void getCard()//gets a new random card
    {
        cardObject tempCard = getRandomCard();
        deck.Add(tempCard);
        if (isLocalPlayer)
        {
            addCardOnline(tempCard.cardNumber);
        }
        setDeckPlayer();
    }

    /**
 * @memo 2022
 * sets deck
 */
    public void setDeckPlayer()
    {
        if (!isLocalPlayer) { return; }
        cardHolder.setDeck(deck);
        
    }
    /**
* @memo 2022
* sets shop
*/
    public void setShopPlayer()
    {
        if (gameManagerScript.instance.areShopsEnabled == false) { return; }
        List<cardObject> tempCard = new List<cardObject>();
        int i = 0;
        while (i < 2)
        {
            //print("addcardshop");
            cardObject tc = getRandomCard();
            tempCard.Add(tc);
            i++;
        }
        shopHolder.setDeck(tempCard);
        shopHolder.openShop();
        isInShop = true;
    }

    /**
* @memo 2022
* gets random card from stack
*/
    public cardObject getRandomCard()
    {
        if (!isLocalPlayer) { return null; }
        int rand = Random.Range(0, gameManagerScript.instance.allCards.Length);
        return gameManagerScript.instance.allCards[rand];
    }


    /**
 * @memo 2022
 * sets player rotation
 */
    private void setPlayerRotation(int i)
    {
        transform.rotation = Quaternion.LookRotation(transform.forward, waypointsScript.instance.waypoints[position + i].position - transform.position);
    }
    /**
 * @memo 2022
 * removes card from deck
 */
    public void removeCard(cardObject tempCard)
    {
        deck.Remove(tempCard);
        if (isLocalPlayer)
        {
            removeCardOnline(tempCard.cardNumber);
        }
        
        setDeckPlayer();
    }
    /**
 * @Denzil 2022
 * setter for playerpos
 */
    public void setPlayerPosition(int position)
    {
        this.position = position;
    }
    /**
     * @Denzil 2022
     * getter for playerPOS
     */
    public int getPlayerPosition()
    {
        return position;
    }
    /**
 * @memo 2022
 * getter for the moveback function
 */
    public bool getHasToMoveBack()
    {
        return hasToMoveBack;
    }
    /**
* @memo 2022
* enables /disables nlac holse locally
*/
    public void enableDisableBlackHolesLocal(bool var)
    {
        waypointsScript.instance.disableEnableBlackHoles(var);
    }
    /**
* @memo 2022
* enable/disable shops locally
*/
    public void enableDisableShopsLocal(bool var)
    {
        waypointsScript.instance.disableEnableShops(var);
    }
    #endregion
    #region commands
    /**
* @memo 2022
* adds turn to gameManager and updates in server
*/
    [Command]
    private void addTurn()
    {
        gameManagerScript gameManager = gameManagerScript.instance;
        gameManager.turn++;

        //gameManager.UpdateRoundTurn();

        if (gameManager.turn == CustomNetworkManager.instance.GamePlayers.Count)
        {
            gameManager.turn = 0;
            gameManager.round++;
        }
    }
    /**
* @memo 2022
* sets player steam id
*/
    public void setSteamID(ulong steamID)
    {
        playerSteamID = steamID;
    }
    /**
* @memo 2022
* updates steam id on other clients
*/
    private void HandleSteamIDUpdated(ulong oldVal, ulong newVal)
    {
        var cSteamID = new CSteamID(newVal);
        steamName = SteamFriends.GetFriendPersonaName(cSteamID);
        ScoreBoard.UpdateScores();
    }
    /**
 * @memo 2022
 * caller for add turn beacuse of networking
 */
    public void callAddTurn()
    {
        //isTurn = false;
        addTurn();
        //timer = 0; 
    }
    /**
 * @memo 2022
 * handler for the skip next turn card
 */
    public void skipNextPlayerTurn()
    {
        addTurn();
        //addTurn();
    }
    /**
 * @memo 2022
 * disables shops on command which means on server
 */
    [Command]
    public void disableShops(bool var)
    {
        gameManagerScript.instance.areShopsEnabled = var;
    }
    /**
* @memo 2022
* move everyone but you handler
*/
    [Command]
    public void everyoneBackButYou(int tiles, uint id)
    {
        gameManagerScript.instance.allPlayersMoveBack(tiles, id);
    }
    /**
* @memo 2022
* swaps decks, not implemented
*/
    [Command]
    public void swapDecks()
    {
        gameManagerScript.instance.swapDecks(this);
    }
    /**
* @memo 2022
* subtracts score in server
*/
    [Command]
    public void subtractScore(int val)
    {
        score -= val;
    }
    /**
 * @memo 2022
 * calls all players to move on the server
 */
    [Command]
    public void callAllPlayersMove(int tiles)
    {
        gameManagerScript.instance.allPlayersMove(tiles);
    }
    /**
* @memo 2022
* add score to the player
*/
    [Command]
    public void addScore(int toAdd)
    {
        if (doublePoints) { toAdd *= 2; doublePoints = false; }
        score += toAdd;
    }
    /**
* @memo 2022
* adds card to syncdeck
*/
    [Command]
    public void addCardOnline(int var)
    {
        syncDeck.Add(var);
    }
    /**
* @memo 2022
* removes card on syncdeck
*/
    [Command]
    public void removeCardOnline(int var)
    {
        syncDeck.Remove(var);
    }
    /**
* @memo 2022
* clears syncdeck
*/
    [Command]
    public void clearDeckOnline()
    {
        syncDeck.Clear();
    }
    /**
* @memo 2022
* enable disable blackholes on the server
*/
    [Command]
    public void enableDisableBlackholes(bool val)
    {
        gameManagerScript.instance.disableBlackholes(val);
        gameManagerScript.instance.areTrapsEnabled = val;
    }
    /**
* @memo 2022
* tells the server you own game
*/
    [Command]
    private void winGame()
    {
        gameManagerScript.instance.playerWon();
    }
    /**
* @memo 2022
* if someone won do this
*/
    public void calledWin()
    {
        if (hasWon)
        {
            //show win screen
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
        }
    }
    #endregion
    #region getters & setters
    /**
* @memo 2022
* getter for turn
*/
    public int getTurn()
    {
        return turn;
    }

    /**
 * @memo 2022
 * sets the deck to a specific thing transforming a sync list to a list
 */
    
    public void setDeckToSpecific(Controller oppController)
    {
        List<int> tempList = getSyncDeck(oppController);
        print("pass " + netId + " " +" "+this.gameObject.name);
        if (!hasAuthority) { return; }
        print("pass1");
        clearDeckOnline();
        Debug.LogWarning("pass2 " + syncDeck.Count);
        foreach (int tempCard in tempList)
        {
            addCardOnline(tempCard);
            print("added " + tempCard);
        }
        print("pass3 " + syncDeck.Count);
        setDeckFromSync();
        print("pass5");
        //StartCoroutine(waitForNetwork());
    }
    /**
* @memo 2022
* on changed deck, not implemented for swap decks
*/
    public void changedDeckOnline(int[] oldVal, int []newVal)
    {
        print("changed deck!");
        setDeckFromSync();
    }
    /**
* @memo 2022
* sets deck from another player, not implemented
*/
    public void setDeckFromSync()
    {
        deck.RemoveRange(0, deck.Count);
        print("pass4");
        foreach (int cardID in syncDeck)
        {
            deck.Add(gameManagerScript.instance.getCardByID(cardID));
        }
        setDeckPlayer();
    }
    /**
* @memo 2022
* same as before
*/
    public List<int> getSyncDeck(Controller playerController)
    {
        List<int> tempList = new List<int>();
        foreach (int intTemp in playerController.syncDeck)
        {
            tempList.Add(intTemp);
        }
        return tempList;
    }

    /**
* @memo 2022
* getter for hasmtomove
*/
    public bool getHasToMove()
    {
        return hasToMove;
    }
    /**
 * @memo 2022
 * quit shop method
 */
    public void quitShop()
    {
        isInShop = false;
        shopTimer = 0;
        shopHolder.closeShop();
    }
    /**
* @memo 2022
* getter for forcedMovement
*/
    public bool getForcedMovement()
    {
        return forcedMovement;
    }
    /**
* @memo 2022
* waits 1 second for net to update on beginning, better do thisthan deal with ping just for this game
*/

    public IEnumerator waitForNet()
    {
        yield return new WaitForSeconds(1f);
        UpdateScoreboard();
    }
    #endregion
    #region scoreboardUtil
    /**
* @memo 2022
* Creates scoreboard
*/
    public void CreateScoreBoard()
    {
        //print("pass controller");
        //print("islocal? " + isLocalPlayer + " " + this.gameObject);
        if (!isLocalPlayer) { return; }
        //print("passcontroller2");
        ScoreBoard.createPlayerScoreboard();
    }
    /**
* @memo 2022
* callable method to update scoreboard
*/
    public void UpdateScoreboard()
    {
        if (turn == gameManagerScript.instance.turn)
        {
            isTurn = true;
        }
        StartCoroutine(scoreUpdate());
    }
    /**
* @memo 2022
* updates the score .3 seconds after it updates on the network as if updated instantly will sometimes not update properly
*/
    private IEnumerator scoreUpdate()
    {
        yield return new WaitForSeconds(.3f);
        ScoreBoard.UpdateScores();
    }
    #endregion
    #region animations
    /**
* @memo 2022
* fire animation when rocket is moving, not implemented
*/
    IEnumerator fire()
    {
        Random.Range(0, 1);
        yield return null;
    }
    #endregion

}
