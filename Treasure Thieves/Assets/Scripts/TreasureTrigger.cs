using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TreasureTrigger : MonoBehaviourPun
{
    [SerializeField]
    bool isPickedUp; //Check if the Treasure has been picked up
    [SerializeField]
    bool canBePickedUp; //Check if the treasure can be picked up
    GameObject parentObject;
    Rigidbody rb; // The Rigidbody of the Treasure Game Object

    [SerializeField] Spellcaster spell;
    [SerializeField] KarateKid karate;

    // Start is called before the first frame update
    void Start()
    {
        //The parent Object of the trigger (The Treasure GameObject) is put in a variable
        parentObject = this.gameObject.transform.parent.gameObject;
        //Find the Rigidbody of the Treasure Game Object (Parent GameObject)
        rb = parentObject.GetComponent<Rigidbody>();
        karate = parentObject.GetComponent<KarateKid>();
        spell = parentObject.GetComponent<Spellcaster>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp == true)
        {
            karate = parentObject.GetComponent<KarateKid>();
            spell = parentObject.GetComponent<Spellcaster>();
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player") 
        {
            //Get the Player ID of the object 
            int playerid = col.gameObject.GetComponent<PhotonView>().ViewID;

            //Get the Player Controller script from the player that is touching the Trigger
            PlayerController playerController = PhotonView.Find(playerid).GetComponent<PlayerController>();
            //If Treasure hasnt been picked up yet
            if (!isPickedUp)
            {
                //Then it means it can be picked up and the Player will know
                canBePickedUp = true;
                //Tell the player that it can be picked up
                playerController.NotifyPickup(canBePickedUp);
                //Check if the Treasure can be picked up and if the player is trying to carry it
                if (playerController.carrying && canBePickedUp)
                {
                    //If the player is, then Attach the Treasure to the player
                    photonView.RPC("AttachToPlayer", RpcTarget.All, playerid);
                    //Treasure has been picked up
                    isPickedUp = true;
                }
            }
            else //If it has been picked up 
            {
                canBePickedUp = false;
                playerController.NotifyPickup(canBePickedUp);
                //Check if the player is still carrying and if they are not
                if (!playerController.carrying) 
                {
                    //Player Drops the Treasure
                    photonView.RPC("DetachFromPlayer", RpcTarget.All); 
                }
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player") 
        {
            //Get the Player ID of the object 
            int playerid = col.gameObject.GetComponent<PhotonView>().ViewID;
            //Get the Player Controller script from the player that has exited The Trigger
            PlayerController playerController = PhotonView.Find(playerid).GetComponent<PlayerController>();
         
            //Tell the player that it cannot pick up the Treasure
            playerController.NotifyPickup(false);
        }

    }
    [PunRPC]
    void AttachToPlayer(int idofplayer) 
    {
        //Disable the Gravity so it stays above the player
        rb.useGravity = false;
        //Stops the Game Object from floating away (Puts it in place)
        rb.isKinematic = true;
        //Set Collision to false
        parentObject.gameObject.GetComponent<BoxCollider>().enabled = false;
        //Gets The Transform of the Player Object
        Transform playerObject = PhotonView.Find(idofplayer).transform;
        //Set the parent GameObject to be the child GameObject of the player
        parentObject.gameObject.transform.parent = PhotonView.Find(idofplayer).transform;

        if (spell != null)
        {
            //Sets the position of The Treasure GameObject to be above the Player
            parentObject.gameObject.transform.localPosition = new Vector3(0.0590000004f, 1.57000005f, 0.437000006f);
            //Make sure the Treasure GameObject has the correct Rotation values and is not tilted
            parentObject.transform.localEulerAngles = new Vector3(0, 180, 0);
            Debug.Log("Spellcaster is holding box");
        }
        else if (karate != null)

        {
            //Sets the position of The Treasure GameObject to be above the Player
            parentObject.gameObject.transform.localPosition = new Vector3(-0.030000004f, 1.57900005f, 0.510000006f);
            //Make sure the Treasure GameObject has the correct Rotation values and is not tilted
            parentObject.transform.localEulerAngles = new Vector3(0, 180, 0);
            Debug.Log("Karatekid is holding box");
        }


    }
    [PunRPC]
    void DetachFromPlayer() 
    {
        //Becomes its own object again
        parentObject.gameObject.transform.parent = null;
        isPickedUp = false;
        //Put Gravity back on so that the Treasure falls down
        rb.useGravity = true;
        //Allows the Gravity to work for the treasure
        rb.isKinematic = false;
        //Set Collision to true
        parentObject.gameObject.GetComponent<BoxCollider>().enabled = true;
        Debug.Log("Dropped");
    }
}
