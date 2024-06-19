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

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.1f;
        flipTimeActual = flipTime;
        anguloCorreccion = -90f;
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
        if(_active) 
        {
            CancelInvoke("Patrol");
            MoveTo(waittingZone.position);
            destino = waittingZone.position;
            _predatorsKilled = 0;
            _active = false;
        }
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Predator")
        {
            if((Vector2.Distance(collision.gameObject.transform.position, gameObject.transform.position) <= ATTACK_RANGE_RADIOUS)
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
        if(_active && _target != null) 
        {
            MoveTo(_target.transform.position);
            destino = _target.transform.position;
        }

        SpriteMove();
    }
    public override void ArrivedAtResource(GameObject resource){}
    public override void ArrivedAtRoom(Room room){}
    public override void WhenCombatWon(){}

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
