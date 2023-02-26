using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CharUI : MonoBehaviour
{
    public static CharUI Instance { get; private set; }



    [SerializeField] private GameObject loadoutUI;
    [SerializeField] private Button changeRhinoButton;
    [SerializeField] private Button changeAnglerButton;
    [SerializeField] private Button changeWintonButton;
    [SerializeField] private Button changeBeastButton;

    [SerializeField] private TextMeshProUGUI equippedChar;

    private void Awake()
    {
        Instance = this;

        Hide();

        changeRhinoButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Rhino);
            equippedChar.text = "Rhino";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeAnglerButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Angler);
            equippedChar.text = "Angler";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeWintonButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Winton);
            equippedChar.text = "Winston";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeBeastButton.onClick.AddListener(() =>
        {
            //LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Beast);
            equippedChar.text = "Beast";
            Hide();
            loadoutUI.SetActive(true);
        });
    }

    private void Start()
    {

    }



   

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
