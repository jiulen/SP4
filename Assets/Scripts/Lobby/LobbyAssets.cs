using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite marineSprite;
    [SerializeField] private Sprite ninjaSprite;
    [SerializeField] private Sprite zombieSprite;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.Rhino:   return marineSprite;
            case LobbyManager.PlayerCharacter.Angler:    return ninjaSprite;
            case LobbyManager.PlayerCharacter.Winton:   return zombieSprite;
            case LobbyManager.PlayerCharacter.Beast: return zombieSprite;
        }
    }
}