using BehaviourAPI.Core;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntQueen : Ant
{
    public int vida;
    bool alimentada;

    public override void Initialize()
    {
        
    }

    public System.Action StartWaitForFood = () => {
        Debug.Log("Esperando a ser alimentada...");
    };
    public Func<Status> UpdateWaitForFood = () => {
        // Implementa la lógica para actualizar el estado de la reina mientras espera a ser alimentada
        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
        Debug.Log("Actualizando estado mientras espera a ser alimentada...");
        return Status.Running; // Ejemplo: devolver un estado ficticio
    };

    public System.Action StartLayEgg = () => {
        // Implementa la lógica para que la reina espere a ser alimentada
        // Esto podría incluir animaciones, sonidos, etc.
        Debug.Log("Poniendo huevo...");
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
    public Func<Status> UpdateDie = () => {
        // Implementa la lógica para actualizar el estado de la reina mientras pone un huevo
        // Puede incluir la verificación de ciertas condiciones y devolver el estado apropiado
        Debug.Log("Actualizando estado mientras la reina muere...");
        return Status.Running; // Ejemplo: devolver un estado ficticio
    };




    public override void ArrivedAtResource(GameObject resource){}
    public override void ArrivedAtRoom(Room room){}
    public override void WhenCombatWon(){}
}
