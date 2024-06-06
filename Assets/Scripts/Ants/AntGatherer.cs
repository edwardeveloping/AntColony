using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AntGatherer : Ant
{
    public Map map;
    public GameObject storageRoom;
    public GameObject securityRoom;

    public GameObject assignedResource;

    public float tiempoEspera = 3f;
    public bool comidaCargada = false;
    public bool climaFavorable = true; // Variable para representar si el clima es favorable

    public bool inDanger = false;

    //bool para comprobar si esta muerta
    public bool isDead = false;

    private static List<GameObject> resourcesInUse = new List<GameObject>(); // Recursos que están siendo recogidos por recolectoras

    public override void Initialize()
    {
        LookForResource();
    }

    // Buscar recurso para recolección
    public void LookForResource()
    {
        if (climaFavorable) // Verificar si el clima es favorable antes de buscar recursos
        {
            assignedResource = RequestResource(); // Solicitar un recurso para recoger (se eliminará de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
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
        if (comidaCargada) // Si tiene comida cargada
        {
            storageRoom.GetComponent<Room>().Add(1); // Agregar comida a la sala de almacenamiento
            comidaCargada = false; // Marcar que ya no tiene comida cargada
        }
        LookForResource();
    }

    // Cuando gana un combate
    public override void WhenCombatWon()
    {
        Flee(); // Huir
    }


    // Esperar un tiempo antes de realizar otra acción
    void WaitForAction()
    {
        tiempoEspera -= Time.deltaTime;
        if (tiempoEspera <= 0)
        {
            LookForResource(); // Cuando el tiempo de espera haya terminado, buscar más recursos
            tiempoEspera = 3f; // Reiniciar el tiempo de espera
        }


    }

    // Actualizar
    void Update()
    {
        if (!climaFavorable) // Si el clima no es favorable, esperar
        {
            WaitForAction();
        }

        // Verificar si la recolectora ha muerto
        if (isDead)
        {
            if (assignedResource != null)
            {
                resourcesInUse.Remove(assignedResource); // Marcar el recurso como no en uso si la recolectora muere
                assignedResource = null;

            }
        }

        if (inDanger && assignedResource == null) //Si esta en peligro y sin un recurso
        {
            MoveTo(securityRoom.transform.position);
        }

        else
        {
            // Si no tiene un recurso asignado y el clima es favorable, buscar uno
            if (assignedResource == null && climaFavorable)
            {
                Initialize();
            }
        }
        
    }
}