using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour
{
    ParticleSystem effect;
    public void OnEnable()
    {
        effect = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (!effect.IsAlive())
        {
            Destroy(this.gameObject);
        }
       
    }
}
