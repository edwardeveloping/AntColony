using UnityEngine;
using UnityEngine.SceneManagement;

public class ReiniciarSimulacion : MonoBehaviour
{
    public void Reiniciar()
    {
        // Cambiar a la escena de simulaci�n (aseg�rate de usar el nombre correcto de la escena)
        SceneManager.LoadScene("SampleScene");
    }
}