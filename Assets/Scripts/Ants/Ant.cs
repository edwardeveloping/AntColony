using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public abstract class Ant : MonoBehaviour
{
    NavMeshAgent agent;
    public AntManager antManager;

    public Transform refugeZone; // Location to flee when in danger.
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // BASICS.
    public abstract void Initialize();

    // MOVEMENT.
    public void MoveTo(float x, float y)
    {
        agent.isStopped = false;
        agent.SetDestination(new Vector3(x,y,0));
    }
    public void MoveTo(Vector3 pos)
    {
        agent.isStopped = false;
        agent.SetDestination(pos);
    }
    public void Stop()
    {
        agent.isStopped = true;
    }

    // COMBAT.

    public bool EnterCombat()
    {
        Stop();
        
        if (Random.Range(0, 2) == 0) // 50% probability.
        {
            // if == 0 preedator wins.
            return true;
        }
        // if == 1 ant wins.

        Flee();
        return false;
    }

    public void Flee() 
    {
        MoveTo(refugeZone.position);
    }

    public void Die()
    {
        antManager.KillAnt(this);
    }

    // UTILITIES.
    public abstract void ArrivedAtRoom(Room room);
    public abstract void ArrivedAtResource(GameObject resource);
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Resource>() != null) // Collision with a resource.
        {
            ArrivedAtResource(collision.gameObject);
        }
        else if(collision.GetComponent<Room>() != null) // Collision with a room area.
        {
            Room room = collision.GetComponent<Room>();
            ArrivedAtRoom(room);
        }
    }
}
