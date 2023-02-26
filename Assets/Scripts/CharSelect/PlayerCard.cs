using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System;

public class PlayerCard : MonoBehaviour
{
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

    public void UpdatePlayer(Player player)
    {
        this.player = player;
        playerNameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        LobbyManager.PlayerCharacter playerCharacter = EnumUtils.Parse<LobbyManager.PlayerCharacter>(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
        characterIconImage.sprite = LobbyAssets.Instance.GetSprite(playerCharacter);

        LobbyManager.ReadyState readyState =
                EnumUtils.Parse<LobbyManager.ReadyState>(player.Data[LobbyManager.KEY_IS_LOCKED].Value);

        switch (readyState)
        {
            default:
            case LobbyManager.ReadyState.True:
                Locked.SetActive(true);
                break;
            case LobbyManager.ReadyState.False:
                Locked.SetActive(false);
                break;
        }
    }
}
