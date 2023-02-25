using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityBase : NetworkBehaviour
{
    protected float Health;
    public float MaxHealth;
    private GameObject lasttouch;

    public void SetLastTouch(GameObject source)
    {
        lasttouch = source;
    }
    public GameObject GetLastTouch()
    {
        return lasttouch;
    }
    protected void Start()
    {
        Health = MaxHealth;
    }
    protected void Update()
    {
        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }
    public virtual void TakeDamage(float hp, Vector3 dir, GameObject source, GameObject weaponUsed)
    {
        SetLastTouch(source);
        SetHealth(GetHealth() - hp);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetMaxHealth(float hp)
    {
        SetMaxHealthClientRpc(hp);
    }

    public void SetHealth(float hp)
    {
        SetHealthClientRpc(hp);
    }

    [ClientRpc]
    private void SetHealthClientRpc(float hp)
    {
        Health = hp;
    }

    [ClientRpc]
    private void SetMaxHealthClientRpc(float hp)
    {
        Health = MaxHealth = hp;
    }

    public float GetHealth()
    {
        return Health;
    }
}
