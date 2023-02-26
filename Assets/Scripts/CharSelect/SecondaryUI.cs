using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondaryUI : MonoBehaviour
{
    public static SecondaryUI Instance { get; private set; }



    [SerializeField] private GameObject loadoutUI;
    [SerializeField] private Button changeDickScatButton;
    [SerializeField] private Button changeGrenadeButton;
    [SerializeField] private Button changeRPGButton;
    [SerializeField] private Button changeShotgunButton;
    [SerializeField] private Button changeSniperButton;
    [SerializeField] private Button changeStaffButton;
    [SerializeField] private Button changeSwordButton;

    [SerializeField] private TextMeshProUGUI equippedSecondary;
    private void Awake()
    {
        Instance = this;

        Hide();

        changeDickScatButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.DickScat);
            equippedSecondary.text = "DickScat";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeGrenadeButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.Grenade);
            equippedSecondary.text = "Grenade";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeRPGButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.RPG);
            equippedSecondary.text = "RPG";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeShotgunButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.Shotgun);
            equippedSecondary.text = "Shotgun";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeSniperButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.Sniper);
            equippedSecondary.text = "Sniper";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeStaffButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.Staff);
            equippedSecondary.text = "Staff";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeSwordButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponSecondary(LobbyManager.PlayerWeapon.Sword);
            equippedSecondary.text = "Sword";
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
