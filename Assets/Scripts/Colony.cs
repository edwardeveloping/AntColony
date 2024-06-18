using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Colony : MonoBehaviour
{
    [SerializeField] AntManager antManager;
    [SerializeField] PredatorManager predatorManager;
    [SerializeField] EscarabajoManage escarabajoManager;

    int shells;

    public Room storageRoom;

    //variables para controlar desde el menu de Unity compilando el programa
    private int initial_Gatherers;
    private int initial_Workers;
    private int initial_Soldiers;
    private int initial_Predators;
    private int initial_Escarabajos;
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

    private void Start()
    {
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
        initial_Escarabajos = (int)sliderEscarabajos.value;

        // Inicialización de la simulación
        for (int i = 0; i < initial_Gatherers; i++)
        {
            antManager.GenerateAnt(0, -5, AntManager.Role.Gatherer);
        }

        for (int i = 0; i < initial_Workers; i++)
        {
            antManager.GenerateAnt(-17, -6, AntManager.Role.Worker);
        }

        for (int i = 0; i < initial_Soldiers; i++)
        {
            antManager.GenerateAnt(-10, -10, AntManager.Role.Soldier);
        }

        antManager.GenerateAnt(20, -7, AntManager.Role.Queen);

        //depredadores
        predatorManager.initialNumPredators = initial_Predators;
        StartCoroutine(predatorManager.GeneratePredatorsOverTime(initial_Predators));

        //Escarabajos
        escarabajoManager.initialNumBeetles = initial_Escarabajos;
        StartCoroutine(escarabajoManager.GenerateBeetleOverTime(initial_Escarabajos));

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
    }

    private void ManageColony()
    {
        int gathererNumber = antManager.antGathererObjectList.Count;
        int workerNumber = antManager.antWorkerObjectList.Count;
        int soldierNumber = antManager.antSoldierObjectList.Count;
        int resources = storageRoom.GetComponent<Room>().count;
        int predatorNumber = predatorManager.predators.Count;

        int gathererLarvaCount = CountPendingLarvas("Gatherer");
        int workerLarvaCount = CountPendingLarvas("Worker");
        int soldierLarvaCount = CountPendingLarvas("Soldier");

        AuxiliarControl(gathererNumber, workerNumber, soldierNumber, gathererLarvaCount, workerLarvaCount, resources, predatorNumber, soldierLarvaCount);

        string type = DecideAntType(gathererNumber, workerNumber, gathererLarvaCount, workerLarvaCount);

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
}
