using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Colony : MonoBehaviour
{
    [SerializeField] AntManager antManager;
    [SerializeField] PredatorManager predatorManager;

    int shells;

    //
    public Room storageRoom;

    //variables para controlar desde el menu de Unity compilando el programa
    public int population;
    public int storageResources;
    public int totalGatherers;
    public int totalWorkers;
    public int totalLarvaGatherers;
    public int totalLarvaWorkers;
    public int totalPredators;
    public int initial_Gatherers;
    public int initial_Workers;


    public void Initialize(string s)
    {
        if (s == "Init") //INICIALIZACION
        {
            for (int i = 0; i < initial_Gatherers; i++)
            {
                antManager.GenerateAnt(0, -5, AntManager.Role.Gatherer);
            }

            for (int i = 0; i < initial_Workers; i++)
            {
                antManager.GenerateAnt(-17, -6, AntManager.Role.Worker);
            }

            antManager.GenerateAnt(13, -5, AntManager.Role.Queen);

        }
    }


    private void Update()
    {
        ManageColony();
    }

    private void ControlColony(int gathererNumber, int workerNumber, int gathererLarvaCount, int workerLarvaCount, int resources)
    {
        //Control de hormigas
        //Si no tenemos ninguna gatherer y no tenemos 2 recursos MINIMO, una worker se tiene que convertir en gatherer



        if (gathererNumber == 0 && workerNumber > 0)
        {
            //Si solo tenemos el caso extremo de un worker
            if (workerNumber == 1)
            {
                if (resources <= 2)
                {
                    foreach (var worker in antManager.antWorkerObjectList)
                    {
                        //Cambiamos de rol
                        worker.GetComponent<Ant>().ChangeRole(AntManager.Role.Gatherer);

                        break;
                    }
                }
            }
            else
            {
                foreach (var worker in antManager.antWorkerObjectList)
                {
                    //Cambiamos de rol
                    worker.GetComponent<Ant>().ChangeRole(AntManager.Role.Gatherer);

                    break;
                }
            }
            
        }

        //Control de larvas
        if (gathererNumber <= 1 && workerNumber >= 1 && gathererLarvaCount == 0 && workerLarvaCount >= 1) 
        {
            //transformamos todas las larvas workers en gatherers, ya que tenemos al menos 1 worker
            foreach (var larva in antManager.antLarvaList)
            {
                if (larva.GetComponent<AntLarva>().type == "Worker" || string.IsNullOrEmpty(larva.GetComponent<AntLarva>().type))//no tiene tag o es worker
                {
                    larva.GetComponent<AntLarva>().type = "Gatherer";
                } 
            }
        }

        //Si tenemos muchos recursos, pero pocas obreras, una recolectora pasara a ser obrera

        if (resources >= 2)
        {
            if (totalWorkers < totalGatherers && totalGatherers > 1)
            {
                foreach (var gatherer in antManager.antGathererObjectList)
                {
                    //Cambiamos de rol
                    gatherer.GetComponent<Ant>().ChangeRole(AntManager.Role.Worker);
                    break;
                }
            }
        }

        //comparacion gatherers con workers
        if (workerNumber * 2 > gathererNumber)
        {
            foreach (var worker in antManager.antWorkerObjectList)
            {
                //Cambiamos de rol
                worker.GetComponent <Ant>().ChangeRole(AntManager.Role.Gatherer);
                break;
            }
        }

        if (gathererNumber > ((workerNumber * 2) + 2))
        {
            foreach (var gatherer in antManager.antGathererObjectList)
            {
                //Cambiamos de rol
                gatherer.GetComponent<Ant>().ChangeRole(AntManager.Role.Worker);
                break;
            }
        }


        //Enemigos
        //Si hay muchos enemigos, las gatherer iran a la SecurityRoom a esperar hasta que estos mueran
        //Hay que tener en cuenta que, si inicializamos con dos predadores, cuando mueran se generaran dos nuevos
        //Es esperar en caso de que haya mas de estos predadores base, si hubiesen 5 al regenerarse se regenerarian unicamente 2
        //Pueden producirse bucles infinitos si no

        /*if (predatorManager.initialNumPredators < totalPredators)
        {
            foreach(var gatherer in antManager.antGathererObjectList)
            {
                gatherer.GetComponent<AntGatherer>().inDanger = true;
            }
        }

        else if (predatorManager.initialNumPredators == totalPredators)
        {
            foreach (var gatherer in antManager.antGathererObjectList)
            {
                gatherer.GetComponent<AntGatherer>().inDanger = false;
            }
        }*/

    }
    private void ManageColony()
    {
        int gathererNumber = antManager.antGathererObjectList.Count;
        int workerNumber = antManager.antWorkerObjectList.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int predatorNumber = predatorManager.predators.Count;

        //Comprobar continuamente las listas de las hormigas para asignarle el tag correcto de cada larva
        //Jerarquia => Gatherer > worker

        //Ahora tambien necesitamos que la colonia tenga nocion de la cantidad de larvas que tienen ya dicho tag creado, ya que pdriamos crear 5 gatherer
        //antes de que nazca la primera larva

        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");


        //AUXILIAR PARA EL CONTROL DESDE EL MENU DEL JUEGO
        AuxiliarControl(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount, resources, predatorNumber);

        //Llamamos a nuestro metodo
        string type = DecideAntType(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount);

        //Llamamos a nuestra funcion
        AssignLarvaType(type);

        //Controlamos
        ControlColony(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount, resources);

    }

    private void AuxiliarControl(int gathererNumber, int workerNumber, int gathererLarvaCount, int workerLarvaCount, int resources, int predatorCount)
    {
        storageResources = resources;
        totalGatherers = gathererNumber;
        totalWorkers = workerNumber;
        totalLarvaGatherers = gathererLarvaCount;
        totalLarvaWorkers = workerLarvaCount;
        totalPredators = predatorCount;
        population = totalGatherers + totalWorkers + totalLarvaGatherers + totalLarvaWorkers;
    }

    private string DecideAntType(int gathererNumber, int workerNumber, int gathererLarvaCount, int workerLarvaCount)
    {
        //Si no existen Gatherers en la lista ni tampoco en las larvas pendientes por nacer, crearemos una Gatherer
        if (gathererNumber == 0 && gathererLarvaCount == 0)
        {
            return "Gatherer";
        }

        //Si existen mas gatherers que workers pero ninguna en larvas, haremos worker
        else if (gathererNumber > workerNumber && workerLarvaCount == 0)
        {
            return "Worker";
        }

        //Si ninguna de las anteriores se cumple, entonces iremos a descarte, dando prioridad a las gatherer

        else if (gathererLarvaCount == 0)
        {
            return "Gatherer";
        }

        else if (workerLarvaCount == 0)
        {
            return "Worker";
        }

        else
        {
            return gathererNumber <= workerNumber ? "Gatherer" : "Worker";
        }
    }

    private void AssignLarvaType(string antType)
    {
        //recorremos la lista de las larvas
        foreach (var larva in antManager.antLarvaList)
        {
            //si la larva aun no tiene un valor en su tipo
            if (string.IsNullOrEmpty(larva.GetComponent<AntLarva>().type))
            {
                larva.GetComponent<AntLarva>().type = antType; //establecemos el tipo
                break; //IMPORTANTE ya que si no se pondria el string a todas nuestras larvas
            }
        }
    }

    private int CountPendingLarvas(string antType)
    {
        int count = 0;
        foreach(var larva in antManager.antLarvaList)
        {
            if (larva.GetComponent<AntLarva>().type == antType)
            {
                count++;
            }
        }

        return count;
    }



}