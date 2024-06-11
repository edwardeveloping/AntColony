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
        Starvation();
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

    public void Starvation()
    {
        foreach (Predator predator in predators)
        {
            if (predator.hungry <= 0)
            {
                predators.Remove(predator); //Eliminamos de la lista
                Destroy(predator.gameObject); //Destruimos el depredador
            }
        }
    }

    public void KillPredator(Predator predatorToKill)
    {
        foreach (Predator predator in predators)
        {
            if (predatorToKill == predator)
            {
                predators.Remove(predator); //Eliminamos de la lista
                Destroy(predator.gameObject); //Destruimos el depredador
            }
        }
    }
}
