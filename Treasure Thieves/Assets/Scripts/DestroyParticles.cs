using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        //Find the particle systems in the child of this game object
        ps = this.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the particle system is not alive then destroy the game Object
        if (!ps.IsAlive()) 
        {
            Destroy(this.gameObject);
        }
    }
}
