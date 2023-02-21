using UnityEngine;
using UnityEngine.UI;

public class CustomCrosshair : MonoBehaviour
{
    public float centreGap = 5;
    public float xLineLength = 10;
    public float xLineThickness = 2;
    public float yLineLength = 4;
    public float yLineThickness = 2;
    public float centreThickness = 2;

    public Color color = Color.green;

    // To be set to true whenever the crosshair values are changed.
    public bool doUpdate = true;

    private RectTransform lineXP;
    private RectTransform lineXN;
    private RectTransform lineYP;
    private RectTransform lineYN;
    private RectTransform lineCentre;

    void Start()
    {
        lineXP = this.transform.Find("LineXP").GetComponent<RectTransform>();
        lineXN = this.transform.Find("LineXN").GetComponent<RectTransform>();
        lineYP = this.transform.Find("LineYP").GetComponent<RectTransform>();
        lineYN = this.transform.Find("LineYN").GetComponent<RectTransform>();
        lineCentre = this.transform.Find("LineCentre").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!doUpdate)
            return;
        doUpdate = false;

        float xOffset = centreGap + xLineLength / 2;
        float yOffset = centreGap + yLineLength / 2;

        lineXP.sizeDelta = new Vector2(xLineLength, xLineThickness);
        lineXP.anchoredPosition = new Vector3(xOffset, 0);
        lineXP.GetComponent<Image>().color = color;

        lineXN.sizeDelta = new Vector2(xLineLength, xLineThickness);
        lineXN.anchoredPosition = new Vector3(-xOffset, 0);
        lineXN.GetComponent<Image>().color = color;

        lineYP.sizeDelta = new Vector2(yLineThickness, yLineLength);
        lineYP.anchoredPosition = new Vector3(0, yOffset);
        lineYP.GetComponent<Image>().color = color;

        lineYN.sizeDelta = new Vector2(yLineThickness, yLineLength);
        lineYN.anchoredPosition = new Vector3(0, -yOffset);
        lineYN.GetComponent<Image>().color = color;

        lineCentre.sizeDelta = new Vector2(centreThickness, centreThickness);
        lineCentre.GetComponent<Image>().color = color;

    }
}
