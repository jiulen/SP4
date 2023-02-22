using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class GrenadeProjectile : ProjectileBase
{
    private Explosion explosion;
    private Rigidbody rb;
    [SerializeField] GrenadeHold grenadeWeapon;
    public enum GrenadeState
    {
        NONE,
        EXPLODE
    }
    public GrenadeState state;

    // Start is called before the first frame update
    void Start()
    {
        state = GrenadeState.NONE;
        rb = GetComponent<Rigidbody>();
        explosion = GetComponent<Explosion>();
        explosion.damage = grenadeWeapon.damage[0];
        rb.isKinematic = true;
        duration = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GrenadeState.EXPLODE)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= duration)
            {
                GrenadeExplode();
                Destroy(this.gameObject);
            }
        }
    }

    private void GrenadeExplode()
    {
        explosion.Explode();
        particleManager.GetComponent<ParticleManager>().CreateEffect("Explosion_PE", this.transform.position, this.transform.forward);
    }
}
