using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyParticles : MonoBehaviourPun
{
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        //Find the particle systems in the child of this game object
        ps = this.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the particle system is not alive then destroy the game Object
        if (!ps.IsAlive()) 
        {
            photonView.RPC("DestroyObject", RpcTarget.All, this.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void DestroyObject(int go)
    {
        //Find the Id of the Game Object that needs to be destroyed
        Destroy(PhotonView.Find(go).gameObject);
    }
}
