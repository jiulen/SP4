using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CustomCrosshair : MonoBehaviour
{
    public float centreGap = 5;
    public float xLineLength = 10;
    public float xLineThickness = 2;
    public float yLineLength = 4;
    public float yLineThickness = 2;
    public float centreThickness = 2;
    public float circleThickness = 3;
    public bool enableCircle = false;
    public bool enableLines = true;

    public Color color = Color.green;

    // To be set to true whenever the crosshair values are changed.
    public bool doUpdate = true;

    private RectTransform lineXP;
    private RectTransform lineXN;
    private RectTransform lineYP;
    private RectTransform lineYN;
    private RectTransform lineCentre;

    private Transform circle;
    private ProudLlama.CircleGenerator.StrokeCircleGenerator circleScript;


    void Start()
    {
        lineXP = this.transform.Find("LineXP").GetComponent<RectTransform>();
        lineXN = this.transform.Find("LineXN").GetComponent<RectTransform>();
        lineYP = this.transform.Find("LineYP").GetComponent<RectTransform>();
        lineYN = this.transform.Find("LineYN").GetComponent<RectTransform>();
        lineCentre = this.transform.Find("LineCentre").GetComponent<RectTransform>();

        circle = this.transform.Find("Circle");
        circleScript = circle.GetComponent<ProudLlama.CircleGenerator.StrokeCircleGenerator>();
    }

    void Update()
    {
        if (!doUpdate)
            return;
        doUpdate = false;

        float xOffset = centreGap + xLineLength / 2 + circleThickness -0.1f;
        float yOffset = centreGap + yLineLength / 2 + circleThickness -0.1f;

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

        circleScript.StrokeData = new ProudLlama.CircleGenerator.StrokeData(circleThickness, false);
        circleScript.CircleData = new ProudLlama.CircleGenerator.CircleData(centreGap, 360, 0, 64, true);
        circleScript.Generate();
        circle.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);

        lineXP.gameObject.SetActive(enableLines);
        lineXN.gameObject.SetActive(enableLines);
        lineYP.gameObject.SetActive(enableLines);
        lineYN.gameObject.SetActive(enableLines);

        circle.gameObject.SetActive(enableCircle);

    }
}
