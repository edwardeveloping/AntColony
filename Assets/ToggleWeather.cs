using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleWeather : MonoBehaviour
{
    [SerializeField] public Colony colony;
    public GameObject panelWeather;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOperator(bool isWeather)
    {
        if  (isWeather)
        {
            panelWeather.SetActive(false);
            colony.weatherFavorable = true;
        }
        else
        {
            panelWeather.SetActive(true);
            colony.weatherFavorable = false;
        }
    }
}
