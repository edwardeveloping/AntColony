using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExteriorArea : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Predator"))
        {
            //activamos el outSideBounds para que vuelva dentro de la zona
            collision.gameObject.GetComponent<Predator>().outSideBounds = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Predator"))
        {
            //apagamos el outSideBounds
            collision.gameObject.GetComponent<Predator>().outSideBounds = false;
        }
    }
}
