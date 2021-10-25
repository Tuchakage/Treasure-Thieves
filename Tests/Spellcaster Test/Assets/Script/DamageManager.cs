using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("PlayerManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject col) 
    {
        //Player loses health
        gm.health -= 5;
        //The parent of the particle system object is destroyed
        Destroy(col.transform.parent.gameObject);
        Debug.Log("Touched " + col.gameObject.name );
    }
}
