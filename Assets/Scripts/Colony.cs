using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Colony : MonoBehaviour
{
    [SerializeField] AntManager antManager;

    int population;
    int resources;
    int shells;


    public void Initialize(string s)
    {
        if (s == "Init") //INICIALIZACION
        {
            for (int i = 0; i < 2; i++)
            {
                GenerateGatherer();
                GenerateWorker();
            }

            for (int i = 0; i < 2; i++)
            {
                GenerateLarva();
            }

            GenerateQueen();

        }

        if (s == "Gatherer")
        {
            GenerateGatherer();
        }

        if (s == "Worker")
        {
            GenerateWorker();
        }

        if (s == "Larva")
        {
            GenerateLarva();
        }

    }

    public void GenerateGatherer()
    {
        antManager.GenerateAnt(0, 0, AntManager.Role.Gatherer);
    }

    public void GenerateLarva()
    {
        antManager.GenerateAnt(0, 0, AntManager.Role.Larva);
    }

    public void GenerateWorker()
    {
        antManager.GenerateAnt(0, -5, AntManager.Role.Worker);
    }

    public void GenerateQueen()
    {
        GameObject queenObj = antManager.GenerateAnt(13, -5, AntManager.Role.Queen);
        //queenObj.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);

    }

}