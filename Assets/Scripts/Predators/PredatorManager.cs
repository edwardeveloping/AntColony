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
        for (int i= 0;i < predators.Count;i++)
        {
            if (predators[i].hungry <= 0)
            {
                Destroy(predators[i].gameObject);
                predators[i] = null;

                //reajustamos la lista
                List<Predator> auxList = new List<Predator>();
                for (int j= 0;j < predators.Count;j++)
                {
                    if (predators[j] != null)
                    {
                        auxList.Add(predators[j]);
                    }
                }

                //Actualizamos lista de depredadores y vaciamos la anterior
                predators.Clear();

                for (int j= 0; j < auxList.Count; j++)
                {
                    predators.Add(auxList[j]);
                }

                auxList.Clear();
            }
        }

    }
}
