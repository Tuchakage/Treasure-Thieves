using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class KarateKid : MonoBehaviourPunCallbacks
{

    //Damage Calc
    public float dmg;
    //Timer for the cool down
    public float timer;
    //The max cooldown value (Where the timer will start counting down from
    public float cooldown;

    [SerializeField] Animator _playeranim;

    [SerializeField]
    bool pickUpTreasure = false; // Check if the Player can pick up the Treasure
    public bool carrying = false; // Player is carrying the Treasure, also used to notify the treasure that it is being carried

    // Start is called before the first frame update
    void Start()
    {
        //Grab Player Animator
        _playeranim = GetComponent<Animator>();
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

            if (Input.GetMouseButton(1))
            {
                //If there is no cooldown (Its at 0) then player can use the basic attack
                if (timer <= 0 && carrying == false)
                {
                    //Shoots out the lightning bolt
                    _playeranim.SetTrigger("Attack1");
                    //Add cooldown to the basic attack
                    timer = cooldown;
                }
            }


            //If the player can pickup the treasure
            if (pickUpTreasure)
            {
                //Start Carrying the Treasure
                carrying = true;
                //Cannot pick up the treasure again because its already holding it
                pickUpTreasure = false;
                Debug.Log("Pickup Treasure SpellCASTER");
            }

        }
    }

    void Attack()
    {

    }

    void DisableAttack()
    {

    }
}
