using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
//using Steamworks;
/**
 * @memo 2022
 * Custom network manager
 */
public class CustomNetworkManager : NetworkManager
{
    /**
 * @memo 2022
 * Creates an instance of this object
 */
    public static CustomNetworkManager instance;
    public override void Awake()  
    {
            base.Awake();
            if (instance == null)
        {
            instance = this;
        }
    }

    public List<GameObject> GamePlayers;

    

    /**
 * @memo 2022
 * Whenever a new player joins do this
 */
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        GamePlayers.Add(player);
        Controller playerController = player.GetComponent<Controller>();
        handCards(playerController);
        CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobbyID, numPlayers - 1);
        playerController.setSteamID(steamID.m_SteamID);

        playerController.turn = GamePlayers.Count - 1;

        gameManagerScript.instance.addPlayer();
        print("new player joined!");
        StartCoroutine(playerController.waitForNet());

    }
    /**
 * @memo 2022
 * Starts the game
 */
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
    /**
 * @memo 2022
 * gives cards to the player
 */
    private void handCards(Controller controller)
    {
        controller.getDeck(3);//set here the amount of initial cards you want to give to the player
    }
}
