using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

public class CharSelectUI : NetworkBehaviour
{
    public static CharSelectUI Instance { get; private set; }

    [SerializeField] private Button CharBtn;
    [SerializeField] private Button PrimaryBtn;
    [SerializeField] private Button SecondaryBtn;
    [SerializeField] private Button SpecialBtn;

    [SerializeField] private Button RhinoBtn;
    [SerializeField] private Button BeastBtn;
    [SerializeField] private Button AnglerBtn;
    [SerializeField] private Button WintonBtn;

    [SerializeField] private Button LockBtn;

    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform charactersHolder;
    //[SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Transform introSpawnPoint;


    private NetworkList<CharacterSelectState> players;
    public string clientName;
    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }



    public override void OnNetworkSpawn()
    {

        if (IsClient)
        {

            //Character[] allCharacters = characterDatabase.GetAllCharacters();

            //foreach (var character in allCharacters)
            //{
            //    var selectbuttonInstance = Instantiate(selectButtonPrefab, charactersHolder);
            //    selectbuttonInstance.SetCharacter(this, character);
            //    characterButtons.Add(selectbuttonInstance);
            //}
            Debug.Log("Updated");
            players.OnListChanged += HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                players.Add(new CharacterSelectState(client.ClientId));
                Debug.Log(client.ClientId);
            }
            int i = 0;
            foreach (Player player in LobbyManager.Instance.joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    clientName = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
                    playerCards[i].SetDisplay(clientName);
                }
                i++;
            }
        }
        players.OnListChanged += HandlePlayersStateChanged;




    }
        public void Select(Character character)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (players[i].IsLockedIn) { return; }

            if (players[i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                characterId,
                players[i].IsLockedIn
            );
        }
    }

    public void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            //if (!characterDatabase.IsValidCharacterId(players[i].CharacterId)) { return; }

            //if (IsCharacterTaken(players[i].CharacterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                players[i].CharacterId,
                true
            );
        }

        foreach (var player in players)
        {
            if (!player.IsLockedIn) { return; }
        }

        foreach (var player in players)
        {
            HostManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }

        //foreach (var button in characterButtons)
        //{
        //    if (button.IsDisabled) { continue; }

        //    if (IsCharacterTaken(button.Character.Id, false))
        //    {
        //        button.SetDisabled();
        //    }
        //}

        foreach (var player in players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                LockBtn.interactable = false;
                break;
            }

            if (IsCharacterTaken(player.CharacterId, false))
            {
                LockBtn.interactable = false;
                break;
            }

            LockBtn.interactable = true;

            break;
        }
    }

    private bool IsCharacterTaken(int characterId, bool checkAll)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!checkAll)
            {
                if (players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (players[i].IsLockedIn && players[i].CharacterId == characterId)
            {
                return true;
            }
        }

        return false;
    }
}
