using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviourPun
{
    // Link: https://canvas.kingston.ac.uk/courses/19811/pages/pun-guided-programming-part-3-multiplayer-movement
    //public float turnSpeed = 180;
    //public float tiltSpeed = 180;
    //public float walkSpeed = 1;

    [Header("PlayerMovement")] 
    public float _moveSpeed = 10f; //Player movement speed;
    public float _moveSlowSpeed = 3f; //Player movement speed;
    [SerializeField] private float _horizontalMovement;
    [SerializeField] private float _verticalMovement;
    [SerializeField] private float _movementMultiplier = 10f;
    [SerializeField] private Vector3 _moveDir; //Player move direction
    [SerializeField] private Rigidbody _playerRB; //Player Rigidbody
    [SerializeField] Animator _playeranim; // Player Animation Referrence

    [Header("GroundSettings")] 
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _groundDrag = 6f; //Player ground Drag

    [Header("OtherSettings")]
    [SerializeField]
    public Transform fpcam;    // first person camera
    [SerializeField]
    TextMesh nickname;
    [SerializeField]
    bool canBePickedUp = false; // Check if the Player can pick up the Treasure
    public bool canBeThrown = false;
    public bool carrying = false; // Player is carrying the Treasure, also used to notify the treasure that it is being carried

    // Start is called before the first frame update
    void Start()
    {

        //photonView.IsMine - It only gets your client and only i can control it
        //If your player is there and your fp camera is there
        if (photonView.IsMine && fpcam != null)
        {
            //Gets player Rigidbody
            _playerRB = GetComponent<Rigidbody>();
            fpcam.GetComponent<Camera>().enabled = true;
            nickname.text = " ";

            //Grab Player Animator
            _playeranim = GetComponent<Animator>();
        }

        else  //If its not my client and its another player 
        {
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
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight / 2 + 0.1f);

            PlayerInput();
            ControlDrag();
            
            //If the player can pickup the treasure
            if (canBePickedUp)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //Start Carrying the Treasure
                    carrying = true;
                    //Start Allowing the treasure to be thrown
                    canBeThrown = true;
                    //Treasure Animation is true
                    _playeranim.SetBool("Carrying", true);
                    //Set Speed to Slow
                    _moveSpeed = _moveSlowSpeed;
                    //Cannot pick up the treasure again because its already holding it
                    canBePickedUp = false;
                    Debug.Log("Pickup Treasure");
                }

            }
            //If the Treasure cannot be picked up yet and you try to pick something up make sure the carrying variable is false so that the player cant just walk and pickup the Treasure
            if (!canBePickedUp && !carrying)
            {
                //Make sure the player cant pick it up
                carrying = false;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                carrying = false;
                canBeThrown = false;
                _playeranim.SetBool("Carrying", false);
                _moveSpeed = 10f;
                //Debug.Log("Drop");
            }
            
            if (canBeThrown && Input.GetKeyDown(KeyCode.Space))
            {
                canBeThrown = true;
                _playeranim.SetBool("Carrying", false);
                _moveSpeed = 10f;
                Debug.Log("Thrown");
            }
        }
        
        if (Camera.current != null) 
        {
            //nicknames of other players are always facing towards me
            nickname.transform.LookAt(Camera.current.transform);
            nickname.transform.Rotate(0, 180, 0);
        }

        //Player Animation Parameter
        _playeranim.SetFloat("Speed", Mathf.Abs(_moveDir.x));

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    //Player Movement Input
    void PlayerInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");

        _moveDir = transform.forward * _verticalMovement + transform.right * _horizontalMovement;
    }

    //Move the player using force
    void MovePlayer()
    {
        if (_isGrounded)
        {
            _playerRB.AddForce(_movementMultiplier * _moveSpeed * _moveDir.normalized, ForceMode.Acceleration);
        }
    }

    //Adding drag to the player to not make them move as if they are floating
    void ControlDrag()
    {
        if (_isGrounded)
        {
            _playerRB.drag = _groundDrag;
        }
    }

    //The Treasure Trigger will tell the player if it can be picked up
    public void NotifyPickup(bool canyou) 
    {

        canBePickedUp = canyou;
        if (canyou)
        {
            Debug.Log("You can pick up Treasure");
        }
        else 
        {
            //Debug.Log("You cannot pick up Treasure");
        }
    }

    public void FootL()
    {

    }

    public void FootR()
    {

    }    
}
