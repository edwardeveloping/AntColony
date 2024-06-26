using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntGatherer : Ant
{
    public Map map;
    public GameObject storageRoom;

    public GameObject assignedResource;

    public bool comidaCargada = false;
    public bool climaFavorable = true; // Variable para representar si el clima es favorable
    public bool isDead = false;

    private static List<GameObject> resourcesInUse = new List<GameObject>(); // Recursos que est�n siendo recogidos por recolectoras
    private Vector3? currentExplorationTarget = null;

    // Referencia al nuevo sprite
    public Sprite spriteObj;
    public Sprite spriteOriginal;
    private float flipTime;
    private float flipTimeActual;

    private Vector2 explorePosition;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Punto destino para moverse
    public Vector3 destino;

    // �ngulo de correcci�n para alinear correctamente el sprite
    private float anguloCorreccion;

    //Posici�n
    private Vector3 lastPosition;
    private float timeStanding;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;

    //Idle
    public bool isIdle;
    private bool startIdle;
    private Vector3 previousPosition;
    private float idleTime;
    private float idleThreshold = 5.0f; // Tiempo en segundos para considerar la hormiga como ociosa

    private bool exploration;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;

        // Guardar el sprite original
        if (spriteRenderer != null)
        {
            spriteOriginal = spriteRenderer.sprite;
        }

        anguloCorreccion = -90f;

        //
        lastPosition = transform.position;
        timeStanding = 0f;

        previousPosition = transform.position;
        idleTime = 0f;
        idleThreshold = 10f;

        exploration = false;
        explorePosition = map.RandomPositionInsideAnthill();

        // Iniciar la b�squeda de recursos
        StartCoroutine(LookForResourceCor());
    }

    private void CheckIdleStatus()
    {
        if (startIdle)
        {
            idleTime += Time.deltaTime;
            if (idleTime >= idleThreshold)
            {
                isIdle = true;
            }
        }
        else
        {
            isIdle = false;
            idleTime = 0f;
            previousPosition = transform.position;
        }
    }

    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "LLevando comida a storageRoom":
                barkPanel.sprite = barkList[1];
                break;
            case "Buscando comida":
                barkPanel.sprite = barkList[0];
                break;

        }

        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
    }

    public override void Initialize()
    {
        // Iniciar la b�squeda de recursos
        StartCoroutine(LookForResourceCor());
    }

    IEnumerator LookForResourceCor()
    {
        while (assignedResource == null && climaFavorable && !comidaCargada && !isDead)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger (se elimina de la lista de disponibles)
            if (assignedResource != null)
            {
                startIdle = false;
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                destino = assignedResource.transform.position;
                StartCoroutine(Bark("Buscando comida"));
                Debug.Log("Lo tengo"); // Mensaje de confirmaci�n de que se ha encontrado un recurso
                yield break; // Salir de la corrutina cuando se encuentra un recurso
            }
            else
            {
                startIdle = true;
                Explore();
            }

            yield return new WaitForSeconds(1f); // Esperar 1 segundo antes de volver a intentar
        }

        // Si no se encontr� ning�n recurso, se puede seguir buscando o explorar
        if (assignedResource == null && climaFavorable && !comidaCargada && !isDead)
        {
            StartCoroutine(LookForResourceCor()); // Volver a intentar buscar un recurso
        }
    }

    // Explorar cuando no haya recursos asignados
    private void Explore()
    {
        StopCoroutine(LookForResourceCor());
        exploration = true;
    }

    private void CheckPosition()
    {
        //guardamos posicion del predator
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        Vector2 currentPos = new Vector2(positionX, positionY);

        if (currentPos == explorePosition) //comprobamos que haya llegado a la posicion para actualizarla
        {
            exploration = false;
            //recalculamos posicion nueva
            explorePosition = map.RandomPositionInsideAnthill();
            // Iniciar la b�squeda de recursos
            StartCoroutine(LookForResourceCor());
        }
    }

    // Cuando llega al recurso
    public override void ArrivedAtResource(GameObject resource)
    {
        if (assignedResource == resource && !comidaCargada) // Si la hormiga llega al recurso asignado y no tiene comida cargada
        {
            Destroy(resource); // Destruir el recurso

            // Cambiar sprite, si es necesario
            ChangeSprite(spriteObj);

            resourcesInUse.Remove(resource); // Marcar el recurso como no en uso
            comidaCargada = true; // Marcar que tiene comida cargada
            MoveTo(storageRoom.transform.position); // Moverse hacia la sala de almacenamiento
            destino = storageRoom.transform.position;
            StartCoroutine(Bark("LLevando comida a storageRoom"));
            Debug.Log("Lo cargo");
        }
    }

    // Cuando llega a la sala
    public override void ArrivedAtRoom(Room room)
    {
        if (comidaCargada && room == storageRoom.GetComponent<Room>()) // Si tiene comida cargada y ha llegado a la sala de almacenamiento
        {
            storageRoom.GetComponent<Room>().Add(1); // Agregar comida a la sala de almacenamiento
            comidaCargada = false; // Marcar que ya no tiene comida cargada

            // Cambiar sprite, si es necesario
            ChangeSprite(spriteOriginal);

            assignedResource = null; // Marcar el recurso actual como null para buscar uno nuevo
            StartCoroutine(LookForResourceCor()); // Volver a buscar otro recurso
        }
    }

    // Cuando gana un combate
    public override void WhenCombatWon()
    {
        Flee(); // Huir
    }

    // Actualizar
    void Update()
    {
        if (isDead)
        {
            if (assignedResource != null)
            {
                resourcesInUse.Remove(assignedResource); // Marcar el recurso como no en uso si la recolectora muere
                map.GetComponent<Map>().unasignedResources.Add(assignedResource); //Volver a meterlo en el map
                assignedResource = null;

            }
            base.Die();

            return; // No hacer nada m�s si la hormiga est� muerta
        }

        if (exploration)
        {
            CheckPosition();
            MoveTo(explorePosition);
            destino=explorePosition;
        }

        if (!climaFavorable && assignedResource != null) // Si el clima no es favorable o est� en peligro, esperar
        {
            Vector3 posicion = new Vector3(0, -5, 0);
            MoveTo(posicion); //se mueven a donde spawnean
            destino = posicion;
        }


        else if (assignedResource == null && climaFavorable && !comidaCargada)
        {
            StartCoroutine(LookForResourceCor()); // Buscar un recurso si no tiene ninguno asignado y el clima es favorable
        }
        else if (assignedResource != null && climaFavorable && !comidaCargada)
        {
            // Moverse hacia el recurso asignado si lo tiene
            MoveTo(assignedResource.transform.position);
            destino = assignedResource.transform.position;
        }
        else if (assignedResource == null && climaFavorable && comidaCargada)
        {
            // Si tiene comida cargada y no tiene asignado un recurso, ir a la sala de almacenamiento
            MoveTo(storageRoom.transform.position);
            destino = storageRoom.transform.position;
        }

        CheckIdleStatus(); //Comprobar si hay idle
        SpriteMove(); // Actualizar el movimiento del sprite
    }

    // Cambiar Sprite cuando tenga un recurso
    private void ChangeSprite(Sprite sprite)
    {
        // Cambiar el sprite del SpriteRenderer
        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    private void SpriteMove()
    {
        // Mover hacia el destino
        if (destino != null)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                // Rotar el sprite hacia la direcci�n de movimiento
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

                // A�adir el �ngulo de correcci�n
                angulo += anguloCorreccion;

                // Ajustar la rotaci�n del sprite para que solo cambie en el plano 2D
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
}