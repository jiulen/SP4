using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class HoverListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int idx;
    public PlayerEntity player;
    public Text Weapontext;

    void Start()
    {
        Weapontext.text = "";
        //player = transform.root.GetComponent<PlayerEntity>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Weapontext.text = player.Weapons[idx].name;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Weapontext.text = "";
    }
}
