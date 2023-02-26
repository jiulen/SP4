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

    [SerializeField] private GameObject loadoutUI;
    [SerializeField] private Button CharBtn;
    [SerializeField] private Button PrimaryBtn;
    [SerializeField] private Button SecondaryBtn;
    [SerializeField] private Button SpecialBtn;

    [SerializeField] private GameObject Chars;
    [SerializeField] private GameObject Primaries;
    [SerializeField] private GameObject Secondaries;
    //[SerializeField] private Button WintonBtn;

    [SerializeField] private Button LockBtn;

    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Transform charactersHolder;
    //[SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private Transform playerCard;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Transform introSpawnPoint;




    void Awake()
    {
        Instance = this;

        playerCard.gameObject.SetActive(false);

        //UpdateLobby(LobbyManager.Instance.GetJoinedLobby());

        CharBtn.onClick.AddListener(() =>
        {
            loadoutUI.SetActive(false);
            Chars.SetActive(true);
        });
        PrimaryBtn.onClick.AddListener(() =>
        {
            //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Angler);
        });
        SecondaryBtn.onClick.AddListener(() =>
        {
            //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Winton);
        });

        LockBtn.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerLock();

        });

        //changeGameModeButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.ChangeGameMode();
        //});

        //startGameButton.onClick.AddListener(() => {
        //    LobbyManager.Instance.StartGame();
        //});
    }

    void Start()
    {
        LobbyManager.Instance.OnSelect += UpdateLobby_Event;
        LobbyManager.Instance.OnCharSelect += LobbyManager_OnCharSelect;
        Hide();
    }
    private void LobbyManager_OnCharSelect(object sender, LobbyManager.LobbyEventArgs e)
    {
        Show();
    }
    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
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
