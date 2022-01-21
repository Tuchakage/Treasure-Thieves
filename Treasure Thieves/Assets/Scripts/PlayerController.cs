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
    public Animator _playeranim; // Player Animation Referrence

    [Header("GroundSettings")] 
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _groundDrag = 6f; //Player ground Drag

    [Header("OtherSettings")]
    [SerializeField]
    public Transform fpcam;    // first person camera
    [SerializeField]
    TextMesh nickname;


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
            //Player Animation Parameter
            _playeranim.SetFloat("Speed", Mathf.Abs(_moveDir.x));
        }
        
        if (Camera.current != null) 
        {
            //nicknames of other players are always facing towards me
            nickname.transform.LookAt(Camera.current.transform);
            nickname.transform.Rotate(0, 180, 0);
        }



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


    public void FootL()
    {

    }

    public void FootR()
    {

    }    
}
