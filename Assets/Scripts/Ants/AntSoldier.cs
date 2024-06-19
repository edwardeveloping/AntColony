using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSoldier : Ant
{
    public PredatorManager _predatorManager;
    public Map _map;
    public Transform waittingZone;

    const int PATROL_CHANGE_POSITION_TIME = 1;
    //const int PREDATORS_KILLED_THRESHOLD_TO_DEACTIVATE = 2;
    const float ATTACK_RANGE_RADIOUS = 2.5f;
    const float PATROL_DURATION = 8f; // Duración de la patrulla en segundos

    // Punto destino para moverse
    public Vector3 destino;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Ángulo de corrección para alinear correctamente el sprite
    private float anguloCorreccion;
    private float flipTime;
    private float flipTimeActual;

    GameObject _target;
    bool _active = false;
    int _predatorsKilled = 0;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;

    private Vector2 randomPos;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        anguloCorreccion = -90f;
        barkPanel.gameObject.SetActive(false);
        randomPos = _map.RandomPositionInsideAnthill();
    }
    public override void Initialize()
    {
        // Nos suscribimos al evento
        Predator.OnAntGathererKilled += Activate;
        //_predatorManager.OverPopulatedEvent += Activate; // When prerdatorManagers invokes the event soldiers will activate.

        MoveTo(waittingZone.position);
        destino = waittingZone.position;
    }

    public void Activate()
    {
        if (!_active)
        {
            StartCoroutine(Bark("Activo")); //bark
            _active = true;
            InvokeRepeating("Patrol", 0, PATROL_CHANGE_POSITION_TIME);
            StartCoroutine(PatrolDuration());
        }
    }
    private IEnumerator PatrolDuration()
    {
        yield return new WaitForSeconds(PATROL_DURATION);
        Deactivate();
    }

    public void Deactivate()
    {
        if (_active)
        {
            CancelInvoke("Patrol");
            StartCoroutine(Bark("Durmiendo"));
            randomPos = _map.RandomPositionInsideAnthill();
            _predatorsKilled = 0;
            _active = false;
        }
    }

    private void CheckPosition()
    {
        //guardamos posicion del predator
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        Vector2 currentPos = new Vector2(positionX, positionY);

        if (currentPos == randomPos) //comprobamos que haya llegado a la posicion para actualizarla
        {
            randomPos = _map.RandomPositionInsideAnthill(); //actualizamos el randomPos
        }
    }

    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "Activo":
                barkPanel.sprite = barkList[0];
                break;
            case "Durmiendo":
                barkPanel.sprite = barkList[1];
                break;

        }

        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Predator")
        {
            if ((Vector2.Distance(collision.gameObject.transform.position, gameObject.transform.position) <= ATTACK_RANGE_RADIOUS)
                && _active)
            {
                _predatorManager.KillPredator(collision.gameObject.GetComponent<Predator>());
                _predatorsKilled++;

                Debug.Log("Predator slained. Predators Killed: " + _predatorsKilled);
                //CheckIfSatiated();
            }

            _target = collision.gameObject;
        }
    }

    //private void CheckIfSatiated()
    //{
    //    if(_predatorsKilled >= PREDATORS_KILLED_THRESHOLD_TO_DEACTIVATE)
    //    {
    //        Deactivate();
    //    }
    //}

    private void Patrol()
    {
        Vector3 pos = _map.RandomPositionInsideBounds();
        MoveTo(pos);
        destino = pos;
    }

    private void Update()
    {
        if (_active && _target != null)
        {
            MoveTo(_target.transform.position);
            destino = _target.transform.position;
        }

        if (!_active && _target == null) //Desactivado
        {
            CheckPosition();
            MoveTo(randomPos);
            destino = randomPos;
        }

        SpriteMove();
    }
    public override void ArrivedAtResource(GameObject resource) { }
    public override void ArrivedAtRoom(Room room) { }
    public override void WhenCombatWon() { }

    private void SpriteMove()
    {
        // Mover hacia el destino
        if (destino != null)
        {
            Vector3 direccion = (destino - transform.position).normalized;
            if (direccion != Vector3.zero)
            {
                // Rotar el sprite hacia la dirección de movimiento
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

                // Añadir el ángulo de corrección
                angulo += anguloCorreccion;

                // Ajustar la rotación del sprite para que solo cambie en el plano 2D
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
