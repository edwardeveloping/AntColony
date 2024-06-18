using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarvaPredator : MonoBehaviour
{

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;
    [SerializeField] public PredatorManager predatorManager;
    //
    private float flipTime;
    private float flipTimeActual;

    // Start is called before the first frame update
    void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.5f;
        flipTimeActual = flipTime;
    }

    private void Awake()
    {
        StartCoroutine(Born());
    }

    IEnumerator Born()
    {
        Debug.Log("Crecimiento de larva de avispa...");
        yield return new WaitForSeconds(5f);
        Debug.Log("Nacimiento de avispa");
        predatorManager.GeneratePredatorAtSpawn();
        //predatorManager.KillLarvaPredator(this);
    }

    // Update is called once per frame
    void Update()
    {
        SpriteMove();
    }

    private void SpriteMove()
    {
        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
}
