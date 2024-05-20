using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AntWorker : Ant
{
    public enum Recurso
    {
        ComidaReina,
        ComidaLarvas,
        Larva,
        Nada
    }//Dice la accion que esta haciendo la worker

    public Room storageRoom;
    public Room raisingRoom;
    public Room queenRoom;


    public Recurso recursoCargado = Recurso.Nada;

    //recurso objetivo y larva a la que alimentar
    GameObject assignedResource;
    GameObject assignedLarva;
    public Map map;

    public GameObject resource;

    public override void Initialize()
    {
        //Si hay una larva en el mapa la lleva a la raisingRoom si no alimenta a la Queen
        //Cambiar para seguir la logica del documento
        if (antManager.antLarvaList.Count > 0)
            GetLarvasToRaisingRoom();
        else
        {
            FeedTheQueen();
        }
    }

    #region OnCollision/Trigger

    public override void ArrivedAtResource(GameObject resource)
    {
        //Cuando llega al recurso objetivo lo coge y lo lleva a la reina o a la larva dependiendo de la accion que este haciendo
        if (assignedResource == resource) // Si la hormiga colisiona con su recurso asignado y no tiene comida cargada
        {
            if (recursoCargado == Recurso.ComidaReina)
            {
                Destroy(resource); // Destruir el recurso
                MoveTo(queenRoom.transform.position);
            }

            if (recursoCargado == Recurso.ComidaLarvas)
            {
                Destroy(resource); // Destruir el recurso
                MoveTo(assignedLarva.transform.position);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Cuando colisiona con la larva o la lleva a la raisingRoom o la alimenta dependiendo de la accion que este haciendo
        if (collision.gameObject.TryGetComponent<AntLarva>(out var aux))
        {
            if (recursoCargado == Recurso.Larva)
            {
                assignedLarva.GetComponent<AntLarva>().FollowTo(raisingRoom.transform.position);
                antManager.antLarvaList.Remove(assignedLarva);
                Debug.Log("he cogido a la larva");
                MoveTo(raisingRoom.transform.position);
            }

            if (recursoCargado == Recurso.ComidaLarvas)
            {
                var auxComida = Instantiate(this.resource);
                auxComida.transform.position = gameObject.transform.position;
                recursoCargado = Recurso.Nada;
                Debug.Log("he alimentado a la larva");
                Initialize();
            }
        }
    }

    public override void ArrivedAtRoom(Room room)
    {
        //Cuando llega a la storageRoom si hay al menos un recurso lo lleva a la larva o a la reina dependiendo de la tarea que haga
        //Si no hay busca uno
        if (storageRoom == room)
        {
            if (storageRoom.count > 0)
            {
                storageRoom.Remove(1);
                if (recursoCargado == Recurso.ComidaReina)
                {
                    MoveTo(queenRoom.transform.position);
                }

                if (recursoCargado == Recurso.ComidaLarvas)
                {
                    MoveTo(assignedLarva.transform.position);
                }
            }
            else
            {
                LookForResource();
            }
        }

        //Si llega a la raisingRoom con una larva "la deja" y va a alimentarla si esa es la tarea
        if (raisingRoom == room && recursoCargado == Recurso.Larva)
        {
            recursoCargado = Recurso.Nada;
            FeedTheLarva();
        }

        //Si llega a la sala de la reina deja la comida si esa es la tarea
        if (queenRoom == room && recursoCargado == Recurso.ComidaReina)
        {
            var auxComida = Instantiate(this.resource);
            auxComida.transform.position = gameObject.transform.position;
            recursoCargado = Recurso.Nada;
            Initialize();
        }
    }

    #endregion

    #region LookForMethods

    public void LookForResource()
    {
        StartCoroutine(LookForResourceCor());
    }

    IEnumerator LookForResourceCor()
    {
        //Para buscar el recurso lo hago en una corrutina para que no se queden tontas en la storageRoom sin saber que hacer
        //Comprueba cada segundo si ha aparecido un recurso nuevo en el mapa y si aparece se lo asigna
        //Se aceptan sugerencias para hacerlo mas limpio xd
        while (assignedResource == null)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger (se eliminar  de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.

            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void LookForLarva()
    {
        //Si hay larvas en el mundo coge una al azar de la lista y va a por ella
        if (antManager.antLarvaList.Count > 0)
        {
            assignedLarva = antManager.antLarvaList[new Random().Next(0, antManager.antLarvaList.Count)]; // Solicitar un recurso para recoger (se eliminar  de la lista)
            if (assignedLarva != null)
            {
                MoveTo(assignedLarva.transform.position); // Moverse hacia el recurso.
            }
        }
    }

    #endregion



    public override void WhenCombatWon()
    {
        Flee();
    }

    #region AccionesWorker

    public void FeedTheQueen()//done
    {
        //Si no esta haciendo nada va a buscar comida a la reina
        if (recursoCargado == Recurso.Nada)
        {
            recursoCargado = Recurso.ComidaReina;
            MoveTo(storageRoom.transform.position);
        }
    }

    public void GetLarvasToRaisingRoom()//falta ver como cojo y suelto las larvas
    {
        //Si no esta haciendo nada va a buscar una larva
        if (recursoCargado == Recurso.Nada)
        {
            recursoCargado = Recurso.Larva;
            LookForLarva();
        }
    }

    public void FeedTheLarva()
    {
        //Si ha terminado de llevar a la larva a la raisingRoom le lleva comida
        if (recursoCargado == Recurso.Nada)
        {
            recursoCargado = Recurso.ComidaLarvas;
            MoveTo(storageRoom.transform.position);
        }
    }

    #endregion

}