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

    private static List<GameObject> resourcesInUse = new List<GameObject>(); // Recursos que están siendo recogidos por recolectoras
    private Vector3? currentExplorationTarget = null;

    // Referencia al nuevo sprite
    public Sprite spriteObj;
    public Sprite spriteOriginal;
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Punto destino para moverse
    public Vector3 destino;

    // Ángulo de corrección para alinear correctamente el sprite
    private float anguloCorreccion;

    //Posición
    private Vector3 lastPosition;
    private float timeStanding;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;

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

        // Iniciar la búsqueda de recursos
        StartCoroutine(LookForResourceCor());
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
        // Iniciar la búsqueda de recursos
        StartCoroutine(LookForResourceCor());
    }

    IEnumerator LookForResourceCor()
    {
        while (assignedResource == null && climaFavorable && !comidaCargada && !isDead)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger (se elimina de la lista de disponibles)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                destino = assignedResource.transform.position;
                StartCoroutine(Bark("Buscando comida"));
                Debug.Log("Lo tengo"); // Mensaje de confirmación de que se ha encontrado un recurso
                yield break; // Salir de la corrutina cuando se encuentra un recurso
            }

            yield return new WaitForSeconds(1f); // Esperar 1 segundo antes de volver a intentar
        }

        // Si no se encontró ningún recurso, se puede seguir buscando o explorar
        if (assignedResource == null && climaFavorable && !comidaCargada && !isDead)
        {
            StartCoroutine(LookForResourceCor()); // Volver a intentar buscar un recurso
        }
    }

    // Explorar cuando no haya recursos asignados
    private void Explore()
    {
        if (currentExplorationTarget == null || Vector3.Distance(transform.position, (Vector3)currentExplorationTarget) < 1.0f)
        {
            // Elegir un nuevo destino aleatorio para explorar
            currentExplorationTarget = new Vector3(
                transform.position.x + Random.Range(-10f, 10f),
                transform.position.y,
                transform.position.z + Random.Range(-10f, 10f)
            );
        }

        MoveTo((Vector3)currentExplorationTarget);
        Debug.Log("Explorando");
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

            return; // No hacer nada más si la hormiga está muerta
        }

        if (!climaFavorable) // Si el clima no es favorable o está en peligro, esperar
        {
            return; // Salir de la actualización si está esperando
        }


        else if (assignedResource == null && climaFavorable && !comidaCargada)
        {
            StartCoroutine(LookForResourceCor()); // Buscar un recurso si no tiene ninguno asignado y el clima es favorable
        }
        else if (assignedResource != null && climaFavorable && !comidaCargada)
        {
            // Moverse hacia el recurso asignado si lo tiene
            MoveTo(assignedResource.transform.position);
        }
        else if (assignedResource == null && climaFavorable && comidaCargada)
        {
            // Si tiene comida cargada y no tiene asignado un recurso, ir a la sala de almacenamiento
            MoveTo(storageRoom.transform.position);
        }
        else if (assignedResource == null && climaFavorable && !comidaCargada)
        {
            // Explorar si no hay recursos asignados, el clima es favorable y no está en peligro
            Explore();
        }

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
}