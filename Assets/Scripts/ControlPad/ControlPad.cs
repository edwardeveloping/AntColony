using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlPad : MonoBehaviour
{
    [SerializeField] public Colony colony;
    [SerializeField] public Map map;

    public string typePad;
    TextMeshProUGUI numberElements;
    TextMeshProUGUI namePad;
    void Start()
    {
        //Inicializacion
        numberElements = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        namePad = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        namePad.alignment = TextAlignmentOptions.Center;
        numberElements.alignment = TextAlignmentOptions.Center;

        //Nombre
        namePad.text = typePad.ToUpper();
    }

    // Update is called once per frame
    void Update()
    {
        switch (typePad)
        {
            case "Gatherer":
                numberElements.text = colony.totalGatherers.ToString();
                break;
            case "Worker":
                numberElements.text = colony.totalWorkers.ToString();
                break;
            case "Soldier":
                numberElements.text = colony.totalSoldiers.ToString();
                break;
            case "Predator":
                numberElements.text = colony.totalPredators.ToString();
                break;
            case "Beetle":
                numberElements.text = colony.totalBeetles.ToString();
                break;
            case "Resource":
                numberElements.text = colony.mapResources.ToString();
                break;
            default:
                Debug.Log("Nombre del Pad erroneo");
                break;
        }
    }

    public void increaseElement()
    {
        switch (typePad)
        {
            case "Gatherer":
                colony.GenerateAnt(AntManager.Role.Gatherer);
                break;
            case "Worker":
                colony.GenerateAnt(AntManager.Role.Worker);
                break;
            case "Soldier":
                colony.GenerateAnt(AntManager.Role.Soldier);
                break;
            case "Predator":
                colony.GeneratePredator();
                break;
            case "Beetle":
                colony.GenerateBeetle();
                break;
            case "Resource":
                colony.GenerateResource();
                break;
            default:
                Debug.Log("Nombre del Pad erroneo");
                break;
        }
    }

    public void decreaseElement()
    {
        switch (typePad)
        {
            case "Gatherer":
                colony.DecreaseAnt(AntManager.Role.Gatherer);
                break;
            case "Worker":
                colony.DecreaseAnt(AntManager.Role.Worker);
                break;
            case "Soldier":
                colony.DecreaseAnt(AntManager.Role.Soldier);
                break;
            case "Predator":
                colony.DecreasePredator();
                break;
            case "Beetle":
                colony.DecreaseBeetle();
                break;
            case "Resource":
                colony.DecreaseResource();
                break;
            default:
                Debug.Log("Nombre del Pad erroneo");
                break;
        }
    }


}
