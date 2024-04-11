using System.Collections;
using System.Collections.Generic;
using BehaviourAPI.UnityToolkit;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Predator : MovableObject
{

    public PredatorManager predatorManager;
    public GameObject antTarget;
    public bool inVisionRange;

    public Map map;
    public GameObject exterior;

    public Vector2 randomPos;

    private void Start()
    {
        inVisionRange = false;
        randomPos = map.RandomPositionInsideBounds();
    }

    private void Update()
    {

        if (inVisionRange)
        {
            
            MoveTo(antTarget.transform.position);

        }
        
        else
        {
            antTarget = null;
            CheckPosition(); //comprobamos que haya llegado a la posicion para actualizarla
            MoveTo(randomPos);
        }


    }

    // COMBAT.
    
    public void GetStunned()
    {
        Stop();
    }

    private void CheckPosition()
    {
        //guardamos posicion del predator
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        Vector2 currentPos = new Vector2(positionX, positionY);

        if (currentPos == randomPos) //comprobamos que haya llegado a la posicion para actualizarla
        {
            randomPos = map.RandomPositionInsideBounds(); //actualizamos el randomPos
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
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
    }*/
    
}
