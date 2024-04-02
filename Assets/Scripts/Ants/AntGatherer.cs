using UnityEngine;

public class AntGatherer : Ant
{
    public Map map;
    public GameObject storageRoom;

    GameObject assignedResource;

    public float tiempoEspera = 3f;
    public bool comidaCargada = false;
    public bool climaFavorable = true; // Variable para representar si el clima es favorable

    public override void Initialize()
    {
        LookForResource();
    }

    // Buscar recurso para recolección
    public void LookForResource()
    {
        if (climaFavorable) // Verificar si el clima es favorable antes de buscar recursos
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger (se eliminará de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
            }
        }
    }

    // Cuando llega al recurso
    public override void ArrivedAtResource(GameObject resource)
    {
        if (assignedResource == resource && !comidaCargada) // Si la hormiga colisiona con su recurso asignado y no tiene comida cargada
        {
            Destroy(resource); // Destruir el recurso
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
    }
}