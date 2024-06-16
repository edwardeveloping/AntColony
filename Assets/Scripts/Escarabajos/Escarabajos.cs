/*
using System.Collections;
using UnityEngine;

public class Escarabajos : MovableObject
{
    public Map map;
    public float cooldownTime = 5f;
    public GameObject assignedResource;

    private Vector2 randomPos;
    private float hungry;
    private bool readyToEat = true;

    // Sprite
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Ángulo de corrección para alinear correctamente el sprite
    private float anguloCorreccion;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        hungry = 100;
        randomPos = map.RandomPositionInsideBounds();
        anguloCorreccion = -90f;
        SetNewTargetPosition();
    }

    private void SetNewTargetPosition()
    {
        randomPos = map.RandomPositionInsideBounds();
    }

    private void Update()
    {
        hungry -= Time.deltaTime * 8;

        if (readyToEat && assignedResource != null)
        {
            MoveTo(assignedResource.transform.position);
            if (Vector3.Distance(transform.position, assignedResource.transform.position) < 0.1f)
            {
                EatResource(assignedResource);
            }
        }
        else
        {
            CheckPosition();
            MoveTo(randomPos);
        }

        SpriteMove();
    }

    private void CheckPosition()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        if (currentPos == randomPos)
        {
            randomPos = map.RandomPositionInsideBounds();
        }
    }

    private void EatResource(GameObject resource)
    {
        Destroy(resource);
        readyToEat = false;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(15);
        readyToEat = true;
        assignedResource = null;
        SetNewTargetPosition();
    }

    private void SpriteMove()
    {
        // Mover hacia el destino
        if (randomPos != null)
        {
            Vector3 direccion = (randomPos - (Vector2)transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                angulo += anguloCorreccion;
                transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
        }

        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
}
*/

using System.Collections;
using UnityEngine;

public class Escarabajos : MovableObject
{
    public Map map;
    public float searchInterval = 15f; // Intervalo para buscar recursos
    public GameObject assignedResource;

    private Vector2 randomPos;
    private bool readyToEat = true;

    // Sprite
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Ángulo de corrección para alinear correctamente el sprite
    private float anguloCorreccion;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        randomPos = map.RandomPositionInsideBounds();
        anguloCorreccion = -90f;

        // Iniciar la rutina para buscar recursos cada cierto tiempo
        StartCoroutine(SearchForResourceRoutine());
    }

    private void Update()
    {
        if (readyToEat && assignedResource != null)
        {
            MoveTo(assignedResource.transform.position);
            if (Vector3.Distance(transform.position, assignedResource.transform.position) < 0.1f)
            {
                EatResource(assignedResource);
            }
        }
        else
        {
            CheckPosition();
            MoveTo(randomPos);
        }

        // Actualizar la animación del sprite
        SpriteMove();
    }

    private void CheckPosition()
    {
        if (Vector2.Distance(transform.position, randomPos) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    private void SetNewTargetPosition()
    {
        randomPos = map.RandomPositionInsideBounds();
    }

    private IEnumerator SearchForResourceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(searchInterval);
            LookForResource();
        }
    }

    public void LookForResource()
    {
        StartCoroutine(LookForResourceCor());
    }

    IEnumerator LookForResourceCor()
    {
        while (assignedResource == null)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                yield break;
            }
            yield return new WaitForSeconds(1f); // Esperar un segundo antes de volver a buscar
        }
    }

    private void EatResource(GameObject resource)
    {
        Destroy(resource);
        Debug.Log("Destruido");
        readyToEat = false;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(15); // Tiempo de espera antes de que el escarabajo esté listo para comer de nuevo
        readyToEat = true;
        assignedResource = null;
        SetNewTargetPosition();
    }

    private void SpriteMove()
    {
        // Mover hacia el destino
        if (randomPos != null)
        {
            Vector3 direccion = (randomPos - (Vector2)transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                angulo += anguloCorreccion;
                transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
        }

        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
}