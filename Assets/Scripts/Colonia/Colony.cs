using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static AntManager;

public class Colony : MonoBehaviour
{
    [SerializeField] AntManager antManager;
    [SerializeField] PredatorManager predatorManager;
    [SerializeField] EscarabajoManage escarabajoManager;
    [SerializeField] Map map;

    int shells;

    public Room storageRoom;

    //variables para controlar desde el menu de Unity compilando el programa
    private int initial_Gatherers;
    private int initial_Workers;
    private int initial_Soldiers;
    private int initial_Predators;
    private int initial_Beetles;
    public int population;
    public int totalGatherers;
    public int totalWorkers;
    public int totalSoldiers;
    public int totalLarvaGatherers;
    public int totalLarvaWorkers;
    public int totalLarvaSoldiers;
    public int totalPredators;
    public int totalBeetles;
    public int storageResources;
    public int mapResources;

    //auxiliar
    private int auxiliarSoldierCount;
    public bool weatherFavorable;

    // UI Elements
    public Slider sliderGatherers;
    public Slider sliderWorkers;
    public Slider sliderSoldiers;
    public Slider sliderPredators;
    public Slider sliderEscarabajos;
    public Button buttonStartSimulation;

    // Text elements to display slider values
    public TMP_Text textGatherersValue;
    public TMP_Text textWorkersValue;
    public TMP_Text textSoldiersValue;
    public TMP_Text textPredatorsValue;
    public TMP_Text textEscarabajosValue;

    // Referencias a otros scripts
    public GameObject gameManager;
    public MonoBehaviour mapScript;
    public MonoBehaviour predatorManagerScript;

    //Acciones y percepciones
    public ColonyPerceptions perceptions;
    public ColonyActions actions;

    //FUNCIONES PRINCIPALES DE LA COLONIA

    //Nombramos en forma de Enum el numero de percepciones segun nuestra tabla de percepciones
    public enum Perception
    {
        AtaqueALaColonia,
        FaltaDeComida,
        IncapacidadParaRecolectarComida,
        ExcesoDeHuevosAlmacenados,
        FaltaDeHuevos,
        DisminucionDePoblacionDeSoldados,
        ClimaAdverso,
        RecolectorasOciosas,
        ExcesoDeComidaYLarvasHambrientas
    }

    //Nombramos en forma de Action el numero de acciones segun nuestra tabla de acciones
    public enum Action
    {
        AsignarConstructoras,
        AsignarRecolectoras,
        AsignarSoldados,
        AsignarOtroRolARecolectorasOciosas,
        AsignarObreras
    }

    //Funcion en la que vamos a controlar nuestras percepciones, y dependiendo de estas, haremos una accion u otra
    void ControlColony()
    {
        if (perceptions.CheckPerception(Perception.AtaqueALaColonia))
        {
            actions.ExecuteAction(Action.AsignarSoldados);
        }
        if (perceptions.CheckPerception(Perception.FaltaDeComida))
        {
            actions.ExecuteAction(Action.AsignarRecolectoras);
        }
        if (perceptions.CheckPerception(Perception.IncapacidadParaRecolectarComida))
        {
            actions.ExecuteAction(Action.AsignarOtroRolARecolectorasOciosas);
        }
        if (perceptions.CheckPerception(Perception.ExcesoDeHuevosAlmacenados))
        {
            actions.ExecuteAction(Action.AsignarConstructoras);
        }
        if (perceptions.CheckPerception(Perception.FaltaDeHuevos))
        {
            actions.ExecuteAction(Action.AsignarRecolectoras);
        }
        if (perceptions.CheckPerception(Perception.DisminucionDePoblacionDeSoldados))
        {
            actions.ExecuteAction(Action.AsignarSoldados);
        }
        if (perceptions.CheckPerception(Perception.ClimaAdverso))
        {
            actions.ExecuteAction(Action.AsignarOtroRolARecolectorasOciosas);
        }
        if (perceptions.CheckPerception(Perception.RecolectorasOciosas))
        {
            actions.ExecuteAction(Action.AsignarOtroRolARecolectorasOciosas);
        }
        if (perceptions.CheckPerception(Perception.ExcesoDeComidaYLarvasHambrientas))
        {
            actions.ExecuteAction(Action.AsignarObreras);
        }
    }


    // Métodos de percepción
    public bool IsUnderAttack() { /* Implementación */ return false; }
    public bool IsFoodInsufficient() { /* Implementación */ return false; }
    public bool AreGatherersUnableToCollectFood() { /* Implementación */ return false; }
    public bool AreThereTooManyStoredEggs() { /* Implementación */ return false; }
    public bool IsThereALackOfEggs() { /* Implementación */ return false; }
    public bool IsSoldierPopulationDecreasing() { /* Implementación */ return false; }
    public bool IsAdverseWeather() { /* Implementación */ return false; }
    public bool AreGatherersIdle() { /* Implementación */ return false; }
    public bool IsThereExcessFoodAndHungryLarvae() { /* Implementación */ return false; }

