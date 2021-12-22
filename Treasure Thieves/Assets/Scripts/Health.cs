using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health = 100;

    Spellcaster spell;

    NetworkManager nm;

    PlayerController pc;

    Teams ct;

    public float deathtimer;
    public bool dead = false;

    [SerializeField] Animator _playeranim;

    // Start is called before the first frame update
    void Start()
    {
        //Gets The Spellcaster script
        spell = GetComponent<Spellcaster>();
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //Get The Player Controller from this Player Object
        pc = GetComponent<PlayerController>();
        //Grab Player Animator
        _playeranim = GetComponent<Animator>();
        deathtimer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // When the health is less than or equal to 0 then destroy the Player Game Object
        if (health <= 0) 
        {
            //Start the countdown
            deathtimer -= Time.deltaTime;
            //Disable Scripts
            pc.enabled = false;
            spell.enabled = false;

            if (dead == false)
            {
                _playeranim.SetTrigger("Death");
                dead = true;
            }

            if (deathtimer == 0 || deathtimer <= 0)
            {
                //Tells the Network Manager that the player is not alive which means display the Respawn Button
                nm.isAlive = false;
                //Destroy The Player Game Object
                photonView.RPC("DestroyObject", RpcTarget.All, GetComponent<PhotonView>().ViewID);
            }
        }
    }

    void TakeDamage(float damagetaken)
    {
        //Makes sure that when you deal damage you dont take damage from your own attack
        health -= damagetaken;
        Debug.Log("Lost Health");

    }

    private void OnParticleCollision(GameObject col)
    {
        //Makes sure that when you get hit its from someone else and not yourself
        if (photonView.IsMine) 
        {
            //Gets The "OwnerOfSpell" script from the spell the player collided with
            OwnerOfSpell ownerofspell = col.transform.parent.gameObject.GetComponent<OwnerOfSpell>();
            Debug.Log("Ok: " + ownerofspell);

            //Get the Owner Id Of The Spell
            int ownerid = ownerofspell.GetOwner();

            //Find the player id of the owner of the attack and get the Spellcaster script
            spell = PhotonView.Find(ownerid).GetComponent<Spellcaster>();
            ct = PhotonView.Find(ownerid).GetComponent<Teams>();

            Debug.Log("Owner Of Attack: " + PhotonView.Get(PhotonView.Find(ownerid).gameObject));

            //Make Sure the player isnt get hit by its own spell
            if (ownerid != this.GetComponent<PhotonView>().ViewID) //If they are not getting hit by their own spell (So being attacked)
            {
                Debug.Log("Being attacked");

                //Depending on the attack the player will lose a certain amount of health
                if (col.gameObject.tag == "Basic Attack")
                {
                    if(ct.teamid != this.GetComponent<Teams>().teamid)
                    {
                        //Attacked Player will no longer be able to carry the Treasure
                        pc.carrying = false;
                        //Attacked Player will take damage and check how much damage the attack should deal from the owner of the attack
                        TakeDamage(spell.DealDamage());
                        //Destroy The Spell Game Object For Everyone
                        photonView.RPC("DestroyObject", RpcTarget.All, col.transform.parent.gameObject.GetComponent<PhotonView>().ViewID); 
                        Debug.Log("Basic Attack has hit " + col.gameObject.name);
                    } else
                    {
                        Debug.Log("Friendly Fire on the red team");
                    }

                    

                }
            }
            else //If they are hitting themselves
            {
                Debug.Log("Hitting yourself");
                //Destroy The Spell Game Object For Everyone
                photonView.RPC("DestroyObject", RpcTarget.All, col.transform.parent.gameObject.GetComponent<PhotonView>().ViewID);
                //Allow the player to shoot again without any cooldown
                spell.timer = 0;
            }

        }

    }

    //This function allows the variables inside to be sent over the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player so send the other computers the data
            stream.SendNext(health);
        }
        else
        {
            //Network player that receives the data
            health = (float)stream.ReceiveNext();
        }
    }


    [PunRPC]
    void DestroyObject(int go)
    {
        //Find the Id of the Game Object that needs to be destroyed
        Destroy(PhotonView.Find(go).gameObject);
    }



}
