using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System;

public class PlayerCard : MonoBehaviour
{
    //[SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private GameObject Locked;
    [SerializeField] private TMP_Text playerNameText;

    private Player player;

    public static class EnumUtils
    {
        public static TEnum Parse<TEnum>(String value) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }
    }

    //public void UpdateDisplay(Player player)
    //{
    //    this.player = player;
    //    playerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    //    LobbyManager.PlayerCharacter playerCharacter =
    //        EnumUtils.Parse<LobbyManager.PlayerCharacter>(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
    //    characterIconImage.sprite = LobbyAssets.Instance.GetSprite(playerCharacter);

    //    Locked.SetActive(bool.Parse(player.Data[CharSelectManager.KEY_IS_LOCKED].Value));
    //    gameObject.SetActive(true);
    //}

    public void UpdatePlayer(Player player)
    {
        this.player = player;
        playerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        LobbyManager.PlayerCharacter playerCharacter =
            EnumUtils.Parse<LobbyManager.PlayerCharacter>(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
        characterIconImage.sprite = LobbyAssets.Instance.GetSprite(playerCharacter);
        Locked.SetActive(bool.Parse(player.Data[LobbyManager.KEY_IS_LOCKED].Value));
        Debug.Log("debug");
    }
    //public void UpdateDisplay(CharacterSelectState state)
    //{
    //    if (state.CharacterId != -1)
    //    {
    //        //var character = characterDatabase.GetCharacterById(state.CharacterId);
    //        //characterIconImage.sprite = character.Icon;
    //        //characterIconImage.enabled = true;
    //    }
    //    else
    //    {
    //        //characterIconImage.enabled = false;
    //    }

    //    //playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";

    //    Locked.SetActive(state.IsLockedIn);
    //    gameObject.SetActive(true);
    //}

    //public void SetDisplay(string playername)
    //{
    //    playerNameText.text = playername;
    //}
    //public void DisableDisplay()
    //{
    //    gameObject.SetActive(false);
    //}
}
