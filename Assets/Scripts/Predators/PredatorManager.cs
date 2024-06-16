using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PredatorManager : MonoBehaviour
{
    public int initialNumPredators;

    [SerializeField] AntManager antManager;
    [SerializeField] GameObject predatorPrefab;

    [SerializeField] Map map;

    [SerializeField] Transform predatorSpawn;

    public List<Predator> predators = new List<Predator>();

    const int OVEROPULATED_THRESHOLD = 5;
    event Action _overPopulated = null; // When it gets called the ant soldiers get activated.

    public event Action OverPopulatedEvent { add { _overPopulated += value; } remove { _overPopulated -= value; } }

    public void Initialize()
    {
        for(int i= 0; i < initialNumPredators; i++)
        {
            GeneratePredatorAtSpawn();
        }
    }
    public Predator GeneratePredatorAtSpawn()
    {
        return GeneratePredator(predatorSpawn.position.x, predatorSpawn.position.y);
    }
    public Predator GeneratePredator(float x, float y)
    {
        Predator predator = Instantiate(predatorPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Predator>();
        predator.predatorManager = this;
        predator.map = map;

        //Añadimos a la lista
        predators.Add(predator);

        //
        if(predators.Count > OVEROPULATED_THRESHOLD) { _overPopulated?.Invoke(); }
        return predator;
    }

    public GameObject RequestTarget()
    {
        return null;
    }
    private void Update()
    {
        NoPredators();
    }

    public void NoPredators()
    {
        if (predators.Count <= 0)
        {
            for (int i = 0; i < initialNumPredators; i++)
            {
                GeneratePredatorAtSpawn();
            }
        }
    }

    public bool KillPredator(Predator predatorToKill)
    {
        // Remove ant from list.
        if (!predators.Remove(predatorToKill)) // Try to remove ant object from antObject list.
        {
            Debug.Log("Tried to kill a predator, but it was not found in predatorList");
            return false;
        }

        Destroy(predatorToKill.gameObject); // Destroy game object.

        return true;
    }

    //Funcion auxiliar para eliminar los targets del resto de depredadores y evitar errores
    public void CleanPredatorTarget(GameObject antTarget)
    {
        foreach (Predator predator in predators)
        {
            if (predator.antTarget != null && predator.antTarget == antTarget)
            {
                predator.antTarget = null;
            }
        }
    }
}
