using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellDamage : MonoBehaviourPun
{
    [SerializeField]
    private float dmg;

    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject col)
    {
        //Depending on the spell game object depends on the amount of damage the enemy will take
        if (this.gameObject.tag == "Basic Attack" && col.gameObject.tag == "Player")
        {
            //Gets the Photon View of the object it collided with
            photonView = PhotonView.Get(col);
            //Gets the TakeDamage() function and applys it to the target
            photonView.RPC("TakeDamage", RpcTarget.All, dmg);
            //Destroy Object when it hits a player
            //Destroy(transform.parent.gameObject);
            //Debug.Log("Basic Attack has hit " + col.gameObject.name);
        }
    }

    void DealDamage() 
    {
        //Depending on the spell game object depends on the amount of damage the attack will do
        if (this.gameObject.name == "Basic Attack")
        {
            dmg = 5;        
        }
    }
}
