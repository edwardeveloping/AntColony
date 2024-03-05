using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PredatorManager : MonoBehaviour
{
    [SerializeField] AntManager antManager;
    [SerializeField] GameObject predatorPrefab;

    [SerializeField] Transform predatorSpawn;

    List<Predator> predators = new List<Predator>();


    public void Initialize()
    {
        predators.Add(GeneratePredator(7, 3));
        predators[0].ChaseTarget(antManager.antGathererObjectList[0]);
    }
    public Predator GeneratePredatorAtSpawn()
    {
        return GeneratePredator(predatorSpawn.position.x, predatorSpawn.position.y);
    }
    public Predator GeneratePredator(float x, float y)
    {
        Predator predator = Instantiate(predatorPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Predator>();
        predator.predatorManager = this;
        return predator;
    }

    public GameObject RequestTarget()
    {
        return null;
    }
}
