﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerController : MonoBehaviourPun
{
    // Link: https://canvas.kingston.ac.uk/courses/19811/pages/pun-guided-programming-part-3-multiplayer-movement
    public float turnSpeed = 180;
    public float tiltSpeed = 180;
    public float walkSpeed = 1;

    [SerializeField]
    private Transform fpcam;    // first person camera
    [SerializeField]
    private Camera topcam; // top view camera

    [SerializeField]
    TextMesh nickname;

    [SerializeField]
    bool pickUpTreasure = false; // Check if the Player can pick up the Treasure
    public bool carrying = false; // Player is carrying the Treasure, also used to notify the treasure that it is being carried

    // Start is called before the first frame update
    void Start()
    {
         Cursor.lockState = CursorLockMode.Confined;
        //photonView.IsMine - It only gets your client and only i can control it
        //If your player is there and your fp camera is there
        if (photonView.IsMine && fpcam != null)
        {
            //Find the Top View Camera
            topcam = Camera.main;
            //Disable the top view camera
            //topcam.enabled = false;
            //Gets the Camera object and then enable the Camera component
            fpcam.GetComponent<Camera>().enabled = true;
            nickname.text = " ";
        }

        else  //If its not my client and its another player 
        {
            //Fp cam wont get enabled for the other players (Camera only sees my POV and not other players)
            //fpcam.GetComponent<Camera>().enabled = false;
            //Gets the other players nickname
            nickname.text = photonView.Owner.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Makes sure i am controlling my own player
        if (photonView.IsMine)
        {
            float forward = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal") + Input.GetAxis("Mouse X");
            float tilt = Input.GetAxis("Mouse Y");
            transform.Translate(new Vector3(0, 0, forward * walkSpeed * Time.deltaTime));
            transform.Rotate(new Vector3(0, turn * turnSpeed * Time.deltaTime, 0));
            if (fpcam != null)
                fpcam.Rotate(new Vector3(-tilt * tiltSpeed * Time.deltaTime, 0));


            if (Input.GetKeyDown(KeyCode.F))
            {
                //If the player can pickup the treasure
                if (pickUpTreasure)
                {
                    //Start Carrying the Treasure
                    carrying = true;
                    //Cannot pick up the treasure again because its already holding it
                    pickUpTreasure = false;
                    Debug.Log("Pickup Treasure");
                }
            }
            else if (Input.GetKeyUp(KeyCode.F)) 
            {
                //If the Treasure cannot be picked up yet and you try to pick something up make sure the carrying variable is false so that the player cant just walk and pickup the Treasure
                if (!pickUpTreasure && !carrying) 
                {
                    //Make sure the player cant pick it up
                    carrying = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                carrying = false;
                //Debug.Log("Drop");
            }
        }




        if (Camera.current != null) 
        {
            //nicknames of other players are always facing towards me
            nickname.transform.LookAt(Camera.current.transform);
            nickname.transform.Rotate(0, 180, 0);
        }
    }

    //The Treasure Trigger will tell the player if it can be picked up
    public void NotifyPickup(bool canyou) 
    {

        pickUpTreasure = canyou;
        if (canyou)
        {
            Debug.Log("You can pick up Treasure");
        }
        else 
        {
            Debug.Log("You cannot pick up Treasure");
        }
    }
}
