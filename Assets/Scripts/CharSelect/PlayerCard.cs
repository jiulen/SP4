using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    //[SerializeField] private CharacterDatabase characterDatabase;
    //[SerializeField] private Image characterIconImage;
    [SerializeField] private GameObject Locked;
    [SerializeField] private TMP_Text playerNameText;

    public void UpdateDisplay(CharacterSelectState state)
    {
        if (state.CharacterId != -1)
        {
            //var character = characterDatabase.GetCharacterById(state.CharacterId);
            //characterIconImage.sprite = character.Icon;
            //characterIconImage.enabled = true;
        }
        else
        {
            //characterIconImage.enabled = false;
        }

        //playerNameText.text = state.IsLockedIn ? $"Player {state.ClientId}" : $"Player {state.ClientId} (Picking...)";
        
        Locked.SetActive(state.IsLockedIn);
        gameObject.SetActive(true);
    }

    public void SetDisplay(string playername)
    {
        playerNameText.text = playername;
    }
    public void DisableDisplay()
    {
        gameObject.SetActive(false);
    }
}
