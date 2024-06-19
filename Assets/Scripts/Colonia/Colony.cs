using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BehaviourAPI.Core.Perceptions;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static AntManager;
using static AntWorker;

public class Colony : MonoBehaviour
{
    [SerializeField] AntManager antManager;
    [SerializeField] PredatorManager predatorManager;
    [SerializeField] EscarabajoManage escarabajoManager;
    [SerializeField] Map map;

    int shells;
    public bool inDanger;
    public bool activeDanger;

    public Room storageRoom;

    //variables para controlar desde el menu de Unity compilando el programa
    
    private int initial_Gatherers;
    private int initial_Workers;
    private int initial_Soldiers;
    private int initial_Predators;
    private int initial_Beetles;
    public GameObject myQueen;
    public float queenLife;
    public int population;
    public int totalGatherers;
    public int totalWorkers;
    public int totalSoldiers;
    public int totalLarvas;
    public int totalLarvaGatherers;
    public int totalLarvaWorkers;
    public int totalLarvaSoldiers;
    public int totalPredators;
    public int totalBeetles;
    public int storageResources;
    public int mapResources;
    
    

    // Atributos de la colonia
    public int minimumFoodThreshold;
    public int larvaFoodRequirement; // Cantidad de comida que requiere cada larva
    public float minimumQueenHealth;
    public float minimumLarvaEggsPerIndividual;

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
    public GameObject controlInGame;
    public MonoBehaviour mapScript;
    public MonoBehaviour predatorManagerScript;

    //Acciones y percepciones
    public ColonyPerceptions perceptions;
    public ColonyActions actions;
    List<PerceptionWithPriority> perceptionWPList = new List<PerceptionWithPriority>();

    //COOLDOWNS => Para que la colonia no se sature, meteremos cooldowns y asi se podra evaluar las cosas pasado cierto tiempo
    private float initCooldown;
    public float cooldown; // Cooldown de 3 minutos

