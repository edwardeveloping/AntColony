using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Predator : MonoBehaviour
{
    NavMeshAgent agent;

    public PredatorManager predatorManager;
    GameObject antTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if(antTarget != null)
        {
            MoveTo(antTarget.transform.position);
            RotateTowardsTarget();
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

    // MOVEMENT.
    public void MoveTo(float x, float y)
    {
        agent.isStopped = false;
        agent.SetDestination(new Vector3(x, y, 0));
    }
    public void MoveTo(Vector2 pos)
    {
        agent.isStopped = false;
        agent.SetDestination(pos);
    }
    public void Stop()
    {
        agent.isStopped = true;
    }
    private void RotateTowardsTarget()
    {
        if (antTarget != null)
        {
            // Calculamos la dirección hacia el objetivo en el plano XY
            Vector3 direction = antTarget.transform.position - transform.position;
            direction.z = 0; // Mantenemos la dirección en el plano XY

            // Si la dirección tiene alguna magnitud, rotamos hacia esa dirección
            if (direction.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}
