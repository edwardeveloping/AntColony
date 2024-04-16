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

    public GameObject progressBarPrefab; // Prefab de la barra de progreso
    private GameObject progressBarInstance; // Instancia de la barra de progreso
    private Slider progressBar; // Referencia al objeto Slider de la barra de progreso

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
            while (!alimentada) // Seguirá en el bucle hasta que esté alimentada
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

        // Método para poner un huevo
        public void StartLayEgg()
        {
            Debug.Log("Poniendo huevo...");

            // Iniciar el proceso de incubación
            StartCoroutine(IncubateEgg());
        }

        // Método para incubar el huevo durante un tiempo determinado
        private IEnumerator IncubateEgg()
        {
            float tiempoTranscurridoIncubacion = 0f;

        // Crear la barra de progreso
        CreateProgressBar();


        // Esperar el tiempo de incubación
        while (tiempoTranscurridoIncubacion < tiempoIncubacion)
            {
                // Actualizar el tiempo transcurrido
                tiempoTranscurridoIncubacion += Time.deltaTime;
                UpdateProgressBar(tiempoTranscurridoIncubacion);

            
                yield return null; // Esperar un frame
            }

        
            // Destruir la barra de progreso después de la incubación
            DestroyProgressBar();

            // Generar la larva después de la incubación
            GenerateLarva();
        }

        // Método para generar la larva
        private void GenerateLarva()
        {
            Debug.Log("Generando larva...");

            // Calcular la posición de generación de la larva cerca de la reina
            Vector3 queenPosition = transform.position;
            Vector3 larvaPosition = queenPosition + new Vector3(0.5f, 0f, 0f);

            // Generar la larva en la posición calculada
            GameObject larvaObj = antManager.GenerateAnt(larvaPosition.x, larvaPosition.y, AntManager.Role.Larva);

            // Volver a esperar a ser alimentada
            StartWaitForFood();
        }

    private void CreateProgressBar()
    {
        // Instanciar el prefab de la barra de progreso
        progressBarInstance = Instantiate(progressBarPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        // Obtener referencia al Slider de la barra de progreso
        progressBar = progressBarInstance.GetComponentInChildren<Slider>();
        // Establecer el valor máximo de la barra de progreso
        progressBar.maxValue = tiempoIncubacion;
        // Iniciar la barra de progreso con un valor de 0
        progressBar.value = 0;
    }

    // Método para actualizar la barra de progreso
    private void UpdateProgressBar(float value)
    {
        // Actualizar el valor de la barra de progreso
        progressBar.value = value;
    }

    private void DestroyProgressBar()
    {
        // Destruir la instancia de la barra de progreso
        Destroy(progressBarInstance);
    }


    public override void ArrivedAtResource(GameObject resource) { }
    public override void ArrivedAtRoom(Room room) { }
    public override void WhenCombatWon() { }
}


//    public override void Initialize()
//    {
//        StartWaitForFood = () =>
//        {
//            Debug.Log("Esperando a ser alimentada...");
//            //Cuando este alimentada?

//            // Detectar colisiones con objetos en el rango usando OverlapCircle
//            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

//            // Iterar sobre los colisionadores encontrados
//            foreach (Collider2D collider in colliders)
//            {
//                // Verificar si el objeto que entra en el rango es comida
//                if (collider.CompareTag("Food"))
//                {
//                    // Destruir la comida
//                    Destroy(collider.gameObject);
//                    // Marcar a la reina como alimentada
//                    alimentada = true;
//                    // Salir del bucle ya que hemos encontrado comida
//                    break;
//                }
//            }

//        };

//        UpdateWaitForFood = () =>
//        {
//            // Verificar si la reina está alimentada
//            if (alimentada)
//            {
//                // Verificar si el tiempo de vida es menor que x
//                if (tiempoDeVida < tiempoVidaLimite)
//                {

//                    // Cambiar al estado de poner huevo
//                    StartLayEgg();
//                    return Status.Success;
//                }
//                else
//                {
//                    // Generar jalea real
//                    StartGenerateRoyalJelly();
//                    return Status.Success;
//                }
//            }
//            else
//            {
//                // No cambiar de estado
//                return Status.Running;
//            }
//        };

