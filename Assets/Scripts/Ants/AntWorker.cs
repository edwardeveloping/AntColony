using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class AntWorker : Ant
{
    public enum Recurso
    {
        ComidaReina,
        IrComidaLarvas,
        LlevarComidaLarvas,
        IrLarva,
        LlevarLarva,
        Nada
    } //Dice la accion que esta haciendo la worker

    public Room storageRoom;
    public Room raisingRoom;
    public Room breedingRoom;
    public Room queenRoom;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;

    public Recurso recursoCargado = Recurso.Nada;

    //recurso objetivo y larva a la que alimentar
    GameObject assignedResource;
    GameObject assignedLarva;
    public Map map;

    //Sprite
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Punto destino para moverse
    public Vector3 destino;

    // �ngulo de correcci�n para alinear correctamente el sprite
    private float anguloCorreccion;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        anguloCorreccion = -90f;

        barkPanel = gameObject.transform.Find("Bark").gameObject.GetComponent<SpriteRenderer>();
    }
    public override void Initialize()
    {
        //Si hay una larva en el mapa la lleva a la raisingRoom si no alimenta a la Queen
        //Cambiar para seguir la logica del documento
        if (breedingRoom.count > 0)
        {
            breedingRoom.Remove(1);
            GetToBreedingRoom();
        }
        else
        {
            FeedTheQueen();
        }
    }

    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "Llevando comida a la reina":
                barkPanel.sprite = barkList[3];
                break;
            case "Llevando comida a la larva":
                barkPanel.sprite = barkList[2];
                break;
            case "Buscando comida en storageRoom":
                barkPanel.sprite = barkList[4];
                break;
            case "Llevando larva a la raisingRoom":
                barkPanel.sprite = barkList[5];
                break;
            case "Buscando comida":
                barkPanel.sprite = barkList[0];
                break;
            case "Buscando larva":
                barkPanel.sprite = barkList[1];
                break;
        }
        
        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
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
                destino = queenRoom.transform.position;
                StartCoroutine(Bark("Llevando comida a la reina"));
            }

            if (recursoCargado == Recurso.IrComidaLarvas)
            {
                Destroy(resource); // Destruir el recurso
                recursoCargado = Recurso.LlevarComidaLarvas;
                MoveTo(raisingRoom.transform.position);
                destino = raisingRoom.transform.position;
                StartCoroutine(Bark("Llevando comida a la larva"));

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
                    destino = queenRoom.transform.position;
                    StartCoroutine(Bark("Llevando comida a la reina"));

                }

                if (recursoCargado == Recurso.IrComidaLarvas)
                {
                    recursoCargado = Recurso.LlevarComidaLarvas;
                    MoveTo(raisingRoom.transform.position);
                    destino = raisingRoom.transform.position;
                    StartCoroutine(Bark("Llevando comida a la larva"));

                }
            }
            else
            {
                LookForResource();
            }
        }

        //Si llega a la raisingRoom con una larva "la deja" y va a alimentarla si esa es la tarea
        if (raisingRoom == room && recursoCargado == Recurso.LlevarLarva)
        {
            recursoCargado = Recurso.IrComidaLarvas;
            MoveTo(storageRoom.transform.position);
            destino = storageRoom.transform.position;
            StartCoroutine(Bark("Buscando comida en storageRoom"));

        }

        if (raisingRoom == room && recursoCargado == Recurso.LlevarComidaLarvas)
        {
            raisingRoom.Add(1);
            assignedLarva = null;
            recursoCargado = Recurso.Nada;
            Initialize();
        }

        //Si llega a la breading coge una larva y la lleva a la raisingRoom si esa es la tarea
        if (breedingRoom == room && recursoCargado == Recurso.IrLarva)
        {
            assignedLarva = antManager.GenerateAnt(breedingRoom.transform.position.x,
                breedingRoom.transform.position.y, AntManager.Role.Larva);

            assignedLarva.GetComponent<AntLarva>().FollowTo(raisingRoom.transform.position);
            Debug.Log("he cogido a la larva");

            recursoCargado = Recurso.LlevarLarva;
            MoveTo(raisingRoom.transform.position);
            destino = raisingRoom.transform.position;
            StartCoroutine(Bark("Llevando larva a la raisingRoom"));

        }

        //Si llega a la sala de la reina deja la comida si esa es la tarea
        if (queenRoom == room && recursoCargado == Recurso.ComidaReina)
        {
            queenRoom.Add(1);
            recursoCargado = Recurso.Nada;
            Initialize();
        }
    }

    #endregion

    #region LookForMethods

    public void LookForResource()
    {
        StartCoroutine(WaitForResourceCor());
    }
    IEnumerator LookForResourceCor()
    {
        //Para buscar el recurso lo hago en una corrutina para que no se queden tontas en la storageRoom sin saber que hacer
        //Comprueba cada segundo si ha aparecido un recurso nuevo en el mapa y si aparece se lo asigna
        //Se aceptan sugerencias para hacerlo mas limpio xd
        //Por si quereis usarlo 
        while (assignedResource == null)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger (se eliminar  de la lista)
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                destino = assignedResource.transform.position;
                StartCoroutine(Bark("Buscando comida"));

            }

            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator WaitForResourceCor()
    {
        //Mientras que no haya recursos en la storeageRoom van a quedarse pidiendo el recurso
        //Comprueba cada segundo si ha aparecido un recurso nuevo en el mapa y si aparece se lo asigna
        //Se aceptan sugerencias para hacerlo mas limpio xd
        bool hasFood = false;
        while (!hasFood)
        {
            if (storageRoom.count > 0)
            {
                hasFood = true;
                storageRoom.Remove(1);
                if (recursoCargado == Recurso.ComidaReina)
                {
                    MoveTo(queenRoom.transform.position);
                    destino = queenRoom.transform.position;
                    StartCoroutine(Bark("Llevando comida a la reina"));

                }

                if (recursoCargado == Recurso.IrComidaLarvas)
                {
                    recursoCargado = Recurso.LlevarComidaLarvas;
                    MoveTo(raisingRoom.transform.position);
                    destino = raisingRoom.transform.position;
                    StartCoroutine(Bark("Llevando comida a la larva"));

                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
    public override void WhenCombatWon()
    {
        Flee();
    }

    private void SpriteMove()
    {
        // Mover hacia el destino
        if (destino != null)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                // Rotar el sprite hacia la direcci�n de movimiento
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

                // A�adir el �ngulo de correcci�n
                angulo += anguloCorreccion;

                // Ajustar la rotaci�n del sprite para que solo cambie en el plano 2D
                transform.rotation = Quaternion.Euler(0, 0, angulo);

            }

        }

        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }

    private void Update()
    {
        SpriteMove();
    }

    #region AccionesWorker

    public void FeedTheQueen()
    {
        //Si no esta haciendo nada va a buscar comida a la reina
        if (recursoCargado == Recurso.Nada)
        {
            recursoCargado = Recurso.ComidaReina;
            MoveTo(storageRoom.transform.position);
            destino = storageRoom.transform.position;
            StartCoroutine(Bark("Buscando comida para la reina"));

        }
    }
    public void GetToBreedingRoom()
    {
        //Si no esta haciendo nada va a buscar una larva
        if (recursoCargado == Recurso.Nada)
        {
            recursoCargado = Recurso.IrLarva;
            MoveTo(breedingRoom.transform.position); // Moverse hacia el recurso.
            destino = breedingRoom.transform.position;
            StartCoroutine(Bark("Buscando larva"));

        }
    }

    #endregion
}