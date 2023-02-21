using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject[] particleList;
    public enum effectType
    {
        SPARKS,
        BLOOD
    }

    void Awake()
    {

    }

    // Instantiates a new particle effect object. String type must be matching with the name of the particle prefab. This will use the default burst count of the prefab
    public void CreateEffect(string type, Vector3 position, Vector3 normal)
    {
        GameObject effect;
        // While this is not very efficient, it allows for new particle effects to be added without having to go through a lot of hassle
        for(int i = 0; i != particleList.Length; i++)
        {
            if(type == particleList[i].name)
            {
                effect = Instantiate(particleList[i], this.transform);
                effect.transform.position = position;
                effect.transform.rotation = Quaternion.LookRotation(normal);
                return;
            }
        }
        Debug.LogWarning("Failed to find the effect. Make sure your spelling matches the name of the particle effect prefab");
    }

    // Overloads the above CreateEffect function with an added newBurstAmount parameter. This changes the starting burst count of the new particle effect
    public void CreateEffect(string type, Vector3 position, Vector3 normal, int newBurstAmount)
    {
        GameObject effect;
        for (int i = 0; i != particleList.Length; i++)
        {
            if (type == particleList[i].name)
            {
                effect = Instantiate(particleList[i], this.transform);
                effect.transform.position = position;
                effect.transform.rotation = Quaternion.LookRotation(normal);
                ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
                particleSystem.Stop();
                var burst = particleSystem.emission;
                ParticleSystem.Burst newBurst = new ParticleSystem.Burst(0, newBurstAmount);
                burst.SetBurst(0, newBurst);
                particleSystem.Play();
            }
        }
    }


    void Update()
    {
        
    }
}
