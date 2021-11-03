using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spellcaster : MonoBehaviourPun
{
    [SerializeField]
    private Health hs;

    public GameObject basicattack;
    public float dmg;
    //Where the spells will spawn from
    public Transform wand;

    //Timer for the cool down
    public float timer;
    //The max cooldown value (Where the timer will start counting down from
    public float cooldown;
    // Start is called before the first frame update
    void Start()
    {
        //Find The Health Script
        hs = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        //Makes sure i am controlling my own player
        if (photonView.IsMine) 
        {
            if (timer > 0)
            {
                //When the cooldown for basic attack is more than 1 then it will start counting down
                timer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                //If there is no cooldown (Its at 0) then player can use the basic attack
                if (timer <= 0)
                {
                    //Shoots out the lightning bolt
                    photonView.RPC("Shoot", RpcTarget.All);
                    cooldown = 1.0f;
                    //Add cooldown to the basic attack
                    timer = cooldown;
                }
            }
        } 
    }

    //This will send the Damage value of the attack to the Health Script
    public float DealDamage()
    {
        //Depending on the spell game object depends on the amount of damage the attack will do
        if (hs.attackname == "Basic Attack")
        {
            dmg = 5;
        }

        return dmg;
    }

    [PunRPC]
    void Shoot() 
    {
        //Makes sure i only spawn in the Lightning Bolt from my character
        if (photonView.IsMine) 
        {
            //Spawn in the lightning bolt gameobject
            GameObject lightning = PhotonNetwork.Instantiate(basicattack.name, wand.transform.position, Quaternion.identity) as GameObject;

            OwnerOfSpell findowner = lightning.GetComponent<OwnerOfSpell>();
            //Set the Owner of the Spell that was spawned in to this player Id
            findowner.SetOwner(this.GetComponent<PhotonView>().ViewID);
            //Make sure its not a child of the Player Game Object
            lightning.transform.parent = null;
            //Get the rigidbody of the spell game object that was just spawned
            Rigidbody rb = lightning.GetComponent<Rigidbody>();
            //Shoot the spell forward
            rb.velocity = transform.forward * 20;

        }

    }
}
