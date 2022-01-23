using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//Code is not used was originally going to implement basic ai
public class BasicEnemyAI : MonoBehaviour
{
	public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
	public float idleSpeed = 0f;
	public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.

	private UnityEngine.AI.NavMeshAgent nav;                                // Reference to the nav mesh agent.
	private Transform player;                               // Reference to the player's transform.

	private Transform startingPosition;
	private Transform currentPosition;

	private float patrolTimer;                              // A timer for the patrolWaitTime.
	private int wayPointIndex;                              // A counter for the way point array.

	public string text;

	enum AIState { Idle, Patrolling, Shooting, Chasing };

	AIState state;

	Text messagetext;

	void Awake()
	{
		// Setting up the references.
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;

		state = AIState.Idle;

		messagetext = GameObject.Find("MessageText").GetComponent<Text>();
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
			state = AIState.Idle;

		if (Input.GetKeyDown(KeyCode.C))
			state = AIState.Chasing;

		if (Input.GetKeyDown(KeyCode.P))
			state = AIState.Patrolling;

		switch (state)
		{
			case AIState.Chasing:
				Chasing();
				break;
			case AIState.Patrolling:
				Patrolling();
				break;
			case AIState.Idle:
				Idle();
				break;
		}
	}


	void Shooting()
	{
		messagetext.text = "Shooting:";

		// Stop the enemy where it is.
		nav.isStopped = true;
	}

	void Chasing()
	{
		messagetext.text = "Chasing: Nav rem dist= " + nav.remainingDistance + " navstop=" + nav.stoppingDistance;

		nav.destination = player.transform.position;
		nav.speed = chaseSpeed;
		nav.isStopped = false;
	}


	void Patrolling()
	{
		nav.isStopped = false;
		text = "Nav rem dist= " + nav.remainingDistance + " navstop=" + nav.stoppingDistance + " wp = " + wayPointIndex + "dest " + patrolWayPoints[wayPointIndex].position;

		messagetext.text = "Patrolling: " + text;

		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;

		// If near the next waypoint or there is no destination...
		if (nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			patrolTimer += Time.deltaTime;

			// If the timer exceeds the wait time...
			if (patrolTimer >= patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				if (wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;

				// Reset the timer.
				patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			patrolTimer = 0;

		// Set the destination to the patrolWayPoint.
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}


	void Idle()
    {
		if (currentPosition != startingPosition)
		{
			nav.isStopped = false;
			nav.destination = startingPosition.position;
			nav.speed = chaseSpeed;
		}
		else {
			nav.isStopped = false;
			nav.speed = idleSpeed;
		}

    }

}