    //------------------------------------------------------------------------------------------------------------------------------- FUNCIONES PRINCIPALES DE LA COLONIA

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
        //AsignarConstructoras => Lo decidimos quitar
        AsignarRecolectoras,
        AsignarSoldados,
        AsignarOtroRolARecolectorasOciosas,
        AsignarObreras 
    }

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

        //ControlInGame
        controlInGame.SetActive(false);
        activeDanger = false;
        cooldown = 10f;
        initCooldown = 10f;
        minimumFoodThreshold = 2;
        larvaFoodRequirement = 1; // Cantidad de comida que requiere cada larva
        minimumQueenHealth = 90f;
        minimumLarvaEggsPerIndividual = 5f; //Quiere decir que, por cada 5 individuos de la colonia (independientemente del tipo de hormigas que sean) nos hara falta 1 larva
}

    private void Update()
    {
        ManageColony();
        //Controlamos
        ControlColony();
        Cooldown();

        //auxiliar a antManager
        antManager.weatherFavorable = weatherFavorable;

        if (inDanger)
        {
            activeDanger = true;
        }

        if (!inDanger && activeDanger)
        {
            ReorganizeSoldiers();
            activeDanger = false;
        }

        queenLife = myQueen.GetComponent<AntQueen>().salud;
    }

    

    //Funcion en la que vamos a controlar nuestras percepciones, y dependiendo de estas, haremos una accion u otra
    void ControlColony()
    {
        if (cooldown == 0) //AQUI EJECUTAREMOS AQUELLAS PERCEPCIONES CON PRIORIDAD PARA LA COLONIA, NO AQUELLAS EXTREMAS
        {
            CheckForPerceptions(); //Añadimos las percepciones que se cumplan

            if (perceptionWPList.Count > 0)
            {
                // Ordenar la lista de percepciones por prioridad (mayor prioridad primero)
                perceptionWPList.Sort((p1, p2) => p2.priority.CompareTo(p1.priority));

                // Ejecutar la percepción de mayor prioridad
                PerceptionWithPriority perceptionWithPriority = perceptionWPList[0];
                perceptionWPList.RemoveAt(0);

                //Ejecutamos la percepcion
                ExecutePerception(perceptionWithPriority.perception);

                //Reiniciamos el cooldown
                cooldown = initCooldown;
            }
        }

        //PERCEPCIONES EXTREMAS => SIEMPRE QUE SE PRODUZCAN SE EJECUTAN, INDEPENDIENTEMENTE UNAS ENTRE OTRAS Y SIN PRIORIDAD NI COOLDOWN

        if (perceptions.CheckPerception(Perception.AtaqueALaColonia))
        {
            actions.ExecuteAction(Action.AsignarSoldados);
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

    //PERCEPCIONES CON PRIORIDAD

    void CheckForPerceptions()
    {
        //Añadimos todas las percepciones con prioridad, solo se añadiran aquellas que sean true
        // PRIORITY => CUANTO MAS ALTO, ANTES SE INCLUIRA EN LA LISTA => 5, 4, 3, 2, 1

        AddPerception(Perception.FaltaDeComida, 5, IsFoodInsufficient());
        AddPerception(Perception.ExcesoDeComidaYLarvasHambrientas, 4, IsThereExcessFoodAndHungryLarvas());
        AddPerception(Perception.FaltaDeHuevos, 3, IsThereALackOfEggs());
        AddPerception(Perception.ExcesoDeHuevosAlmacenados, 2, AreThereTooManyStoredEggs());
        AddPerception(Perception.IncapacidadParaRecolectarComida, 1, AreGatherersUnableToCollectFood());
    }

    void ExecutePerception(Perception perception)
    {
        //Ejecutamos la prioridad
        switch (perception)
        {
            case Perception.FaltaDeComida:
                WriteColony("POCA COMIDA, REORGANIZANDO GATHERERS");
                actions.ExecuteAction(Action.AsignarRecolectoras);
                break;

            case Perception.IncapacidadParaRecolectarComida:
                WriteColony("INCAPACIDAD PARA RECOLECTAR COMIDA, REORGANIZANDO GATHERERS");
                actions.ExecuteAction(Action.AsignarOtroRolARecolectorasOciosas);
                break;

            case Perception.ExcesoDeHuevosAlmacenados:
                WriteColony("EXCESO DE HUEVOS, REORGANIZANDO WORKERS");
                actions.ExecuteAction(Action.AsignarObreras);
                break;

            case Perception.FaltaDeHuevos:
                WriteColony("FALTA DE HUEVOS, ¿TENEMOS RECURSOS?");
                
                //Comprobar si tenemos recursos
                if (storageResources > 2) 
                {
                    WriteColony("Tenemos recursos, es necesario asignar mas WORKERS");
                    actions.ExecuteAction(Action.AsignarObreras);
                }
                else 
                {
                    WriteColony("No tenemos recursos, es necesario asignar mas GATHERERS");
                    actions.ExecuteAction(Action.AsignarObreras);
                }
                break;

            case Perception.ExcesoDeComidaYLarvasHambrientas:
                WriteColony("EXCESO DE COMIDA Y LARVAS HAMBRIENTAS, REORGANIZANDO WORKERS");
                actions.ExecuteAction(Action.AsignarObreras);
                break;

        }
    }
    void AddPerception(Colony.Perception perception, int priority, bool condition)
    {
        //Si se cumple la condicion, es decir, que la percepcion a de ser realizada, entonces la colonia la añadira a la lista
        if (condition && !perceptionWPList.Exists(p => p.perception == perception))
        {
            perceptionWPList.Add(new PerceptionWithPriority(perception, priority));
        }
    }

    // PERCEPCIONES
    public bool IsUnderAttack() { return inDanger; }
    public bool IsFoodInsufficient()
    {
        // Verificar si ha pasado el cooldown
        if (!weatherFavorable)
        {
            return false;
        }

        // Verificar si los recursos de almacenamiento son insuficientes
        bool storageResourcesLow = storageResources < minimumFoodThreshold;

        // Calcular la cantidad total de comida requerida por las larvas
        //int totalLarvaFoodRequirement = totalLarvas * larvaFoodRequirement;

        // Verificar si la comida es insuficiente considerando las larvas y la vida de la reina
        bool foodLowForLarvasAndQueen = storageResources < minimumFoodThreshold//(totalLarvaFoodRequirement + minimumFoodThreshold)
                                        && myQueen.GetComponent<AntQueen>().salud < minimumQueenHealth;

        // Considerar la falta de comida si cualquiera de las condiciones es verdadera
        bool isFoodInsufficient = storageResourcesLow && foodLowForLarvasAndQueen;

        // Considerar la falta de comida si cualquiera de las condiciones es verdadera
        return isFoodInsufficient;


    }
    public bool AreGatherersUnableToCollectFood() { /* Implementación */ return false; }
    public bool AreThereTooManyStoredEggs() { /* Implementación */ return false; }
    public bool IsThereALackOfEggs() 
    {
        int totalPopulation = population + totalLarvas;

        if (totalPopulation == 0)
        {
            return false; // No hay población para comparar
        }

        float requiredLarvas = population / minimumLarvaEggsPerIndividual;

        return totalLarvas < requiredLarvas;
    }
    public bool IsSoldierPopulationDecreasing() { /* Implementación */ return false; }
    public bool IsAdverseWeather() { return !weatherFavorable; }
    public bool AreGatherersIdle() 
    {
        int contador = 0;
        foreach (GameObject gatherer in antManager.antGathererObjectList)
        {
            //Operador ? para ir practicando 
            contador += gatherer.GetComponent<AntGatherer>().isIdle ? 1 : 0;
        }

        //return manteniendo una hormiga idle por lo menos por si acaso
        return contador > 1;
    }
    public bool IsThereExcessFoodAndHungryLarvas() { /* Implementación */ return false; }



    // ACCIONES
    public void AssignMoreGatherers() 
    {
        List<GameObject> soldiersToTransform = new List<GameObject>();
        List<GameObject> workersToTransform = new List<GameObject>();

        int soldierCount = antManager.antSoldierObjectList.Count;
        int workerCount = antManager.antWorkerObjectList.Count;

        // QUEREMOS TRANSFORMAR DOS HORMIGAS SOLO, SI SE NECESITAN MAS LA COLONIA SE ENCARGARA DE VOLVER A LLAMAR AL METODO, SI TENEMOS 2 SOLDADOS 2 SOLDADOS
        // SI NO 2 WORKERS Y SI NO 1 DE CADA, Y SI NO NINGUNA
        if (soldierCount > 1)
        {
            // Transformar 2 soldados en recolectoras
            for (int i = 1; i <= 2 && i < soldierCount; i++)
            {
                soldiersToTransform.Add(antManager.antSoldierObjectList[i]);
            }
        }
        else if (soldierCount == 1)
        {
            // Transformar 1 soldado y 1 trabajador en recolectoras (si hay trabajadores)
            soldiersToTransform.Add(antManager.antSoldierObjectList[0]);
            if (workerCount > 0)
            {
                // Filtrar trabajadores con recursoCargado == Recurso.Nada
                foreach (var worker in antManager.antWorkerObjectList)
                {
                    AntWorker workerComponent = worker.GetComponent<AntWorker>();
                    if (workerComponent != null && workerComponent.recursoCargado == Recurso.Nada)
                    {
                        workersToTransform.Add(worker);
                        break;
                    }
                }
            }
        }
        else if (workerCount > 0)
        {
            // Transformar 2 trabajadores en recolectoras si hay más de 1, sino transformar solo 1
            int count = 0;
            foreach (var worker in antManager.antWorkerObjectList)
            {
                AntWorker workerComponent = worker.GetComponent<AntWorker>();
                //AQUELLAS QUE NO TENGAN RECURSOS CARGADOS
                if (workerComponent != null && workerComponent.recursoCargado == Recurso.Nada)
                {
                    workersToTransform.Add(worker);
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
        }
        else if (soldierCount == 0 && workerCount == 0)
        {
            Debug.Log("NO PUEDO TRANSFORMAR HORMIGAS EN GATHERERS PORQUE NO HAY");
        }

        // Transformar los soldados seleccionados en recolectoras
        foreach (GameObject soldierObj in soldiersToTransform)
        {
            AntSoldier soldier = soldierObj.GetComponent<AntSoldier>();
            if (soldier != null)
            {
                soldier.ChangeRole(AntManager.Role.Gatherer);
                antManager.antSoldierObjectList.Remove(soldierObj); // Actualizar la lista de soldados
            }
        }

        // Transformar los trabajadores seleccionados en recolectoras
        foreach (GameObject workerObj in workersToTransform)
        {
            AntWorker worker = workerObj.GetComponent<AntWorker>();
            if (worker != null)
            {
                worker.ChangeRole(AntManager.Role.Gatherer);
                antManager.antWorkerObjectList.Remove(workerObj); // Actualizar la lista de trabajadores
            }
        }
    }
    public void AssignMoreSoldiers() 
    {
        WriteColony("COLONIA BAJO ATAQUE ENEMIGO");
        WriteColony("REASIGNANDO SOLDADOS PARA LA DEFENSA");

        //Lista auxiliar para que no se tripie el foreach
        List<GameObject> gathererListCopy = new List<GameObject>(antManager.antGathererObjectList);

        foreach (GameObject gatherer in gathererListCopy)
        {
            gatherer.GetComponent<AntGatherer>().ChangeRole(AntManager.Role.Soldier);
        }
    }
    public void AssignNewRoleToIdleGatherers() 
    {
        //Creamos lista auxiliar para evitar iteraciones sobre elementos cambiantes donde almacenaremos aquellas que sean Idle
        var idleGatherers = antManager.antGathererObjectList
                             .Where(gatherer => gatherer.GetComponent<AntGatherer>().isIdle)
                             .ToList();

        //cambiamos las idle
        foreach (GameObject gatherer in idleGatherers)
        {
            WriteColony("REASIGNANDO HORMIGA IDLE");
            gatherer.GetComponent<AntGatherer>().ChangeRole(AntManager.Role.Worker);
        }
    }
    public void AssignMoreWorkers() 
    {
        List<GameObject> soldiersToTransform = new List<GameObject>();
        List<GameObject> gathererToTransform = new List<GameObject>();

        int soldierCount = antManager.antSoldierObjectList.Count;
        int gathererCount = antManager.antGathererObjectList.Count;

        // QUEREMOS TRANSFORMAR DOS HORMIGAS SOLO, SI SE NECESITAN MAS LA COLONIA SE ENCARGARA DE VOLVER A LLAMAR AL METODO, SI TENEMOS 2 SOLDADOS 2 SOLDADOS
        // SI NO 2 GATHERERS Y SI NO 1 DE CADA, Y SI NO NINGUNA
        if (soldierCount > 1)
        {
            // Transformar 2 soldados en workers
            for (int i = 1; i <= 2 && i < soldierCount; i++)
            {
                soldiersToTransform.Add(antManager.antSoldierObjectList[i]);
            }
        }
        else if (soldierCount == 1)
        {
            // Transformar 1 soldado y 1 gatherer en worker (si hay gatherer)
            soldiersToTransform.Add(antManager.antSoldierObjectList[0]);
            if (gathererCount > 0)
            {
                // Filtrar gatherer con comidaCargada
                foreach (var gatherer in antManager.antGathererObjectList)
                {
                    AntGatherer gathererComponent = gatherer.GetComponent<AntGatherer>();
                    if (gathererComponent != null && !gathererComponent.comidaCargada) //Si no lleva comida, la transformamos
                    {
                        gathererToTransform.Add(gatherer);
                        break;
                    }
                }
            }
        }
        else if (gathererCount > 0)
        {
            // Transformar 2 gatherers en workers si hay más de 1, sino transformar solo 1
            int count = 0;
            foreach (var gatherer in antManager.antGathererObjectList)
            {
                AntGatherer gathererComponent = gatherer.GetComponent<AntGatherer>();
                //AQUELLAS QUE NO TENGAN RECURSOS CARGADOS
                if (gathererComponent != null && !gathererComponent.comidaCargada)
                {
                    gathererToTransform.Add(gatherer);
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
        }
        else if (soldierCount == 0 && gathererCount == 0)
        {
            Debug.Log("NO PUEDO TRANSFORMAR HORMIGAS EN GATHERERS PORQUE NO HAY");
        }

        // Transformar los soldados seleccionados en workers
        foreach (GameObject soldierObj in soldiersToTransform)
        {
            AntSoldier soldier = soldierObj.GetComponent<AntSoldier>();
            if (soldier != null)
            {
                soldier.ChangeRole(AntManager.Role.Worker);
                antManager.antSoldierObjectList.Remove(soldierObj); // Actualizar la lista de soldados
            }
        }

        // Transformar las gatherer seleccionados en workers
        foreach (GameObject gathererObj in gathererToTransform)
        {
            AntGatherer gatherer = gathererObj.GetComponent<AntGatherer>();
            if (gatherer != null)
            {
                gatherer.ChangeRole(AntManager.Role.Worker);
                antManager.antWorkerObjectList.Remove(gathererObj); // Actualizar la lista de trabajadores
            }
        }
    }


    //----------------------------------------------------------------------------------------------------------------------------- FUNCIONES SECUNDARIAS DE LA COLONIA

    private void Cooldown()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            cooldown = 0;
        }
    }
    void WriteColony(string s)
    {
        Debug.Log("COLONY: " + s);
    }
    void ReorganizeSoldiers()
    {
        int soldierCount = antManager.antSoldierObjectList.Count;

        // Si hay más de 1 soldado, realizar la transformación
        if (soldierCount > 1)
        {
            // Crear una lista temporal para los soldados a transformar
            List<GameObject> soldiersToTransform = new List<GameObject>();

            for (int i = 1; i < soldierCount; i++) // Empieza desde 1 para mantener 1 soldado en la reserva
            {
                soldiersToTransform.Add(antManager.antSoldierObjectList[i]);
            }

            // Transformar los soldados fuera del bucle para evitar modificar la lista mientras se itera
            for (int i = 0; i < soldiersToTransform.Count; i++)
            {
                GameObject soldierObj = soldiersToTransform[i];
                AntSoldier soldier = soldierObj.GetComponent<AntSoldier>();

                if (soldier != null)
                {
                    if ((i + 1) % 2 == 0) // Usamos (i + 1) porque estamos iterando desde el índice 0 en soldiersToTransform
                    {
                        soldier.ChangeRole(AntManager.Role.Gatherer);
                    }
                    else
                    {
                        soldier.ChangeRole(AntManager.Role.Worker);
                    }
                }
                else
                {
                    Debug.LogWarning("AntSoldier component not found on GameObject");
                }
            }

            // Remover los soldados transformados de la lista de soldados originales
            antManager.antSoldierObjectList = antManager.antSoldierObjectList.Take(1).ToList();
        }
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

        //controlinGame
        controlInGame.SetActive(true);

        // Activar el script Map
        mapScript.enabled = true;

        // Activar el script predatorManaher
        predatorManagerScript.enabled = true;

        //Queen
        myQueen = antManager.antQueen;
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
        population = totalGatherers + totalWorkers + totalSoldiers;
        totalLarvas = totalLarvaSoldiers + totalLarvaWorkers + totalLarvaGatherers;
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

    //---------------------------------------------------------------------------------------------------------------------------- FUNCIONES AUXILIARES DE LA COLONIA
    //Funciones para asignar botones del controlInGame
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


public class PerceptionWithPriority
{
    public Colony.Perception perception;
    public int priority;

    public PerceptionWithPriority(Colony.Perception _perception, int _priority)
    {
        perception = _perception;
        priority = _priority;
    }
}
