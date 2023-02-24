using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharSelectManager : NetworkBehaviour
{
    public static CharSelectManager Instance { get; private set; }


    
    public const string KEY_IS_LOCKED = "false";
    public const string KEY_PLAYER_CHARACTER = "Character";

    public event EventHandler<CharLobbyEventArgs> OnSelect;
    public class CharLobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public enum CharSelect
    {
        Rhino,
        Angler,
        Winton
    }


    public static class EnumUtils
    {
        public static TEnum Parse<TEnum>(String value) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        //LobbyManager.Instance.Update();
    }
    public async void UpdatePlayerLock()
    {
        if (LobbyManager.Instance.joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_IS_LOCKED, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: "true")
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.joinedLobby.Id, playerId, options);
                LobbyManager.Instance.joinedLobby = lobby;

                OnSelect?.Invoke(this, new CharLobbyEventArgs { lobby = LobbyManager.Instance.joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
    {
        if (LobbyManager.Instance.joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_CHARACTER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerCharacter.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(LobbyManager.Instance.joinedLobby.Id, playerId, options);
                LobbyManager.Instance.joinedLobby = lobby;

                OnSelect?.Invoke(this, new CharLobbyEventArgs { lobby = LobbyManager.Instance.joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
