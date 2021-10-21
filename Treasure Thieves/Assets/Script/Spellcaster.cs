using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : MonoBehaviour
{
    public GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            //Spawn in the fireball gameobject
            GameObject fireball = Instantiate(projectile, transform) as GameObject;
            //Get the rigidbody of the spell game object that was just spawned
            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            //Shoot the spell forward
            rb.velocity = transform.forward * 20;
        }
    }
}
