using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectUI : MonoBehaviour
{
    public static CharSelectUI Instance { get; private set; }

    [SerializeField] private Button CharBtn;
    [SerializeField] private Button PrimaryBtn;
    [SerializeField] private Button SecondaryBtn;
    [SerializeField] private Button SpecialBtn;

    [SerializeField] private Button RhinoBtn;
    [SerializeField] private Button BeastBtn;
    [SerializeField] private Button AnglerBtn;
    [SerializeField] private Button WintonBtn;

    [SerializeField] private Button LockBtn;

    [SerializeField] private TextMeshProUGUI Player1Name;
    [SerializeField] private TextMeshProUGUI Player2Name;
    [SerializeField] private TextMeshProUGUI Player3Name;
    [SerializeField] private TextMeshProUGUI Player4Name;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
