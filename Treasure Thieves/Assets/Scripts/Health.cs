using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health = 100;

    Spellcaster spell;

    KarateKid kid;

    NetworkManager nm;

    PlayerController pc;

    Teams myClass;

    public float deathtimer, hitstuntimer, hitstun;
    public bool dead = false;
    public bool hitStun = false;

    [SerializeField] Animator _playeranim;

    // Start is called before the first frame update
    void Start()
    {
        myClass = GetComponent<Teams>();
        if (myClass.classid == Teams.chosenClass._Spellcaster)
        {
            spell = GetComponent<Spellcaster>();

        }
        else if (myClass.classid == Teams.chosenClass._Karate) 
        {
            kid = GetComponent<KarateKid>();
        }
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
            //Makes it so Players cant attack no more
            ClassSwitch(false);


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
        else 
        {
            //Player cant move until the timer is done
            if (hitstuntimer > 0 && health > 0)
            {
                hitstuntimer -= Time.deltaTime;

                if (hitStun == false)
                {
                        _playeranim.SetTrigger("hitStun");
                        hitStun = true;
                }
            }
            else 
            {
                //Re enable the Player Controller
                pc.enabled = true;
                //Makes it so Players can attack after being hit
                ClassSwitch(true);
            }
        }
    }

    void TakeDamage(float damagetaken)
    {
        //Makes sure that when you deal damage you dont take damage from your own attack
        health -= damagetaken;
        Debug.Log("Lost Health");
        //Make it so player cant move
        pc.enabled = false;
        //Makes it so Players cant attack no more
        ClassSwitch(false);
        //Add Hitstun
        hitstuntimer = hitstun;
        hitStun = false;
       /* if (health > 0)
        {
            _playeranim.SetTrigger("hitStun");
        }*/

        if (pc.carrying) //If Player is carrying
        {
            //Make Player drop the Treasure
            pc.SetCarrying(false);
            //Find The Treasure Trigger Script in all the child of this game Object and call the Drop Treasure function
            GetComponentInChildren<TreasureTrigger>().DropTreasure(pc);
        }



    }

    private void OnParticleCollision(GameObject col)
    {
        //Makes sure that when you get hit its from someone else and not yourself
        if (photonView.IsMine) 
        {
            //Gets The "OwnerOfSpell" script from the spell the player collided with (We get the parent object since the particle sets off the collision)
            OwnerOfSpell ownerofspell = col.transform.parent.gameObject.GetComponent<OwnerOfSpell>();
            Debug.Log("Ok: " + ownerofspell);

            //Get the Owner Id Of The Spell
            int ownerid = ownerofspell.GetOwner();

            //Find the player id of the owner of the attack and get the Spellcaster script
            spell = PhotonView.Find(ownerid).GetComponent<Spellcaster>();
            Teams ct = PhotonView.Find(ownerid).GetComponent<Teams>();

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

    private void OnTriggerEnter(Collider collision)
    {
        //Makes sure that when you get hit its from someone else and not yourself
        if (photonView.IsMine)
        {
            if (collision.tag == "Basic Attack2") //Makes sure its Colliding with an Attack
            {
                //Gets The "OwnerOfKick" script from the spell the player collided with
                OwnerOfKick ownerofkick = collision.gameObject.GetComponent<OwnerOfKick>();
                Debug.Log("Victim = " + this.gameObject.name + "Hit by: " + collision.gameObject.name);
                Debug.Log("Ok: " + ownerofkick);


                //Get the Owner Id Of The Kick
                int ownerid = ownerofkick.GetOwner();

                //Find the player id of the owner of the attack and get the Spellcaster script
                kid = PhotonView.Find(ownerid).GetComponent<KarateKid>();
                Teams ct = PhotonView.Find(ownerid).GetComponent<Teams>();

                Debug.Log("Owner Of Attack: " + PhotonView.Get(PhotonView.Find(ownerid).gameObject));

                //Make Sure the player isnt get hit by its own spell
                if (ownerid != this.GetComponent<PhotonView>().ViewID) //If they are not getting hit by their own spell (So being attacked)
                {
                    Debug.Log("Being attacked");

                    //Depending on the attack the player will lose a certain amount of health
                    if (collision.gameObject.tag == "Basic Attack2")
                    {
                        if (ct.teamid != this.GetComponent<Teams>().teamid)
                        {
                            //Attacked Player will take damage and check how much damage the attack should deal from the owner of the attack
                            TakeDamage(kid.DealDamage());
                            Debug.Log("Basic Attack has hit " + collision.gameObject.name);
                        }
                        else
                        {
                            Debug.Log("Friendly Fire on the red team");
                        }

                    }
                }
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


    //Makes it so that we can turn the Class script on and off (Makes it whether or not the player can attack or not
    void ClassSwitch(bool onoff) 
    {
        if (myClass.classid == Teams.chosenClass._Spellcaster)
        {
            spell.enabled = onoff;

        }
        else if (myClass.classid == Teams.chosenClass._Karate)
        {
            kid.enabled = onoff;
        }
    }

}
