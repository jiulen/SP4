using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerEntityManager : EntityBase
{
    public Canvas DamageCanvas;
    private GameObject DamageUIManager;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        DamageUIManager = GameObject.Find("DamageIndicatorUIManager");
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float hp)
    {
        Canvas dmgImg = Instantiate(DamageCanvas) as Canvas;
        dmgImg.GetComponentInChildren<DamageIndicator>().sourcePosition = new Vector3(0, 0, 0);
        dmgImg.transform.SetParent(DamageUIManager.transform);
        SetHealth(GetHealth() - hp);

        if (Health <= 0)
        {
            Destroy(transform.root.gameObject);
        }
    }
}
