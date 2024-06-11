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

    //
    private Vector3 lastPosition;
    private float timeStanding;


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
    }

    public override void Initialize()
    {
        LookForResource();
    }

    // Buscar recurso para recolección
    public void LookForResource()
    {
        if (climaFavorable && !comidaCargada && !isDead) // Verificar si el clima es favorable, no está cargando comida, no está en peligro y no está muerta antes de buscar recursos
        {
            assignedResource = RequestResource(); // Solicitar un recurso para recoger (se eliminará de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                destino = assignedResource.transform.position;
            }
            else
            {
                InvokeRepeating("TryFindResource", 0, 1f); // Intentar encontrar un recurso cada segundo
            }
        }
    }

    // Intentar encontrar un recurso periódicamente
    private void TryFindResource()
    {
        if (assignedResource == null && climaFavorable && !comidaCargada && !isDead)
        {
            assignedResource = RequestResource();
            if (assignedResource != null)
            {
                CancelInvoke("TryFindResource"); // Detener la invocación periódica si se encuentra un recurso
                MoveTo(assignedResource.transform.position);
                destino = assignedResource.transform.position;
            }
        }
    }

    // Solicitar un recurso para recoger
    private GameObject RequestResource()
    {
        GameObject resource = map.RequestResource(); // Solicitar un recurso del mapa
        while (resource != null && resourcesInUse.Contains(resource))
        {
            resource = map.RequestResource(); // Seguir solicitando recursos hasta encontrar uno que no esté en uso
        }
        if (resource != null)
        {
            resourcesInUse.Add(resource); // Marcar el recurso como en uso
        }
        return resource;
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
        destino = (Vector3)currentExplorationTarget;
    }

    // Cuando llega al recurso
    public override void ArrivedAtResource(GameObject resource)
    {
        if (assignedResource == resource && !comidaCargada) // Si la hormiga colisiona con su recurso asignado y no tiene comida cargada
        {
            Destroy(resource); // Destruir el recurso
            //Cambiamos sprite
            ChangeSprite(spriteObj);
            resourcesInUse.Remove(resource); // Marcar el recurso como no en uso
            comidaCargada = true; // Marcar que tiene comida cargada
            MoveTo(storageRoom.transform.position); // Moverse hacia la sala de almacenamiento
            destino = storageRoom.transform.position;
        }
    }

    // Cuando llega a la sala
    public override void ArrivedAtRoom(Room room)
    {
        if (comidaCargada && room == storageRoom.GetComponent<Room>()) // Si tiene comida cargada y ha llegado a la sala de almacenamiento
        {
            storageRoom.GetComponent<Room>().Add(1); // Agregar comida a la sala de almacenamiento
            comidaCargada = false; // Marcar que ya no tiene comida cargada
            //Cambiamos sprite
            ChangeSprite(spriteOriginal);
            LookForResource(); // Buscar otro recurso
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
                assignedResource = null;
            }
            return; // No hacer nada más si la hormiga está muerta
        }

        if (!climaFavorable) // Si el clima no es favorable o está en peligro, esperar
        {
            return; // Salir de la actualización si está esperando
        }

        
        else
        {
            // Si no tiene un recurso asignado y el clima es favorable, buscar uno
            if (assignedResource == null && climaFavorable)
            {
                LookForResource();
            }
        }

        if (assignedResource == null && climaFavorable && !comidaCargada)
        {
            Explore(); // Explorar si no hay un recurso asignado y el clima es favorable
        }

        SpriteMove();
        
    }


    //Cambiar Sprite cuando tenga un recurso

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

    //Funcion Auxiliar para solucionar problema que a veces se produce y otras veces no
    private void FixRandomPositionError()
    {
        //A veces las recolectoras se rayan, yendo al mismo punto y quedandose bloqueadas
        //Este error no se produce siempre, solo a veces, y en cada compilacion puede darse en mas casos o en ninguno
        //Para ello, continuamente se comprobara si la hormiga permanece en la misma posicion durante cierto tiempo, de ser asi, se reiniciara la hormiga

        // Si la posición no ha cambiado
        if (Vector3.Distance(transform.position, lastPosition) < 0.001f)
        {
            timeStanding += Time.deltaTime;
            if (timeStanding >= 5f)
            {
                // Se cumple la condición de estar parada durante 5 segundos, reiniciaremos la hormiga
                isDead = true;
                antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Gatherer);
                base.Die();

                // reiniciar contador
                timeStanding = 0f;
                lastPosition = transform.position;
            }
        }
        else
        {
            // Si la posición ha cambiado, reiniciar el contador
            timeStanding = 0f;
            lastPosition = transform.position;
        }

    }
}