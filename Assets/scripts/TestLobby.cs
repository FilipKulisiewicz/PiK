using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class TestLobby: MonoBehaviour
{
    private static Lobby joinedLobby;
    public const string KEY_START_GAME = "Start";
    public event EventHandler<EventArgs> OnGameStarted;

    private const float heartbeatTimerMax = 1.1f; 
    private float heartbeatTimer = heartbeatTimerMax;
                
    private async void Start() {
        int randomNum = UnityEngine.Random.Range(1, 99999999);
        InitializationOptions initializationOptions = new InitializationOptions(); 
        initializationOptions.SetProfile(randomNum.ToString());
        await UnityServices.InitializeAsync(initializationOptions);
        
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update() {
        HandleLobbyHeartbeat();
    }
    
    private async void HandleLobbyHeartbeat() {
        if (joinedLobby != null) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0.0f) {
                heartbeatTimer = heartbeatTimerMax;
                // if(IsLobbyHost()){
                //     await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                // }
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    
                if (joinedLobby.Data[KEY_START_GAME].Value != "0") {
                    Debug.Log("Got a Key" + joinedLobby.Data[KEY_START_GAME].Value);
                    // Start Game!
                    if (!IsLobbyHost()) { // Lobby Host already joined Relay
                        TestRelay.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        Debug.Log("Joined");
                    }
                    joinedLobby = null;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public static bool IsLobbyHost() { 
        return joinedLobby != null & joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public static async void CreateLobby() {
        try { 
            string lobbyName = "MyLobby";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            joinedLobby = lobby;
            Debug.Log("Created Lobby!" + lobby.Name);
        } 
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public static async void QuickJoinLobby() {
        try { 
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined Lobby!" + joinedLobby.Name);
        } 
        catch (LobbyServiceException e) { 
            Debug.Log(e);
        }
    }

    public static async void StartGame() {
        if (IsLobbyHost()) {
            try {
                Debug.Log("StartGame");
                string relayCode = await TestRelay.Instance.CreateRelay();
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions { 
                    Data = new Dictionary<string, DataObject> {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });
                joinedLobby = lobby;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
}