    // Métodos de acción
    public void AssignMoreBuilders() { /* Implementación */ }
    public void AssignMoreGatherers() { /* Implementación */ }
    public void AssignMoreSoldiers() { /* Implementación */ }
    public void AssignNewRoleToIdleGatherers() { /* Implementación */ }
    public void AssignMoreWorkers() { /* Implementación */ }


    //FUNCIONES SECUNDARIAS DE LA COLONIA
    private void Start()
    {
        weatherFavorable = true;
        auxiliarSoldierCount = 0;
        buttonStartSimulation.onClick.AddListener(IniciarSimulacion);
        this.enabled = false; // Desactivar el script al inicio
        gameManager.SetActive(false); // Desactivar el GameManager al inicio
        mapScript.enabled = false; // Desactivar el script Map al inicio
        predatorManagerScript.enabled = false;

        // Add listeners to update text values when sliders change
        sliderGatherers.onValueChanged.AddListener(delegate { UpdateSliderValue(sliderGatherers, textGatherersValue); });
        sliderWorkers.onValueChanged.AddListener(delegate { UpdateSliderValue(sliderWorkers, textWorkersValue); });
        sliderSoldiers.onValueChanged.AddListener(delegate { UpdateSliderValue(sliderSoldiers, textSoldiersValue); });
        sliderPredators.onValueChanged.AddListener(delegate { UpdateSliderValue(sliderPredators, textPredatorsValue); });
        sliderEscarabajos.onValueChanged.AddListener(delegate { UpdateSliderValue(sliderEscarabajos, textEscarabajosValue); });

        // Initialize text values
        UpdateSliderValue(sliderGatherers, textGatherersValue);
        UpdateSliderValue(sliderWorkers, textWorkersValue);
        UpdateSliderValue(sliderSoldiers, textSoldiersValue);
        UpdateSliderValue(sliderPredators, textPredatorsValue);
        UpdateSliderValue(sliderEscarabajos, textEscarabajosValue);
    }

    private void IniciarSimulacion()
    {
        initial_Gatherers = (int)sliderGatherers.value;
        initial_Workers = (int)sliderWorkers.value;
        initial_Soldiers = (int)sliderSoldiers.value;
        initial_Predators = (int)sliderPredators.value;
        initial_Beetles = (int)sliderEscarabajos.value;

        // Inicialización de la simulación
        for (int i = 0; i < initial_Gatherers; i++)
        {
            GenerateAnt(AntManager.Role.Gatherer);
        }

        for (int i = 0; i < initial_Workers; i++)
        {
            GenerateAnt(AntManager.Role.Worker);
        }

        for (int i = 0; i < initial_Soldiers; i++)
        {
            GenerateAnt(AntManager.Role.Soldier);
        }

        GenerateAnt(AntManager.Role.Queen);

        //depredadores
        predatorManager.initialNumPredators = initial_Predators;
        StartCoroutine(predatorManager.GeneratePredatorsOverTime(initial_Predators));

        //Escarabajos
        escarabajoManager.initialNumBeetles = initial_Beetles;
        StartCoroutine(escarabajoManager.GenerateBeetleOverTime(initial_Beetles));

        // Ocultar el canvas de configuración
        GameObject configCanvas = GameObject.Find("ConfigCanvas");
        if (configCanvas != null)
        {
            configCanvas.SetActive(false);
        }

        // Activar el script
        this.enabled = true;

        // Activar el GameManager
        gameManager.SetActive(true);

        // Activar el script Map
        mapScript.enabled = true;

        // Activar el script predatorManaher
        predatorManagerScript.enabled = true;
    }

    private void Update()
    {
        ManageColony();
        //Controlamos
        ControlColony();


        //auxiliar a antManager
        antManager.weatherFavorable = weatherFavorable;
    }

    private void ManageColony()
    {
        int gathererNumber = antManager.antGathererObjectList.Count;
        int workerNumber = antManager.antWorkerObjectList.Count;
        int soldierNumber = antManager.antSoldierObjectList.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int inMapResources = map.GetComponent<Map>().unasignedResources.Count;
        int predatorNumber = predatorManager.predators.Count;
        int beetleNumber = escarabajoManager.beetlesList.Count;

        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");
        int soldierLarvaCount = CountPendingLarvas("Soldier");

        AuxiliarControl(gathererNumber, workerNumber, soldierNumber, gathererLarvaCount, workerLarvaCount, resources, inMapResources, predatorNumber, soldierLarvaCount, beetleNumber);

        string type = DecideAntType(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount);

        AssignLarvaType(type);
    }

