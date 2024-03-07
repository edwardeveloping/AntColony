using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovableObject : MonoBehaviour
{
    NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // MOVEMENT.
    public void MoveTo(float x, float y)
    {
        MoveTo(new Vector3((float)x, (float)y));
    }
    public void MoveTo(Vector3 pos)
    {
        agent.isStopped = false;
        agent.SetDestination(pos);
        RotateTowardsTarget(pos);
    }
    private void RotateTowardsTarget(Vector3 target)
    {
        // Calculamos la dirección hacia el objetivo en el plano XY
        Vector3 direction = target - transform.position;
        direction.z = 0; // Mantenemos la dirección en el plano XY

        // Si la dirección tiene alguna magnitud, rotamos hacia esa dirección
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
    }
    public void Stop()
    {
        agent.isStopped = true;
    }

}
