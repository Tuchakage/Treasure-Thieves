using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun
{
    public float health = 100;

    Spellcaster spell;

    [SerializeField]
    public string attackname; // This will make it so that the game can find the name of the attack and find the amount of damage it does
    // Start is called before the first frame update
    void Start()
    {
        //Gets The Spellcaster script
        spell = GetComponent<Spellcaster>();
    }

    // Update is called once per frame
    void Update()
    {
        // When the health is less than or equal to 0 then destroy the Player Game Object
        if (health <= 0) 
        {
            Destroy(this.gameObject);
        }
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

    private void OnParticleCollision(GameObject col)
    {
        //Makes sure that when you get hit its from someone else and not yourself
        if (!photonView.IsMine) 
        {
            //Depending on the attack the player will lose a certain amount of health
            if (col.gameObject.tag == "Basic Attack" && this.gameObject.tag == "Player")
            {
                // This is here so the game can find the amount of damage this attack does
                attackname = "Basic Attack";
                //Gets the Photon View of the object it collided with
                PhotonView photonView = PhotonView.Get(this);
                //Gets the TakeDamage() function and applys it to the target
                photonView.RPC("TakeDamage", RpcTarget.All, spell.DealDamage());
                //Destroy The Spell Game Object
                Destroy(col.transform.parent.gameObject);
                //Debug.Log("Basic Attack has hit " + col.gameObject.name);
            }
        }

    }
}
