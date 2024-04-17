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

    public void Initialize()
    {
        antManager.GenerateAnt(0, 0, AntManager.Role.Gatherer);
        antManager.GenerateAnt(1, 0, AntManager.Role.Gatherer);
        GameObject queenObj = antManager.GenerateAnt(13, -5, AntManager.Role.Queen);

        // Escalar la reina
        float scaleFactor = 2f; // Factor de escala, puedes ajustarlo según sea necesario
        queenObj.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
    }
}
