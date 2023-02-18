using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramesPerSecond : MonoBehaviour
{
    TMP_Text fpsText;

    void Start()
    {
        fpsText = this.transform.Find("FPS Text").GetComponent<TMP_Text>();
    }

    void Update()
    {
        fpsText.text = "FPS " + 1/ Time.deltaTime;
    }
}
