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
            Debug.Log("Tried to kill an ant, but it was not found in antObjectList");
            return false;
        }

        Destroy(predatorToKill.gameObject); // Destroy game object.

        //Debug.Log("AntGathererList size: " + antGathererObjectList.Count + ", AntList size: " + antObjectList.Count);
        return true;
    }
}
