using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TreasureTrigger : MonoBehaviourPun
{
    public bool isPickedUp; //Check if the Treasure has been picked up
    [SerializeField]
    bool canBePickedUp; //Check if the treasure can be picked up
    [SerializeField] private bool canBeThrown; // Check if the treasure can be thrown
    GameObject parentObject;
    Rigidbody rb; // The Rigidbody of the Treasure Game Object
    [SerializeField] private float thrownForce;

    [SerializeField] Spellcaster spell;
    [SerializeField] KarateKid karate;

    PhotonView playerpv; //The Players PhotonView Component
    public int playerid;
    bool inTrigger; //Indicates whether a Player is in the Trigger
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

        if (inTrigger) //If there is a player in the Trigger
        {
            if (playerpv.IsMine) //Check to see if it is the PLayer pressing the button
            {
                if (Input.GetKeyDown(KeyCode.F) && !isPickedUp && inTrigger) //If the F Key Is Pressed and the Treasure hasnt been picked up yet and there is still a player in the trigger
                {
                    // The Player will pick up the Treasure
                    photonView.RPC("AttachToPlayer", RpcTarget.All, playerid);
                    Debug.Log("Picked Up Treasure");
                    //Treasure Animation is true
                    playerpv.gameObject.GetComponent<PlayerController>()._playeranim.SetBool("Carrying", true);
                    //Set Speed to Slow
                    playerpv.gameObject.GetComponent<PlayerController>()._moveSpeed = playerpv.gameObject.GetComponent<PlayerController>()._moveSlowSpeed;

                }
                else if (Input.GetKeyDown(KeyCode.G) && isPickedUp) //If the G Key Is Pressed and the Treasure has been picked up
                {
                    //Player Drops the Treasure
                    photonView.RPC("DetachFromPlayer", RpcTarget.All);              
                    //Treasure will know it has been dropped
                    isPickedUp = false;
                    Debug.Log("Dropped Treasure");
                    playerpv.gameObject.GetComponent<PlayerController>()._playeranim.SetBool("Carrying", false);
                    playerpv.gameObject.GetComponent<PlayerController>()._moveSpeed = 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Space) && isPickedUp && canBeThrown) //If the Space Key is pressed and the Treasure has been picked up and the Treasure can be thrown
                {
                    photonView.RPC("ThrownFromPlayer", RpcTarget.All, playerid);
                    playerpv.gameObject.GetComponent<PlayerController>()._playeranim.SetBool("Carrying", false);
                    playerpv.gameObject.GetComponent<PlayerController>()._moveSpeed = 10f;
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player") //If a Player is in the trigger
        {
            //If a player is in the Trigger then set this variable to true
            inTrigger = true;

            playerpv = col.gameObject.GetComponent<PhotonView>();
            //Get the Player ID of the object 
            playerid = playerpv.ViewID;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player") 
        {
            //If a player has left the trigger
            inTrigger = false;
            //Get the Player ID of the object 
            int playerid = col.gameObject.GetComponent<PhotonView>().ViewID;
         
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

        //Sets the position of The Treasure GameObject to be above the Player
        parentObject.gameObject.transform.localPosition = new Vector3(0.0590000004f, 1.57000005f, 0.437000006f);
        //Make sure the Treasure GameObject has the correct Rotation values and is not tilted
        parentObject.transform.localEulerAngles = new Vector3(0, 180, 0);
        Debug.Log("Spellcaster is holding box");

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

        //Treasure will know it has been picked up
        isPickedUp = true;
        //Treasure can be thrown
        canBeThrown = true;

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

    [PunRPC]
    void ThrownFromPlayer(int IdOfPlayer)
    {
        PlayerController playerController = PhotonView.Find(IdOfPlayer).GetComponent<PlayerController>();
        DetachFromPlayer();
        rb.AddForce(playerController.fpcam.forward * thrownForce, ForceMode.Impulse);
        rb.AddForce(playerController.fpcam.up * thrownForce, ForceMode.Impulse);
        canBeThrown = false;

    }
}