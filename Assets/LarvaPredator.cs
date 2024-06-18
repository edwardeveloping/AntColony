using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarvaPredator : MonoBehaviour
{

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

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
