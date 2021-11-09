using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TreasureTrigger : MonoBehaviourPun
{
    bool isPickedUp; //Check if the Treasure has been picked up
    bool canBePickedUp; //Check if the treasure can be picked up

    // Start is called before the first frame update
    void Start()
    {
  
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
                //Check if the player is trying to carry it
                if (playerController.carrying)
                {
                    //If the player is then Attach the Treasure to the player
                    photonView.RPC("AttachToPlayer", RpcTarget.All, playerid);
                    //Treasure has been picked up
                    isPickedUp = true;
                }



            }
            else //If it has been picked up 
            {
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
        //The parent of the trigger (The Cube GameObject) will become a child of the player GameObject
        this.gameObject.transform.parent.gameObject.transform.parent = PhotonView.Find(idofplayer).transform;
    }
    [PunRPC]
    void DetachFromPlayer() 
    {
        //Becomes its own object again
        this.gameObject.transform.parent.gameObject.transform.parent = null;
        isPickedUp = false;
        Debug.Log("Dropped");
    }
}
