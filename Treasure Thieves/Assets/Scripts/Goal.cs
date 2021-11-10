using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//The Purpose of this script is to determine which of the players gets the points
public class Goal : MonoBehaviourPun
{
    NetworkManager nm;
    // Start is called before the first frame update
    void Start()
    {
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        //If the Treasure is dropped into the Blue Goal
        if (this.gameObject.name == "Blue Goal Trigger" && col.gameObject.tag == "Treasure")
        {
            //Blue Team will gain a point
            photonView.RPC("IncreaseBlueScore", RpcTarget.All);
            Debug.Log(col.gameObject.GetComponent<PhotonView>().ViewID);
            //Destroy The Treasure Game Object For Everyone
            photonView.RPC("DestroyObject", RpcTarget.All, col.gameObject.GetComponent<PhotonView>().ViewID);

        }
        else if (this.gameObject.name == "Red Goal Trigger" && col.gameObject.tag == "Treasure") // The treasure is dropped into the Red Goal
        {
            //red Team will gain a point
            photonView.RPC("IncreaseRedScore", RpcTarget.All);
            //Destroy The Treasure Game Object For Everyone
            Debug.Log(col.gameObject.GetComponent<PhotonView>().ViewID);
            photonView.RPC("DestroyObject", RpcTarget.All, col.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void IncreaseBlueScore()
    {
        //Increases the score of the Blue Team
        nm.bluescore++;
    }

    [PunRPC]
    void IncreaseRedScore()
    {
        //Increases the score of the Red Team
        nm.redscore++;
    }

    [PunRPC]
    void DestroyObject(int go)
    {
        //Find the Id of the Game Object that needs to be destroyed
        Destroy(PhotonView.Find(go).gameObject);
    }

}
