using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun
{
    public float health = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void TakeDamage(float damagetaken)
    {
        //Makes sure that when you deal damage you dont take damage from your own attack
        if (!photonView.IsMine) 
        {
            health -= damagetaken;
            Debug.Log("Lost Health");
        }

    }
}
