using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }


    [SerializeField] private Button createButton;
    [SerializeField] private Button lobbyNameButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private Button maxPlayersButton;
    [SerializeField] private Button gameModeButton;
    [SerializeField] private Button mapSelectButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;
    [SerializeField] private TextMeshProUGUI maxPlayersText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI mapSelectText;


    private string lobbyName;
    private bool isPrivate;
    private int maxPlayers;
    private LobbyManager.GameMode gameMode;
    private LobbyManager.MapSelect mapSelect;

    private void Awake() {
        Instance = this;

        createButton.onClick.AddListener(() => {
            LobbyManager.Instance.CreateLobby(
                lobbyName,
                maxPlayers,
                isPrivate,
                gameMode,
                mapSelect
            );
            Hide();
        });

        lobbyNameButton.onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () => {
                // Cancel
            },
            (string lobbyName) => {
                this.lobbyName = lobbyName;
                UpdateText();
            });
        });

        publicPrivateButton.onClick.AddListener(() => {
            isPrivate = !isPrivate;
            UpdateText();
        });

        maxPlayersButton.onClick.AddListener(() => {
            UI_InputWindow.Show_Static("Max Players", maxPlayers,
            () => {
                // Cancel
            },
            (int maxPlayers) => {
                this.maxPlayers = maxPlayers;
                UpdateText();
            });
        });

        gameModeButton.onClick.AddListener(() => {
            switch (gameMode) {
                default:
                case LobbyManager.GameMode.DUEL:
                    gameMode = LobbyManager.GameMode.FFA;
                    break;
                case LobbyManager.GameMode.FFA:
                    gameMode = LobbyManager.GameMode.DUEL;
                    break;
            }
            UpdateText();
        });

        mapSelectButton.onClick.AddListener(() => {
            switch (mapSelect)
            {
                default:
                case LobbyManager.MapSelect.Parallel_Pillars:
                    mapSelect = LobbyManager.MapSelect.Placeholder;
                    break;
                case LobbyManager.MapSelect.Placeholder:
                    mapSelect = LobbyManager.MapSelect.Parallel_Pillars;
                    break;
            }
            UpdateText();
        });

        Hide();
    }

    private void UpdateText() {
        lobbyNameText.text = lobbyName;
        publicPrivateText.text = isPrivate ? "Private" : "Public";
        maxPlayersText.text = maxPlayers.ToString();
        gameModeText.text = gameMode.ToString();
        mapSelectText.text = mapSelect.ToString();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);

        lobbyName = "MyLobby";
        isPrivate = false;
        maxPlayers = 4;
        gameMode = LobbyManager.GameMode.DUEL;
        mapSelect = LobbyManager.MapSelect.Parallel_Pillars;

        UpdateText();
    }

}