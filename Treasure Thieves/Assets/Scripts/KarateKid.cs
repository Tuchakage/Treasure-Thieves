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
    public float cooldown = 5.0f;

    //Box Collider For Attack Trigger
    [SerializeField] BoxCollider _attackbox;

    [SerializeField] Animator _playeranim;

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

            if (Input.GetMouseButton(1) && !this.gameObject.GetComponent<PlayerController>().carrying)
            {
                //If there is no cooldown (Its at 0) then player can use the basic attack
                if (timer <= 0)
                {
                    //Shoots out the lightning bolt
                    _playeranim.SetTrigger("Attack1");
                    //Add cooldown to the basic attack
                    timer = cooldown;
                }
            }

        }
    }

    void Attack()
    {
        _attackbox.enabled = true;
    }

    void DisableAttack()
    {
        _attackbox.enabled = false;
    }

    void FootL()
    {

    }

    void FootR()
    {

    }
}
