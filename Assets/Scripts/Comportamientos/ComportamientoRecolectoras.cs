using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoRecolectoras : MonoBehaviour
{
    public enum EstadoHormiga { Explorando, Recoleccion, Huida, RetornoNido };

    public EstadoHormiga estadoActual;
    public Vector3 objetivoActual;

    public Map map;
    public GameObject storageRoom;

    public float velocidadMovimiento;

    void Awake()
    {
        estadoActual = EstadoHormiga.Explorando;
        objetivoActual = map.GetRandomWalkablePosition();
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoHormiga.Explorando:
                ComportamientoExploracion();
                break;
            case EstadoHormiga.Recoleccion:
                ComportamientoRecoleccion();
                break;
            case EstadoHormiga.Huida:
                ComportamientoHuida();
                break;
            case EstadoHormiga.RetornoNido:
                ComportamientoRetornoNido();
                break;
        }

        // Mover la hormiga hacia el objetivo actual
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidadMovimiento * Time.deltaTime);
    }

    void ComportamientoExploracion()
    {
        // Si la hormiga llega al objetivo, obtener un nuevo objetivo aleatorio
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            objetivoActual = map.GetRandomWalkablePosition();
        }

        // Si detecta un recurso, cambiar al estado de recolección
        if (map.IsResourceAtPosition(transform.position))
        {
            estadoActual = EstadoHormiga.Recoleccion;
            objetivoActual = transform.position;
        }

        // Si detecta un depredador, cambiar al estado de huida
        if (map.IsPredatorAtPosition(transform.position))
        {
            estadoActual = EstadoHormiga.Huida;
            objetivoActual = map.GetClosestExit();
        }
    }

    void ComportamientoRecoleccion()
    {
        // Si la hormiga ha recogido el recurso, cambiar al estado de retorno al nido
        if (map.CollectResourceAtPosition(transform.position))
        {
            estadoActual = EstadoHormiga.RetornoNido;
            objetivoActual = storageRoom.transform.position;
        }
    }

    void ComportamientoHuida()
    {
        // Si la hormiga llega a la salida, cambiar al estado de exploración
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            estadoActual = EstadoHormiga.Explorando;
            objetivoActual = map.GetRandomWalkablePosition();
        }
    }

    void ComportamientoRetornoNido()
    {
        // Si la hormiga llega al nido, dejar el recurso y cambiar al estado de exploración
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            storageRoom.GetComponent<Room>().Add(1);
            estadoActual = EstadoHormiga.Explorando;
            objetivoActual = map.GetRandomWalkablePosition();
        }
    }
}