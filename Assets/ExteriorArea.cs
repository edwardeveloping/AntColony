using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExteriorArea : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // DEPREDADORES
        //Comprobar si la hormiga que persiguen se sale del mapa para dejar de perseguirla
        if (collision.transform.CompareTag("Predator"))
        {
            collision.gameObject.GetComponent<Predator>().inVisionRange = false;
        }
    }
}
