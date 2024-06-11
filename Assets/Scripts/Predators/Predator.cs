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

    //Sprite
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    //
    // Velocidad de movimiento
    public float velocidad = 5f;

    // Punto destino para moverse
    public Vector3 destino;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;

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
            destino = antTarget.transform.position;
        }
        
        else
        {
            antTarget = null;
            CheckPosition(); //comprobamos que haya llegado a la posicion para actualizarla
            MoveTo(randomPos);
            destino = randomPos;
        }

        // Mover hacia el destino
        if (destino != null)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            

            // Rotar el sprite hacia la dirección de movimiento
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angulo));

        }

        SpriteMove();
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
            if (antTarget.GetComponent<Ant>().EnterCombat()) // Enter combat. If predator wins (true)
            {
                Debug.Log("Predator won.");
                antTarget.GetComponent<AntGatherer>().isDead = false; //la matamos para que libere el recurso asignado en caso de tenerlo
                antTarget.GetComponent<Ant>().Die(); // Kill ant.
                predatorManager.GeneratePredatorAtSpawn(); // Spawn predator.
                antTarget = null;
                hungry = 100;
            } 
            else // If predator looses (false)
            {
                Debug.Log("Ant won");
                predatorManager.KillPredator(this);
            }

            

        }

        //BUG => A veces se atascan dos depredadores cuando tienen direcciones opuestas, en caso de que ninguno este persiguiendo a una hormiga, solucionaremos
        if (collision.gameObject.CompareTag("Predator") && inVisionRange == false) //comparamos solo con el actual, porque el otro se comparara en su propio script
        {
            randomPos = map.RandomPositionInsideBounds(); //nueva posicion random
        }
    }


    private void SpriteMove()
    {
        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
}
