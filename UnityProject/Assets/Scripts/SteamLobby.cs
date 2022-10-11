using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{

    public GameObject HostButton;
    public GameObject HostButtons;
    public GameObject JoinButton;
    public GameObject JoinButtons;
    public GameObject backButton;
    public GameObject title;
    public GameObject rulebook;

    public static SteamLobby Instance;

    //callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //variables
    public ulong currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;

    public static CSteamID lobbyID { get; private set; }

    /**
* @memo 2021
* Simple steam lobby script
*/
    void Start()
    {
        if (!SteamManager.Initialized) { return; }
        if (Instance == null)
        {
            Instance = this;
        }
        manager = GetComponent<CustomNetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    /**
* @memo 2021
* Host friend lobby on steam
*/
    public void HostLobbyFriends()
    {

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }
    /**
* @memo 2021
* Host publc lobby on steam
*/
    public void HostLobbyPublic()
    {

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }
    /**
* @memo 2021
* hosts a private lobbyon steam
*/
    public void HostLobbyPrivate()
    {

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, manager.maxConnections);
    }
    /**
* @memo 2021
* Shows the host buttons
*/
    public void ShowHostButtons()//on host button pressed
    {
        backButton.SetActive(true);
        HostButton.SetActive(false);
        HostButtons.SetActive(true);
        JoinButton.SetActive(false);
    }

    public void ShowRuleBook()
    {
        rulebook.SetActive(true);
        HostButton.SetActive(false);
        JoinButton.SetActive(false);
        title.SetActive(false);
    }


    /**
* @memo 2021
* simple back button script
*/
    public void back()//on back button pressed
    {
        backButton.SetActive(false);
        HostButton.SetActive(true);
        HostButtons.SetActive(false);
        JoinButton.SetActive(true);
        rulebook.SetActive(false);
        title.SetActive(true);

    }
    /**
* @memo 2021
* On lobby created
*/
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }
        Debug.Log("Lobby created succesfully");
        lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        manager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        
    }
    /**
* @memo 2021
* If someone requests to join lobby do this
*/
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    /**
* @memo 2021
* When player joins lobby do this
*/
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //everyone
        currentLobbyID = callback.m_ulSteamIDLobby;

        //clients
        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();
    }
    /**
* @memo 2021
* just read note
*/
    public void LobbyList()
    {
        SteamMatchmaking.RequestLobbyList();//??????????dont even know if this works (memo 2021) //edit after a year i still scratch my head but it works
    }
}
