using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

public class CharSelectUI : MonoBehaviour
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
    [SerializeField] private Transform container;
    [SerializeField] private Transform playerCard;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Transform introSpawnPoint;




    private void Awake()
    {
        Instance = this;

        playerCard.gameObject.SetActive(false);

        //RhinoBtn.onClick.AddListener(() => {
        //    //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        //});
        //AnglerBtn.onClick.AddListener(() => {
        //    //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Ninja);
        //});
        //WintonBtn.onClick.AddListener(() => {
        //    //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Zombie);
        //});

        LockBtn.onClick.AddListener(() => {
            CharSelectManager.Instance.UpdatePlayerLock();
        });

        //changeGameModeButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.ChangeGameMode();
        //});

        //startGameButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.StartGame();
        //});
    }

    private void Start()
    {
        CharSelectManager.Instance.OnSelect += UpdateLobby_Event;
        //Hide();
    }

    private void UpdateLobby_Event(object sender, CharSelectManager.CharLobbyEventArgs e)
    {
        UpdateLobby();
    }
    private void UpdateLobby()
    {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        ClearLobby();

        foreach (Player player in lobby.Players)
        {
            Transform playerSingleTransform = Instantiate(playerCard, container);
            playerSingleTransform.gameObject.SetActive(true);
            PlayerCard lobbyPlayerSingleUI = playerSingleTransform.GetComponent<PlayerCard>();

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        //changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        //lobbyNameText.text = lobby.Name;
        //playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        //gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;

        Show();
    }

        private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerCard) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
