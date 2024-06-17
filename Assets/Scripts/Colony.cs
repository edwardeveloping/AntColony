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
    [SerializeField] EscarabajoManage escarabajoManager;

    int shells;

    //
    public Room storageRoom;

    //variables para controlar desde el menu de Unity compilando el programa
    public int initial_Gatherers;
    public int initial_Workers;
    public int initial_Soldiers;
    public int initial_Predators;
    public int initial_Escarabajos;
    public int population;
    public int totalGatherers;
    public int totalWorkers;
    public int totalSoldiers;
    public int totalLarvaGatherers;
    public int totalLarvaWorkers;
    public int totalLarvaSoldiers;
    public int totalPredators;
    public int storageResources;


    //auxiliar
    private int auxiliarSoldierCount;

    private void Start()
    {
        auxiliarSoldierCount = 0;
    }
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

            for (int i = 0;i < initial_Soldiers; i++)
            {
                antManager.GenerateAnt(-10, -10, AntManager.Role.Soldier);
            }

            antManager.GenerateAnt(20, -7, AntManager.Role.Queen);

            //depredadores
            predatorManager.initialNumPredators = initial_Predators;
            
            for (int i = 0; i < initial_Predators; i++)
            {
                predatorManager.GeneratePredatorAtSpawn();
            }

            //Escarabajos
            escarabajoManager.initialNumBeetles = initial_Escarabajos;

            for (int i = 0; i < initial_Escarabajos; i++)
            {
                escarabajoManager.GenerateBeetle();
            }
        }
    }


    private void Update()
    {
        ManageColony();
        //Controlamos
        //ControlColony();
    }

    private void ControlColony()
    {
        //Control de hormigas
        //Si no tenemos ninguna gatherer y no tenemos 2 recursos MINIMO, una worker se tiene que convertir en gatherer
        int gathererNumber = antManager.antGathererObjectList.Count;
        int workerNumber = antManager.antWorkerObjectList.Count;
        int predatorNumber = predatorManager.predators.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");

        

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
        int soldierNumber = antManager.antSoldierObjectList.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int predatorNumber = predatorManager.predators.Count;

        //Comprobar continuamente las listas de las hormigas para asignarle el tag correcto de cada larva
        //Jerarquia => Gatherer > worker

        //Ahora tambien necesitamos que la colonia tenga nocion de la cantidad de larvas que tienen ya dicho tag creado, ya que pdriamos crear 5 gatherer
        //antes de que nazca la primera larva

        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");
        int soldierLarvaCount = CountPendingLarvas("Soldier");


        //AUXILIAR PARA EL CONTROL DESDE EL MENU DEL JUEGO
        AuxiliarControl(gathererNumber, workerNumber, soldierNumber, gathererLarvaCount, workerLarvaCount, resources, predatorNumber, soldierLarvaCount);

        //Llamamos a nuestro metodo
        string type = DecideAntType(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount);

        //Llamamos a nuestra funcion
        AssignLarvaType(type);

        

    }

    private void AuxiliarControl(int gathererNumber, int workerNumber, int soldierNumber, int gathererLarvaCount, int workerLarvaCount, int resources, int predatorCount, int soldierLarvaCount)
    {
        storageResources = resources;
        totalGatherers = gathererNumber;
        totalWorkers = workerNumber;
        totalSoldiers = soldierNumber;
        totalLarvaGatherers = gathererLarvaCount;
        totalLarvaWorkers = workerLarvaCount;
        totalLarvaSoldiers = soldierLarvaCount;
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

        //Si hay muchas gatherer y muchas workers, soldier
        else if (gathererNumber >= 3 && workerNumber >= 3)
        {
            return "Soldier";
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