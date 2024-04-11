using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PredatorManager : MonoBehaviour
{
    [SerializeField] int initialNumPredators;

    [SerializeField] AntManager antManager;
    [SerializeField] GameObject predatorPrefab;

    [SerializeField] Map map;

    [SerializeField] Transform predatorSpawn;

    List<Predator> predators = new List<Predator>();


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
        return predator;
    }

    public GameObject RequestTarget()
    {
        return null;
    }
}