//        StartLayEgg = () =>
//        {
//            Debug.Log("Poniendo huevo...");
//            float tiempoTranscurridoIncubacion = 0f;
//            while (tiempoTranscurridoIncubacion < tiempoIncubacion)
//            {
//                // Actualizar el tiempo transcurrido
//                tiempoTranscurridoIncubacion += Time.deltaTime;

//                //AQUÍ SE GENERARÁ UNA LARVA
//                GameObject larvaObj = antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Larva);

//            };

//            UpdateLayEgg = () =>
//            {
//                tiempoTranscurridoIncubacion += Time.deltaTime;

//                if (alimentada && tiempoTranscurridoIncubacion >= tiempoIncubacion)
//                {
//                    alimentada = false;
//                    StartWaitForFood(); // Vuelve al estado de espera
//                    GameObject larvaObj = antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Larva);
//                    return Status.Success;
//                }
//                else
//                {
//                    // Verificar otras condiciones si es necesario
//                    return Status.Running;
//                }
//            };
//        };

//        StartGenerateRoyalJelly = () =>
//        {
//            Debug.Log("Generando Jalea Real...");
//            float tiempoTranscurridoGenerarJalea = 0f;
//            while (tiempoTranscurridoGenerarJalea < tiempoIncubacion)
//            {
//                // Actualizar el tiempo transcurrido
//                tiempoTranscurridoGenerarJalea += Time.deltaTime;

//                //AQUÍ SE GENERARÁ la Jalea
//                //GameObject royalJellyObj = Instantiate(royalJellyPrefab, transform.position, Quaternion.identity);
//            };

//            UpdateGenerateRoyalJelly = () =>
//            {
//                tiempoTranscurridoGenerarJalea += Time.deltaTime;

//                if (alimentada && tiempoTranscurridoGenerarJalea >= tiempoIncubacion)
//                {
//                    alimentada = false;
//                    StartWaitForFood(); // Vuelve al estado de espera
//                    return Status.Success;
//                }
//                else
//                {
//                    // Verificar otras condiciones si es necesario
//                    return Status.Running;
//                }
//            };

//            StartDie = () =>
//            {
//                Die();
//            };
//        };
//        }
//    public System.Action StartWaitForFood = () => {
//        // Implementa la lógica para que la reina espere a ser alimentada
//        // Esto podría incluir animaciones, sonidos, etc.
//        Debug.Log("Generando Jalea Real...");
//    };
//    public Func<Status> UpdateWaitForFood = () => {
//        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
//        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
//        Debug.Log("Actualizando estado mientras la reina pone un huevo...");
//        return Status.Running; // Ejemplo: devolver un estado ficticio
//    };

//    public System.Action StartLayEgg = () =>
//    {
//        // Implementa la lógica para que la reina espere a ser alimentada
//        // Esto podría incluir animaciones, sonidos, etc.
//        Debug.Log("Generando Jalea Real...");
//    };

//    public Func<Status> UpdateLayEgg = () => {
//        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
//        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
//        Debug.Log("Actualizando estado mientras la reina pone un huevo...");
//        return Status.Running; // Ejemplo: devolver un estado ficticio
//    };

//    public System.Action StartGenerateRoyalJelly = () => {
//        // Implementa la lógica para que la reina espere a ser alimentada
//        // Esto podría incluir animaciones, sonidos, etc.
//        Debug.Log("Generando Jalea Real...");
//    };
//    public Func<Status> UpdateGenerateRoyalJelly = () => {
//        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
//        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
//        Debug.Log("Actualizando estado mientras la reina genera Jalea Real...");
//        return Status.Running; // Ejemplo: devolver un estado ficticio
//    };

//    public System.Action StartDie = () => {
//        // Implementa la lógica para que la reina espere a ser alimentada
//        // Esto podría incluir animaciones, sonidos, etc.
//        Debug.Log("Muriendo...");
//    };


//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        // Verificar si el objeto que entra en el rango es comida
//        if (other.CompareTag("Food"))
//        //{
//            // Destruir la comida
//            Destroy(other.gameObject);

//            // Indicar que la reina ha sido alimentada
//            alimentada = true;

//            // Puedes agregar aquí cualquier otra lógica que necesites
//        }


//    public override void ArrivedAtResource(GameObject resource){}
//    public override void ArrivedAtRoom(Room room){}
//    public override void WhenCombatWon(){}
//}
