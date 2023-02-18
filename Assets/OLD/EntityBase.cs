using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    protected float Health;
    public float MaxHealth;

    protected void Start()
    {
        Health = MaxHealth;
    }
    protected void Update()
    {
        if (Health > MaxHealth)
            Health = MaxHealth;
        if (Health < 0)
            Health = 0;
    }
    public virtual void TakeDamage(float hp, Vector3 dir)
    {
        SetHealth(GetHealth() - hp);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetMaxHealth(float hp)
    {
        Health = MaxHealth = hp;
    }

    public void SetHealth(float hp)
    {
        Health = hp;
    }

    public float GetHealth()
    {
        return Health;
    }
}
