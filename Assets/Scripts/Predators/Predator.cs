using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Predator : MovableObject
{

    public PredatorManager predatorManager;
    GameObject antTarget;

    private void Update()
    {
        if(antTarget != null)
        {
            MoveTo(antTarget.transform.position);
        }
    }

    // COMBAT.
    public void ChaseTarget(GameObject target)
    {
        antTarget = target;
    }
    public void GetStunned()
    {
        Stop();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == antTarget) // If the object it collisioned with is its target.
        {
            if (antTarget.GetComponent<Ant>().EnterCombat()) // Enter combat. If predator wins (true)
            {
                Debug.Log("Predator won.");
                antTarget.GetComponent<Ant>().Die(); // Kill ant.
                predatorManager.GeneratePredatorAtSpawn(); // Spawn predator.
                // Spawn another predator.
            } 
            else // If predator looses (false)
            {
                Debug.Log("Ant won");
                GetStunned(); // Gets stunned.
            }
            antTarget = null;
        }
    }
    
}
