using BehaviourAPI.Core;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static AntManager;


public class AntQueen : Ant
{
    public Room queenRoom;
    public Room breedingRoom;

    public float tiempoIncubacion = 10f;

    bool alimentada = false;

    public float salud = 100f;
    public float saludMaxima = 100f;
    public float tasaDeterioroSalud = 0.5f;
    public float umbralHambre = 20f; // Umbral para determinar cuándo tiene hambre


    private System.Random random = new System.Random();


    //Barra de vida
    public UnityEngine.UI.Slider barraDeVida;


    //Sprite
    private float flipTime;
    private float flipTimeActual;
    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;
    
    public SpriteRenderer barkPanel;
    public Sprite[] barkList;

    private void Start()
    {

        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.5f;
        flipTimeActual = flipTime;

        // Iniciar la barra de vida
        barraDeVida.maxValue = saludMaxima;
        barraDeVida.value = salud;


        // Iniciar el comportamiento
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
        StartCoroutine(Bark("Quiero comer"));
        StartCoroutine(PollForFood());
    }

    private IEnumerator PollForFood()
    {
        while (true)
        {
            if (queenRoom.count > 0)
            {
                queenRoom.Remove(1);
                if (salud < umbralHambre)
                {
                    Alimentarse();
                }
                else
                {
                    StartLayEgg();
                }
            }



            // Esperamos antes de volver a verificar
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "Quiero comer":
                barkPanel.sprite = barkList[0];
                break;
            case "Engendrando larva":
                barkPanel.sprite = barkList[1];
                break;
            
        }
        
        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
    }
    private void Alimentarse()
    {
        salud = Mathf.Min(salud + 20f, saludMaxima);
        Debug.Log("La hormiga reina se ha alimentado.");
    }

    // Metodo para poner un huevo
    public void StartLayEgg()
    {
        alimentada = false;
        Debug.Log("Poniendo huevo...");

        // Iniciar el proceso de incubación
        StartCoroutine(IncubateEgg());
    }

    private IEnumerator IncubateEgg()
    {
        float tiempoTranscurridoIncubacion = 0f;
        // Esperar el tiempo de incubacion
        while (tiempoTranscurridoIncubacion < tiempoIncubacion)
        {
            // Actualizar el tiempo transcurrido
            tiempoTranscurridoIncubacion += Time.deltaTime;
            yield return null; // Esperar un frame
        }
        // Generar la larva despues de la incubacion
        GenerateLarva();
    }


    // Método para generar la larva
    private void GenerateLarva()
    {
        Debug.Log("Generando larva...");
        breedingRoom.Add(1);
        Debug.Log("Larva generada.");

        StartWaitForFood();
    }


    public override void ArrivedAtResource(GameObject resource) { }
    public override void ArrivedAtRoom(Room room) { }
    public override void WhenCombatWon() { }

    private void SpriteMove()
    {
        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
    private void TerminarSimulacion()
    {
        // Cambiar a la escena "Finalsimulacion"
        SceneManager.LoadScene("FinalSimulacion");
    }

    private void Update()
    {
        SpriteMove();


        // Deterioro de salud con el tiempo
        salud -= tasaDeterioroSalud * Time.deltaTime;
        barraDeVida.value = salud; // Actualizar la barra de vida

        if (salud <= 0)
        {
            Debug.Log("La hormiga reina ha muerto...");
            TerminarSimulacion();
        }

    }

}


        ////public AntManager antManager;
        //public float tiempoDeVida;
        //public float tiempoVidaLimite;
        //public float tiempoIncubacion = 10f;
        //public float tiemoFabricacionJalea = 15f;

        //bool alimentada = false;
        //bool incubado = false;
        //bool jalea = false;

        //void Awake()
        //{
        //    Initialize();
        //}
        //public override void Initialize()
        //{
        //    StartWaitForFood = () =>
        //    {
        //        Debug.Log("Esperando a ser alimentada...");
        //        //Cuando este alimentada?
        //        alimentada = true;
        //    };

        //    UpdateWaitForFood = () =>
        //    {
        //        // Verificar si la reina est� alimentada
        //        if (alimentada)
        //        {
        //            // Verificar si el tiempo de vida es menor que x
        //            if (tiempoDeVida < tiempoVidaLimite)
        //            {

        //                // Cambiar al estado de poner huevo
        //                StartLayEgg();
        //                return Status.Success;
        //            }
        //            else
        //            {
        //                // Generar jalea real
        //                StartGenerateRoyalJelly();
        //                return Status.Success;
        //            }
        //        }
        //        else
        //        {
        //            // No cambiar de estado
        //            return Status.Running;
        //        }
        //    };

