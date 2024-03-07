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
    }
}
