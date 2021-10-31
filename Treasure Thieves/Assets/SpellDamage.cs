using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellDamage : MonoBehaviour
{
    [SerializeField]
    private float dmg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        //Depending on the spell game object depends on the amount of damage the enemy will take
        if (this.gameObject.name == "Basic Attack") 
        {
            //Gets the Photon View of the object it collided with
            PhotonView photonView = PhotonView.Get(col);
            //Gets the TakeDamage() function and applys it to the target
            photonView.RPC("TakeDamage", RpcTarget.All, dmg);
            Debug.Log("Basic Attack has hit " + col.gameObject.name);
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
