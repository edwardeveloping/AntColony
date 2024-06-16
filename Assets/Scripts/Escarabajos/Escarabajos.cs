using System.Collections;
using UnityEngine;

public class Escarabajos : MovableObject
{
    public Map map;
    public float searchInterval = 10f; // Intervalo para buscar recursos
    public GameObject assignedResource;

    private Vector2 randomPos;
    private bool readyToEat = true;
    private bool isSearching = false; // Variable para controlar si ya está buscando recursos

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
            if (!isSearching)
            {
                LookForResource();
            }
        }
    }

    public void LookForResource()
    {
        if (!isSearching)
        {
            StartCoroutine(LookForResourceCor());
        }
    }

    IEnumerator LookForResourceCor()
    {
        isSearching = true;
        int attempts = 10; // Limitar el número de intentos
        while (assignedResource == null && attempts > 0)
        {
            assignedResource = map.RequestResource(); // Solicitar un recurso para recoger
            if (assignedResource != null)
            {
                MoveTo(assignedResource.transform.position); // Moverse hacia el recurso.
                isSearching = false;
                yield break;
            }
            attempts--;
            yield return new WaitForSeconds(1f); // Esperar un segundo antes de volver a buscar
        }
        isSearching = false;
    }

    private void EatResource(GameObject resource)
    {
        Debug.Log("Eating resource.");
        Destroy(resource);
        Debug.Log("Resource destroyed");
        readyToEat = false;
        assignedResource = null; // Reiniciar el recurso asignado
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(10); // Tiempo de espera antes de que el escarabajo esté listo para comer de nuevo
        readyToEat = true;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (readyToEat && other.gameObject == assignedResource)
        {
            EatResource(assignedResource);
        }
    }
}