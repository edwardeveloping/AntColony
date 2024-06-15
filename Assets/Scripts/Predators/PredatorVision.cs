using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{
    public bool insideVision;
    public GameObject ant;

    private void Start()
    {
        insideVision = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ant")
        {
            insideVision = true;
            ant = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == ant)
        {
            insideVision = false;
        }
    }

    private void Update()
    {
        if (insideVision)
        {
            transform.parent.GetComponent<Predator>().antTarget = ant;
        }

        else
        {
            transform.parent.GetComponent<Predator>().antTarget = null;
        }
        
    }
}
