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

            antManager.GenerateAnt(20, -7, AntManager.Role.Queen);

        }
    }


    private void Update()
    {
        ManageColony();
        //Controlamos
        ControlColony();
    }

    private void ControlColony()
    {
        //Control de hormigas
        //Si no tenemos ninguna gatherer y no tenemos 2 recursos MINIMO, una worker se tiene que convertir en gatherer
        int gathererNumber = antManager.antGathererObjectList.Count;
        int workerNumber = antManager.antWorkerObjectList.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");


        /*if (gathererNumber != workerNumber * 2)
        {
            if (gathererNumber < workerNumber * 2)
            {
                foreach (var worker in antManager.antWorkerObjectList)
                {
                    worker.GetComponent<Ant>().ChangeRole(AntManager.Role.Gatherer);
                    break;
                }
            }
            
            else if (gathererNumber > workerNumber * 2)
            {
                foreach (var gatherer in antManager.antGathererObjectList)
                {
                    gatherer.GetComponent<Ant>().ChangeRole(AntManager.Role.Worker);
                    break;
                }
            }
        }*/




        // CASO EXTREMO
        if (gathererNumber == 0 && workerNumber == 1 && resources < 3)
        {
            foreach (var worker in antManager.antWorkerObjectList)
            {
                worker.GetComponent<Ant>().ChangeRole(AntManager.Role.Gatherer);
            }
        }

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