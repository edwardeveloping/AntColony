using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourAPI.UnityToolkit;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Predator : MovableObject
{
    [SerializeField] public PredatorManager predatorManager;
    public GameObject antTarget;

    public float hungry;

    public Map map;
    public GameObject exterior;

    public Vector2 randomPos;

    //Sprite
    private float flipTime;
    private float flipTimeActual;
    private Sprite originalSprite;
    public Sprite spriteWithAnt;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    //
    // Velocidad de movimiento
    public float velocidad = 10f;

    // Punto destino para moverse
    public Vector3 destino;

    // Ángulo de corrección para alinear correctamente el sprite
    private float anguloCorreccion;

    //
    public bool outSideBounds = false;
    public bool confuse = false;
    public bool reproduce = false;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;
    public bool inAntHill;

    // Evento que se dispara cuando una avispa mata a una hormiga recolectora
    public static event Action OnAntGathererKilled;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        antTarget = null;
        randomPos = map.RandomPositionInsideBounds();
        hungry = 500f;
        originalSprite = spriteRenderer.sprite;

        //cambiar desde start
        anguloCorreccion = -90f;
        originalColor = spriteRenderer.color;

        NavMeshAgent navMeshAgent = this.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 8f;
        barkPanel.gameObject.SetActive(false);

        inAntHill = false;
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

    private void CheckAnthillPosition()
    {
        //guardamos posicion del predator
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        Vector2 currentPos = new Vector2(positionX, positionY);

        if (currentPos == randomPos) //comprobamos que haya llegado a la posicion para actualizarla
        {
            randomPos = map.RandomPositionInsideAnthill(); //actualizamos el randomPos
        }

        
    }

    private bool CheckNestPosition()
    {
        //guardamos posicion del predator
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        Vector3 currentPos = new Vector3(positionX, positionY, 0);

        if (currentPos == predatorManager.GetComponent<PredatorManager>().predatorSpawn.position) //comprobamos que haya llegado a la posicion para actualizarla
        {
            return true;
        }

        return false;
    }
    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "Va a morir de hambre":
                barkPanel.sprite = barkList[0];
                break;
            

        }

        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        //Hambre
        hungry -= Time.deltaTime * 8; //Se muere desde predators manager

        

        if (predatorManager.GetComponent<PredatorManager>().colony.GetComponent<Colony>().inDanger)
        {
            outSideBounds = false;
            if (antTarget != null)
            {
                MoveTo(antTarget.transform.position);
                destino = antTarget.transform.position;
            }

            else
            {
                antTarget = null;
                //randomPos = map.RandomPositionInsideAnthill();
                CheckAnthillPosition(); //comprobamos que haya llegado a la posicion para actualizarla
                MoveTo(randomPos);
                destino = randomPos;
            }
        }

        else
        {
            if (!confuse && !reproduce)
            {
                if (!outSideBounds)
                {
                    if (antTarget != null)
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
                }

                else
                {
                    antTarget = null;
                    CheckPosition(); //comprobamos que haya llegado a la posicion para actualizarla
                    MoveTo(randomPos);
                    destino = randomPos;
                }
            }

            else if (!confuse && reproduce)
            {
                //comprobamos que llegue al nido
                MoveTo(predatorManager.GetComponent<PredatorManager>().predatorSpawn.position);
                destino = predatorManager.GetComponent<PredatorManager>().predatorSpawn.position;
                if (CheckNestPosition())
                {
                    predatorManager.GenerateLarvaPredator();
                    spriteRenderer.sprite = originalSprite;
                    reproduce = false;
                }
            }

            else if (confuse && !reproduce)
            {
                this.transform.position = this.transform.position;
            }
        }

        /*if (hungry < 50)
        {
            StartCoroutine(Bark("Va a morir de hambre"));
        }*/

        if (hungry < 0)
        {
            predatorManager.KillPredator(this);
        }

        SpriteMove();
    }


    private IEnumerator Confuse()
    {
        Color redColor = Color.red;
        Color color = redColor; //Auxiliar


        // 5 segundos de cooldown
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.2f);

            if (color == redColor)
            {
                color = originalColor;
            }
            else
            {
                color = redColor;
            }

            spriteRenderer.color = color;
        }

        yield return new WaitForSeconds(0.5f);
        confuse = false;
        spriteRenderer.color = originalColor;
    }


    private void SpriteMove()
    {
        // Mover hacia el destino
        if (destino != null)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                // Rotar el sprite hacia la dirección de movimiento
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

                // Añadir el ángulo de corrección
                angulo += anguloCorreccion;

                // Ajustar la rotación del sprite para que solo cambie en el plano 2D
                transform.rotation = Quaternion.Euler(0, 0, angulo);

            }

        }

        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == antTarget && !confuse)
        {
            if (antTarget.GetComponent<Ant>().EnterCombat()) // Enter combat. If predator wins (true)
            {
                //Debug.Log("Predator won.");

                antTarget.GetComponent<AntGatherer>().isDead = true; //la matamos para que libere el recurso asignado en caso de tenerlo
                OnAntGathererKilled?.Invoke(); // Llamar al evento cuando una hormiga recolectora es asesinada

                //cambiamos sprite

                spriteRenderer.sprite = spriteWithAnt;

                reproduce = true;
                hungry = 100;
                antTarget = null;
            }
            else // If predator looses (false)
            {
                //Debug.Log("Ant won");
                confuse = true;
                StartCoroutine(Confuse());
            }

        }

        //BUG => A veces se atascan dos depredadores cuando tienen direcciones opuestas, en caso de que ninguno este persiguiendo a una hormiga, solucionaremos
        if (collision.gameObject.CompareTag("Predator") && antTarget == null && !confuse) //comparamos solo con el actual, porque el otro se comparara en su propio script
        {
            randomPos = map.RandomPositionInsideBounds(); //nueva posicion random
            MoveTo(randomPos);
            destino = randomPos;
        }

        //BUG => Escarabajo
        if (collision.gameObject.CompareTag("Beetle") && antTarget == null && !confuse) //comparamos solo con el actual, porque el otro se comparara en su propio script
        {
            randomPos = map.RandomPositionInsideBounds(); //nueva posicion random
            MoveTo(randomPos);
            destino = randomPos;
        }
    }


}
