using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExteriorArea : MonoBehaviour
{
    /*
    [SerializeField] public PredatorManager predatorManager;
    public List<Predator> thisPredatorList = new List<Predator> ();

    // Start is called before the first frame update
    void Start()
    {
        thisPredatorList = predatorManager.predators;
    }

    // Update is called once per frame
    void Update()
    {
        thisPredatorList = predatorManager.predators;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // DEPREDADORES
        //Comprobar ai el depredador esta fuera del mapa

        foreach (Predator predator in thisPredatorList)
        {
            if (collision == predator)
            {
                predator.GetComponent<Predator>().outSideBounds = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // DEPREDADORES
        //Comprobar ai el depredador esta dentro del mapa

        foreach (Predator predator in thisPredatorList)
        {
            if (collision == predator)
            {
                predator.GetComponent<Predator>().outSideBounds = false;
            }
        }
    }

    */
}
