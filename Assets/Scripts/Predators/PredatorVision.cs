using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{

    Predator predator;

    // Start is called before the first frame update
    void Start()
    {
        predator = transform.parent.GetComponent<Predator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ant"))
        {
            predator.antTarget = collision.gameObject;
            predator.inVisionRange = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ant"))
        {
            predator.antTarget = null;
            predator.inVisionRange = false;
            
        }
    }
}
