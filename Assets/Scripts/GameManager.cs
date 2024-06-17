using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Map map;
    [SerializeField] Colony colony;
    [SerializeField] PredatorManager predatorManager;

    void Start()
    {
        map.Initilize();
        //colony.Initialize("Init");
        //predatorManager.Initialize();
    }
}
