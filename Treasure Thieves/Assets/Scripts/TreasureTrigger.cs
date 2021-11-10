using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TreasureTrigger : MonoBehaviourPun
{
    bool isPickedUp; //Check if the Treasure has been picked up
    bool canBePickedUp; //Check if the treasure can be picked up
    GameObject parentObject;
    Rigidbody rb; // The Rigidbody of the Treasure Game Object
    // Start is called before the first frame update
    void Start()
    {
        //The parent Object of the trigger (The Treasure GameObject) is put in a variable
        parentObject = this.gameObject.transform.parent.gameObject;
        //Find the Rigidbody of the Treasure Game Object (Parent GameObject)
        rb = parentObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

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

    [PunRPC]
    void AttachToPlayer(int idofplayer) 
    {
        //Disable the Gravity so it stays above the player
        rb.useGravity = false;
        //Stops the Game Object from floating away (Puts it in place)
        rb.isKinematic = true;
        //Gets The Transform of the Player Object
        Transform playerObject = PhotonView.Find(idofplayer).transform;
        //Set the parent GameObject to be the child GameObject of the player
        parentObject.gameObject.transform.parent = PhotonView.Find(idofplayer).transform;
        //Sets the position of The Treasure GameObject to be above the Player
        parentObject.transform.position = new Vector3(playerObject.position.x, playerObject.position.y + 1.5f, playerObject.position.z);
        //Make sure the Treasure GameObject has the correct Rotation values and is not tilted
        parentObject.transform.eulerAngles = new Vector3(0, 0, 0);


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
        Debug.Log("Dropped");
    }
}
