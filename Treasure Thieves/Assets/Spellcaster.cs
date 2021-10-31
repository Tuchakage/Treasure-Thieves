using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spellcaster : MonoBehaviourPun
{

    public GameObject projectile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Makes sure i am controlling my own player
        if (photonView.IsMine) 
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Spawn in the fireball gameobject
                GameObject lightning = Instantiate(projectile, transform) as GameObject;
                //Make sure its not a child of the Player Game Object
                lightning.transform.parent = null;
                //Get the rigidbody of the spell game object that was just spawned
                Rigidbody rb = lightning.GetComponent<Rigidbody>();
                //Shoot the spell forward
                rb.velocity = transform.forward * 20;
            }
        }

    }

}
