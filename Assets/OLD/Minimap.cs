//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class Minimap : MonoBehaviour
//{
//    public GameObject TwoDSpriteGO;
//    private Image TwoDSprite;
//    private GameObject MinimapCamera;
//    private GameObject maincam;
//    GameObject[] allObjects;
//    private GameObject PlayerEntity;
//    [SerializeField] Sprite player;
//    // Start is called before the first frame update
//    void Start()
//    {
//        allObjects = GameObject.FindObjectsOfType<GameObject>();
//        MinimapCamera = GameObject.Find("MinimapCamera");
//        maincam = GameObject.Find("Main Camera");
//        //TwoDSpriteGO = GameObject.FindGameObjectWithTag("temp");
//        TwoDSprite = TwoDSpriteGO.GetComponentInChildren<Image>();

//        foreach (var allObjects in allObjects)
//        {
//            if (allObjects.gameObject.tag == "Player")
//            {
//                TwoDSprite.sprite = player;
//                PlayerEntity = allObjects.transform.Find("Player Entity").gameObject;
//                Instantiate(TwoDSpriteGO, allObjects.transform.position, Quaternion.identity);
//            }
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        allObjects = FindObjectsOfType<GameObject>();
//        foreach (var objects in allObjects)
//        {
//            if (objects.gameObject.tag == "Player")
//            {
//                PlayerEntity = objects.transform.Find("Player Entity").gameObject;
//                MinimapCamera.transform.position = PlayerEntity.transform.position;
//                MinimapCamera.transform.position = new Vector3(MinimapCamera.transform.position.x, 60f, MinimapCamera.transform.position.z);
//                Vector3 forward = maincam.transform.forward;
//                float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
//                foreach (var allObjects2 in allObjects)
//                {
//                    if (allObjects2.gameObject.tag == "temp")
//                    {
//                        allObjects2.GetComponentInChildren<Image>().sprite = player;
//                        allObjects2.transform.position = new Vector3(MinimapCamera.transform.position.x, 13f, MinimapCamera.transform.position.z);
//                        allObjects2.GetComponentInChildren<Image>().transform.rotation = Quaternion.Euler(90, targetAngle, 0);
//                    }
//                }
//            }
//        }
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject TwoDSprite;
    private GameObject MinimapCamera;
    private GameObject maincam;

    // Start is called before the first frame update
    void Start()
    {
        MinimapCamera = GameObject.Find("MinimapCamera");
        maincam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        MinimapCamera.transform.position = transform.position;
        MinimapCamera.transform.position = new Vector3(MinimapCamera.transform.position.x, 200f, MinimapCamera.transform.position.z);

        Vector3 forward = maincam.transform.forward;
        float targetAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        TwoDSprite.transform.rotation = Quaternion.Euler(90, targetAngle, 0);
    }
}
