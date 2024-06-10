using System.Collections.Generic;
using UnityEngine;

public class AntGatherer : Ant
{
    public Map map;
    public GameObject storageRoom;
    public GameObject securityRoom;

    public GameObject assignedResource;

    public bool comidaCargada = false;
    public bool climaFavorable = true; // Variable para representar si el clima es favorable
    public bool inDanger = false;
    public bool isDead = false;

    private static List<GameObject> resourcesInUse = new List<GameObject>(); // Recursos que están siendo recogidos por recolectoras
    private Vector3? currentExplorationTarget = null;

    public override void Initialize()
    {
        LookForResource();
    }

    // Buscar recurso para recolección
    public void LookForResource()
    {
        if (climaFavorable && !comidaCargada && !inDanger && !isDead) // Verificar si el clima es favorable, no está cargando comida, no está en peligro y no está muerta antes de buscar recursos
        {
            assignedResource = RequestResource(); // Solicitar un recurso para recoger (se eliminará de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
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
        if (assignedResource == null && climaFavorable && !comidaCargada && !inDanger && !isDead)
        {
            assignedResource = RequestResource();
            if (assignedResource != null)
            {
                CancelInvoke("TryFindResource"); // Detener la invocación periódica si se encuentra un recurso
                MoveTo(assignedResource.transform.position);
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
    }

    // Cuando llega al recurso
    public override void ArrivedAtResource(GameObject resource)
    {
        if (assignedResource == resource && !comidaCargada) // Si la hormiga colisiona con su recurso asignado y no tiene comida cargada
        {
            Destroy(resource); // Destruir el recurso
            resourcesInUse.Remove(resource); // Marcar el recurso como no en uso
            comidaCargada = true; // Marcar que tiene comida cargada
            MoveTo(storageRoom.transform.position); // Moverse hacia la sala de almacenamiento
        }
    }

    // Cuando llega a la sala
    public override void ArrivedAtRoom(Room room)
    {
        if (comidaCargada && room == storageRoom.GetComponent<Room>()) // Si tiene comida cargada y ha llegado a la sala de almacenamiento
        {
            storageRoom.GetComponent<Room>().Add(1); // Agregar comida a la sala de almacenamiento
            comidaCargada = false; // Marcar que ya no tiene comida cargada
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

        if (!climaFavorable || inDanger) // Si el clima no es favorable o está en peligro, esperar
        {
            return; // Salir de la actualización si está esperando
        }

        if (inDanger && assignedResource == null) // Si está en peligro y sin un recurso
        {
            MoveTo(securityRoom.transform.position);
        }
        else
        {
            // Si no tiene un recurso asignado y el clima es favorable, buscar uno
            if (assignedResource == null && climaFavorable)
            {
                LookForResource();
            }
        }

        if (assignedResource == null && climaFavorable && !comidaCargada && !inDanger)
        {
            Explore(); // Explorar si no hay un recurso asignado y el clima es favorable
        }
    }
}