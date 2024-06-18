using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ControlGeneratorPad : MonoBehaviour
{
    public GameObject reference;


    void Start()
    {
        List<string> namePads = new List<string> { "Gatherer", "Worker", "Soldier", "Predator", "Beetle"};

        // Obtener la posición de referencia
        Vector3 position = reference.transform.position;

        foreach (string namePad in namePads)
        {
            // Instanciar un nuevo objeto basado en reference
            GameObject aux = Instantiate(reference, position, Quaternion.identity);

            // Asignar el padre correcto a la nueva instancia
            aux.transform.SetParent(reference.transform.parent);
            aux.GetComponent<ControlPad>().typePad = namePad;

            position.y -= 50;
        }

        Destroy(reference); //destruimos referencia
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
