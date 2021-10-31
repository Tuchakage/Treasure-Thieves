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
        health -= damagetaken;
    }

    private void OnCollisionEnter(Collision col)
    {
        //If the player that you are controlling gets hit
        if (photonView.IsMine)
        {

        }
    }
}
