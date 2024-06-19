using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleWeather : MonoBehaviour
{
    [SerializeField] public Colony colony;
    public GameObject panelWeather;
    public GameObject panelDanger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleWeatherController(bool isWeather)
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

    public void ToggleDangerController(bool noDanger)
    {
        if (noDanger)
        {
            panelDanger.SetActive(false);
            colony.inDanger = false;
        }
        else
        {
            panelDanger.SetActive(true);
            colony.inDanger = true;
        }
    }
}
