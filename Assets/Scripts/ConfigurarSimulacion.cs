using UnityEngine;
using UnityEngine.UI;

public class ConfigurarSimulacion : MonoBehaviour
{
    public Slider sliderWorkers;
    public Slider sliderGatherers;
    public Slider sliderPredators;
    public Slider sliderSoldiers;
    public Slider sliderEscarabajos;
    public Button buttonStartSimulation;

    public Colony colonyScript;

    private void Start()
    {
        buttonStartSimulation.onClick.AddListener(IniciarSimulacion);
        colonyScript = FindObjectOfType<Colony>();
        colonyScript.enabled = false; // Asegúrate de que el script Colony esté desactivado al inicio
    }

    public void IniciarSimulacion()
    {
        int numWorkers = (int)sliderWorkers.value;
        int numGatherers = (int)sliderGatherers.value;
        int numPredators = (int)sliderPredators.value;
        int numSoldiers = (int)sliderSoldiers.value;
        int numEscarabajos = (int)sliderEscarabajos.value;

        // Guarda estos valores en un lugar accesible para el script Colony
        SimulationConfig.NumWorkers = numWorkers;
        SimulationConfig.NumGatherers = numGatherers;
        SimulationConfig.NumPredators = numPredators;
        SimulationConfig.NumSoldiers = numSoldiers;
        SimulationConfig.NumEscarabajos = numEscarabajos;

        // Activa el script Colony
        colonyScript.enabled = true;

        // Oculta el canvas de configuración
        gameObject.SetActive(false);
    }
}

public static class SimulationConfig
{
    public static int NumWorkers;
    public static int NumGatherers;
    public static int NumPredators;
    public static int NumSoldiers;
    public static int NumEscarabajos;
}