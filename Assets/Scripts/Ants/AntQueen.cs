using BehaviourAPI.Core;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AntManager;

public class AntQueen : Ant
{
    public float detectionRadius = 5f;
    public float tiempoDeVida = 0f;
    public float tiempoVidaLimite = 100000f;
    public float tiempoIncubacion = 10f;
    public float tiemoFabricacionJalea = 15f;

    //public GameObject progressBarPrefab; // Prefab de la barra de progreso
    //private GameObject progressBarInstance; // Instancia de la barra de progreso
    //private Slider progressBar; // Referencia al objeto Slider de la barra de progreso

    bool alimentada = false;
    bool incubado = false;
    bool jalea = false;

    void Awake()
    {
        Initialize();
    }
    public override void Initialize()
    {
        StartWaitForFood();
    }



    //METODOS
    public void StartWaitForFood()
    {
        Debug.Log("Esperando a ser alimentada...");

        StartCoroutine(PollForFood());
    }

    private IEnumerator PollForFood()
    {
        while (!alimentada) // Seguir� en el bucle hasta que est� alimentada
        {
            // Detectar colisiones con objetos en el rango usando OverlapCircle
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

            // Iterar sobre los colisionadores encontrados
            foreach (Collider2D collider in colliders)
            {
                // Verificar si el objeto que entra en el rango es comida
                if (collider.CompareTag("Food"))
                {
                    // Destruir la comida
                    Destroy(collider.gameObject);
                    // Marcar a la reina como alimentada
                    alimentada = true;
                    // Iniciar la puesta de huevo
                    StartLayEgg();
                    // Salir del bucle ya que hemos encontrado comida
                    break;
                }
            }

            // Esperar un corto tiempo antes de volver a verificar
            yield return new WaitForSeconds(0.5f);
        }
    }

    // M�todo para poner un huevo
    public void StartLayEgg()
    {
        Debug.Log("Poniendo huevo...");

        // Iniciar el proceso de incubaci�n
        StartCoroutine(IncubateEgg());
    }

    // M�todo para incubar el huevo durante un tiempo determinado
    private IEnumerator IncubateEgg()
    {
        float tiempoTranscurridoIncubacion = 0f;




        // Esperar el tiempo de incubaci�n
        while (tiempoTranscurridoIncubacion < tiempoIncubacion)
        {
            // Actualizar el tiempo transcurrido
            tiempoTranscurridoIncubacion += Time.deltaTime;



            yield return null; // Esperar un frame
        }



        // Generar la larva despu�s de la incubaci�n
        GenerateLarva();
    }

    // M�todo para generar la larva
    private void GenerateLarva()
    {
        Debug.Log("Generando larva...");

        // Calcular la posici�n de generaci�n de la larva cerca de la reina
        Vector3 queenPosition = transform.position;

        // Definir el rango m�ximo desde la posici�n de la reina para generar la larva
        float maxRange = 1.0f;

        // Generar un vector aleatorio dentro de un c�rculo unitario y escalarlo al rango m�ximo
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * maxRange;

        // Calcular la posici�n de la larva sumando el desplazamiento aleatorio alrededor de la reina
        Vector3 larvaPosition = queenPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

        // Generar la larva en la posici�n calculada
        GameObject larvaObj = antManager.GenerateAnt(larvaPosition.x, larvaPosition.y, AntManager.Role.Larva);

        // Volver a esperar a ser alimentada
        StartWaitForFood();
    }

    public override void ArrivedAtResource(GameObject resource) { }
    public override void ArrivedAtRoom(Room room) { }
    public override void WhenCombatWon() { }
}