    private void AuxiliarControl(int gathererNumber, int workerNumber, int soldierNumber, int gathererLarvaCount, int workerLarvaCount, int resources, int inMapResources, int predatorCount, int soldierLarvaCount, int beetleCount)
    {
        storageResources = resources;
        mapResources = inMapResources;
        totalGatherers = gathererNumber;
        totalWorkers = workerNumber;
        totalSoldiers = soldierNumber;
        totalLarvaGatherers = gathererLarvaCount;
        totalLarvaWorkers = workerLarvaCount;
        totalLarvaSoldiers = soldierLarvaCount;
        totalPredators = predatorCount;
        totalBeetles = beetleCount;
        population = totalGatherers + totalWorkers + totalLarvaGatherers + totalLarvaWorkers;
    }

    private string DecideAntType(int gathererNumber, int workerNumber, int gathererLarvaCount, int workerLarvaCount)
    {
        if (gathererNumber == 0 && gathererLarvaCount == 0)
        {
            return "Gatherer";
        }
        else if (gathererNumber > workerNumber && workerLarvaCount == 0)
        {
            return "Worker";
        }
        else if (gathererNumber >= 3 && workerNumber >= 3)
        {
            return "Soldier";
        }
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
        foreach (var larva in antManager.antLarvaList)
        {
            if (string.IsNullOrEmpty(larva.GetComponent<AntLarva>().type))
            {
                larva.GetComponent<AntLarva>().type = antType;
                break;
            }
        }
    }

    private int CountPendingLarvas(string antType)
    {
        int count = 0;
        foreach (var larva in antManager.antLarvaList)
        {
            if (larva.GetComponent<AntLarva>().type == antType)
            {
                count++;
            }
        }

        return count;
    }

    private void UpdateSliderValue(Slider slider, TMP_Text text)
    {
        text.text = slider.value.ToString();
    }

    //Funciones Auxiliares para el control desde los botones

    public void GenerateAnt(AntManager.Role role)
    {
        switch (role)
        {
            case AntManager.Role.Gatherer:
                antManager.GenerateAnt(0, -5, role);
                break;
            case AntManager.Role.Worker:
                antManager.GenerateAnt(-17, -6, role);
                break;
            case AntManager.Role.Soldier:
                antManager.GenerateAnt(-24, -18, role);
                break;
            case AntManager.Role.Queen:
                antManager.GenerateAnt(50, -13, role);
                break;
            default:
                Debug.LogWarning("Unknown ant role: " + role);
                break;
        }
    }

    public void GenerateBeetle()
    {
        escarabajoManager.GenerateBeetle();
    }

    public void GeneratePredator()
    {
        predatorManager.GeneratePredatorAtSpawn();
    }

    public void GenerateResource()
    {
        map.GenerateResource();
    }

    public void DecreaseAnt(AntManager.Role role)
    {
        switch (role)
        {
            case AntManager.Role.Gatherer:
                try
                {
                    antManager.antGathererObjectList[0].GetComponent<AntGatherer>().isDead = true; //isDead para liberar el recurso
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.Log("No existen elementos que eliminar!");
                }
                break;
            case AntManager.Role.Worker:
                try
                {
                    antManager.antWorkerObjectList[0].GetComponent<AntWorker>().Die(); //DIE
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.Log("No existen elementos que eliminar!");
                }
                break;
            case AntManager.Role.Soldier:
                try
                {
                    antManager.antSoldierObjectList[0].GetComponent<AntSoldier>().Die(); //DIE
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Debug.Log("No existen elementos que eliminar!");
                }
                break;
            case AntManager.Role.Queen:
                antManager.GenerateAnt(20, -7, role);
                break;
            default:
                Debug.LogWarning("Unknown ant role: " + role);
                break;
        }
    }

    public void DecreasePredator()
    {
        try
        {
            predatorManager.KillPredator(predatorManager.predators[0].GetComponent<Predator>()); //isDead para liberar el recurso
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.Log("No existen elementos que eliminar!");
        }
    }
    public void DecreaseBeetle()
    {
        try
        {
            escarabajoManager.KillBeetle(escarabajoManager.beetlesList[0]); //isDead para liberar el recurso
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.Log("No existen elementos que eliminar!");
        }
    }

    public void DecreaseResource()
    {
        try
        {
            map.DeleteResource(); //isDead para liberar el recurso
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.Log("No existen elementos que eliminar!");
        }
    }

}
