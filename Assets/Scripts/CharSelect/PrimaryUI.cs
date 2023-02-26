using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PrimaryUI : MonoBehaviour
{
    public static PrimaryUI Instance { get; private set; }



    [SerializeField] private GameObject loadoutUI;
    [SerializeField] private Button changeDickScatButton;
    [SerializeField] private Button changeGrenadeButton;
    [SerializeField] private Button changeRPGButton;
    [SerializeField] private Button changeShotgunButton;
    [SerializeField] private Button changeSniperButton;
    [SerializeField] private Button changeStaffButton;
    [SerializeField] private Button changeSwordButton;

    [SerializeField] private TextMeshProUGUI equippedPrimary;



    private void Awake()
    {
        Instance = this;

        Hide();

        changeDickScatButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.DickScat);
            equippedPrimary.text = "DickScat";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeGrenadeButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.Grenade);
            equippedPrimary.text = "Grenade";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeRPGButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.RPG);
            equippedPrimary.text = "RPG";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeShotgunButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.Shotgun);
            equippedPrimary.text = "Shotgun";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeSniperButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.Sniper);
            equippedPrimary.text = "Sniper";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeStaffButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.Staff);
            equippedPrimary.text = "Staff";
            Hide();
            loadoutUI.SetActive(true);
        });
        changeSwordButton.onClick.AddListener(() => {
            LobbyManager.Instance.UpdatePlayerWeaponPrimary(LobbyManager.PlayerWeapon.Sword);
            equippedPrimary.text = "Sword";
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
