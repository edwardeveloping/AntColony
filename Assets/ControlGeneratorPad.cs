using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ControlGeneratorPad : MonoBehaviour
{
    public GameObject reference;
    public Colony _colony;
    public Map _map;
    private bool isShared;
    private GameObject button;
    public GameObject panelWeather;

    void Start()
    {
        List<string> namePads = new List<string> { "Gatherer", "Worker", "Soldier", "Predator", "Beetle", "Resource"};

        isShared = false;

        //referencia al boton
        button = this.transform.parent.GetChild(0).gameObject;

        // Obtener la posición de referencia
        Vector3 position = reference.transform.position;

        foreach (string namePad in namePads)
        {
            // Instanciar un nuevo objeto basado en reference
            GameObject aux = Instantiate(reference, position, Quaternion.identity);

            // Asignar el padre correcto a la nueva instancia
            aux.transform.SetParent(reference.transform.parent);
            aux.GetComponent<ControlPad>().typePad = namePad;
            aux.GetComponent<ControlPad>().map = _map;
            aux.GetComponent<ControlPad>().colony = _colony;

            position.y -= 50;
        }

        Destroy(reference); //destruimos referencia
        
    }


    //Funcion auxiliar para animacion de entrar o salir
    public void ButtonAnimation()
    {
        if (isShared)
        {
            this.transform.parent.GetComponent<Animator>().SetBool("In", false);
            Flip();
            isShared = false;
        }
        else
        {
            this.transform.parent.GetComponent<Animator>().SetBool("In", true);
            Flip();
            isShared = true;
        }
    }



    //funcion para flipear nuestro boton{
    private void Flip()
    {
        Vector3 scale = button.GetComponent<RectTransform>().localScale;

        scale.x *= -1;

        button.GetComponent<RectTransform>().localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
