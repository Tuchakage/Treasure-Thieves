using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : MonoBehaviour
{
    public GameObject projectile;
    private float basiccooldown = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (basiccooldown > 0) 
        {
            //When the cooldown for basic attack is more than 1 then it will start counting down
            basiccooldown -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space)) 
        {   //If there is no cooldown (Its at 0) then player can use the basic attack
            if (basiccooldown <= 0) 
            {
                //Spawn in the fireball gameobject
                GameObject fireball = Instantiate(projectile, transform) as GameObject;
                //Get the rigidbody of the spell game object that was just spawned
                Rigidbody rb = fireball.GetComponent<Rigidbody>();
                //Shoot the spell forward
                rb.velocity = transform.forward * 20;
                //Add cooldown to the basic attack
                basiccooldown = 1;
            }

        }
    }
}
