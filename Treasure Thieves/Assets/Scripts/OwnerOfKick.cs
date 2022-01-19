using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OwnerOfKick : MonoBehaviourPunCallbacks, IPunObservable
{

    public int owner;


    //This function allows the variables inside to be sent over the network
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player so send the other computers the data
            stream.SendNext(owner);
        }
        else
        {
            //Network player that receives the data
            owner = (int)stream.ReceiveNext();

        }
    }
    //Sets the owner of the Spell
    public void SetOwner(int ownerofkick)
    {
        owner = ownerofkick;
    }

    //When called Returns the Owner Of The Spell
    public int GetOwner()
    {
        return owner;
    }


}