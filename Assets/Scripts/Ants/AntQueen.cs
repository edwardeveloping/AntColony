using BehaviourAPI.Core;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntQueen : Ant
{
    public float tiempoDeVida;
    public float tiempoVidaLimite;
    public float tiempoIncubacion = 10f;
    public float tiemoFabricacionJalea = 15f;

    bool alimentada = false;
    bool incubado = false;
    bool jalea = false;

    void Awake()
    {
        Initialize();
    }
    public override void Initialize()
    {
        StartWaitForFood = () =>
        {
            Debug.Log("Esperando a ser alimentada...");
        };

        UpdateWaitForFood = () =>
        {
            // Verificar si la reina está alimentada
            if (alimentada)
            {
                // Verificar si el tiempo de vida es menor que x
                if (tiempoDeVida < tiempoVidaLimite)
                {
                    // Cambiar al estado de poner huevo
                    StartLayEgg();
                    return Status.Success;
                }
                else
                {
                    // Generar jalea real
                    StartGenerateRoyalJelly();
                    return Status.Success;
                }
            }
            else
            {
                // No cambiar de estado
                return Status.Running;
            }
        };

        StartLayEgg = () =>
        {
            Debug.Log("Poniendo huevo...");
            float tiempoTranscurridoIncubacion = 0f;
            while (tiempoTranscurridoIncubacion < tiemoFabricacionJalea)
            {
                // Actualizar el tiempo transcurrido
                tiempoTranscurridoIncubacion += Time.deltaTime;
                jalea = true;

                //AQUÍ SE GENERARÁ UNA LARVA
            };

            UpdateLayEgg = () =>
            {
                tiempoTranscurridoIncubacion += Time.deltaTime;

                if (incubado && tiempoTranscurridoIncubacion >= tiemoFabricacionJalea)
                {
                    jalea = false;
                    StartWaitForFood(); // Vuelve al estado de espera
                    return Status.Success;
                }
                else
                {
                    // Verificar otras condiciones si es necesario
                    return Status.Running;
                }
            };
        };

        StartGenerateRoyalJelly = () =>
        {
            Debug.Log("Generando Jalea Real...");
            float tiempoTranscurridoGenerarJalea = 0f;
            while (tiempoTranscurridoGenerarJalea < tiempoIncubacion)
            {
                // Actualizar el tiempo transcurrido
                tiempoTranscurridoGenerarJalea += Time.deltaTime;
                jalea = true;

                //AQUÍ SE GENERARÁ la Jalea
            };

            UpdateGenerateRoyalJelly = () =>
            {
                tiempoTranscurridoGenerarJalea += Time.deltaTime;

                if (jalea && tiempoTranscurridoGenerarJalea >= tiempoIncubacion)
                {
                    incubado = false;
                    StartWaitForFood(); // Vuelve al estado de espera
                    return Status.Success;
                }
                else
                {
                    // Verificar otras condiciones si es necesario
                    return Status.Running;
                }
            };

            StartDie = () =>
            {
                Die();
            };
        };
        }
    public System.Action StartWaitForFood = () => {
        // Implementa la lógica para que la reina espere a ser alimentada
        // Esto podría incluir animaciones, sonidos, etc.
        Debug.Log("Generando Jalea Real...");
    };
    public Func<Status> UpdateWaitForFood = () => {
        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
        Debug.Log("Actualizando estado mientras la reina pone un huevo...");
        return Status.Running; // Ejemplo: devolver un estado ficticio
    };

    public System.Action StartLayEgg = () =>
    {
        // Implementa la lógica para que la reina espere a ser alimentada
        // Esto podría incluir animaciones, sonidos, etc.
        Debug.Log("Generando Jalea Real...");
    };

    public Func<Status> UpdateLayEgg = () => {
        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
        Debug.Log("Actualizando estado mientras la reina pone un huevo...");
        return Status.Running; // Ejemplo: devolver un estado ficticio
    };

    public System.Action StartGenerateRoyalJelly = () => {
        // Implementa la lógica para que la reina espere a ser alimentada
        // Esto podría incluir animaciones, sonidos, etc.
        Debug.Log("Generando Jalea Real...");
    };
    public Func<Status> UpdateGenerateRoyalJelly = () => {
        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
        Debug.Log("Actualizando estado mientras la reina genera Jalea Real...");
        return Status.Running; // Ejemplo: devolver un estado ficticio
    };

    public System.Action StartDie = () => {
        // Implementa la lógica para que la reina espere a ser alimentada
        // Esto podría incluir animaciones, sonidos, etc.
        Debug.Log("Muriendo...");
    };
    




    public override void ArrivedAtResource(GameObject resource){}
    public override void ArrivedAtRoom(Room room){}
    public override void WhenCombatWon(){}
}
