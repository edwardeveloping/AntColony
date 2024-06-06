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

    public float hungry;

    public Map map;
    public GameObject exterior;

    public Vector2 randomPos;

    private void Start()
    {
        inVisionRange = false;
        randomPos = map.RandomPositionInsideBounds();
        hungry = 100;
    }

    private void Update()
    {
        //Hambre
        hungry -= Time.deltaTime * 8; //Se muere desde predators manager


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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ant"))
        {
            /*if (antTarget.GetComponent<Ant>().EnterCombat()) // Enter combat. If predator wins (true)
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
            }*/

            //eliminar recurso
            antTarget.GetComponent<AntGatherer>().isDead = false; //la matamos para que libere el recurso asignado en caso de tenerlo
            antTarget.GetComponent<Ant>().Die(); // Kill ant.
            predatorManager.GeneratePredatorAtSpawn(); // Spawn predator.
            antTarget = null;
            hungry = 100;

        }

        //BUG => A veces se atascan dos depredadores cuando tienen direcciones opuestas, en caso de que ninguno este persiguiendo a una hormiga, solucionaremos
        if (collision.gameObject.CompareTag("Predator") && inVisionRange == false) //comparamos solo con el actual, porque el otro se comparara en su propio script
        {
            randomPos = map.RandomPositionInsideBounds(); //nueva posicion random
        }
    }
    
}
