using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Code is not used was originally going to implement basic ai nav mesh
//This script is not used in the final product
public class NavScript : MonoBehaviour
{
    public Transform target;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.Log("Target Error For Game Object " + this.name);
        }

        agent.SetDestination(target.position);
        
    }
}