        //    StartLayEgg = () =>
        //    {
        //        Debug.Log("Poniendo huevo...");
        //        float tiempoTranscurridoIncubacion = 0f;
        //        while (tiempoTranscurridoIncubacion < tiempoIncubacion)
        //        {
        //            // Actualizar el tiempo transcurrido
        //            tiempoTranscurridoIncubacion += Time.deltaTime;


        //            //AQU� SE GENERAR� UNA LARVA
        //        };

        //        UpdateLayEgg = () =>
        //        {
        //            tiempoTranscurridoIncubacion += Time.deltaTime;

        //            if (alimentada && tiempoTranscurridoIncubacion >= tiempoIncubacion)
        //            {
        //                alimentada = false;
        //                StartWaitForFood(); // Vuelve al estado de espera
        //                GameObject larvaObj = antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Larva);
        //                return Status.Success;
        //            }
        //            else
        //            {
        //                // Verificar otras condiciones si es necesario
        //                return Status.Running;
        //            }
        //        };
        //    };

        //    StartGenerateRoyalJelly = () =>
        //    {
        //        Debug.Log("Generando Jalea Real...");
        //        float tiempoTranscurridoGenerarJalea = 0f;
        //        while (tiempoTranscurridoGenerarJalea < tiempoIncubacion)
        //        {
        //            // Actualizar el tiempo transcurrido
        //            tiempoTranscurridoGenerarJalea += Time.deltaTime;

        //            //AQU� SE GENERAR� la Jalea
        //        };

        //        UpdateGenerateRoyalJelly = () =>
        //        {
        //            tiempoTranscurridoGenerarJalea += Time.deltaTime;

        //            if (alimentada && tiempoTranscurridoGenerarJalea >= tiempoIncubacion)
        //            {
        //                alimentada = false;
        //                StartWaitForFood(); // Vuelve al estado de espera
        //                return Status.Success;
        //            }
        //            else
        //            {
        //                // Verificar otras condiciones si es necesario
        //                return Status.Running;
        //            }
        //        };

        //        StartDie = () =>
        //        {
        //            Die();
        //        };
        //    };
        //}
        //public System.Action StartWaitForFood = () => {
        //    // Implementa la l�gica para que la reina espere a ser alimentada
        //    // Esto podr�a incluir animaciones, sonidos, etc.
        //    Debug.Log("Generando Jalea Real...");
        //};
        //public Func<Status> UpdateWaitForFood = () => {
        //    // Implementa la l�gica para actualizar el estado de la reina mientras pone un huevo
        //    // Puede incluir la verificaci�n de ciertas condiciones y devolver el estado apropiado
        //    Debug.Log("Actualizando estado mientras la reina pone un huevo...");
        //    return Status.Running; // Ejemplo: devolver un estado ficticio
        //};

        //public System.Action StartLayEgg = () =>
        //{
        //    // Implementa la l�gica para que la reina espere a ser alimentada
        //    // Esto podr�a incluir animaciones, sonidos, etc.
        //    Debug.Log("Generando Jalea Real...");
        //};

        //public Func<Status> UpdateLayEgg = () => {
        //    // Implementa la l�gica para actualizar el estado de la reina mientras pone un huevo
        //    // Puede incluir la verificaci�n de ciertas condiciones y devolver el estado apropiado
        //    Debug.Log("Actualizando estado mientras la reina pone un huevo...");
        //    return Status.Running; // Ejemplo: devolver un estado ficticio
        //};

        //public System.Action StartGenerateRoyalJelly = () => {
        //    // Implementa la l�gica para que la reina espere a ser alimentada
        //    // Esto podr�a incluir animaciones, sonidos, etc.
        //    Debug.Log("Generando Jalea Real...");
        //};
        //public Func<Status> UpdateGenerateRoyalJelly = () => {
        //    // Implementa la l�gica para actualizar el estado de la reina mientras pone un huevo
        //    // Puede incluir la verificaci�n de ciertas condiciones y devolver el estado apropiado
        //    Debug.Log("Actualizando estado mientras la reina genera Jalea Real...");
        //    return Status.Running; // Ejemplo: devolver un estado ficticio
        //};

        //public System.Action StartDie = () => {
        //    // Implementa la l�gica para que la reina espere a ser alimentada
        //    // Esto podr�a incluir animaciones, sonidos, etc.
        //    Debug.Log("Muriendo...");
        //};





        //public override void ArrivedAtResource(GameObject resource) { }
        //public override void ArrivedAtRoom(Room room) { }
        //public override void WhenCombatWon() { }
    