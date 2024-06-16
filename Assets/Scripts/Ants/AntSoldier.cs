using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSoldier : Ant
{
    public PredatorManager _predatorManager;
    public Map _map;
    public Transform waittingZone;

    const int PATROL_CHANGE_POSITION_TIME = 1;
    const int PREDATORS_KILLED_THRESHOLD_TO_DEACTIVATE = 2;
    const float ATTACK_RANGE_RADIOUS = 2.5f;

    GameObject _target;
    bool _active = false;
    int _predatorsKilled = 0;


    public override void Initialize()
    {
        _predatorManager.OverPopulatedEvent += Activate; // When prerdatorManagers invokes the event soldiers will activate.

        MoveTo(waittingZone.position);
    }

    public void Activate()
    {
        if(!_active)
        {
            InvokeRepeating("Patrol", 0, PATROL_CHANGE_POSITION_TIME);
            _active = true;
        }
    }

    public void Deactivate()
    {
        if(_active) 
        {
            CancelInvoke("Patrol");
            MoveTo(waittingZone.position);
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
                CheckIfSatiated();
            }

            _target = collision.gameObject;
        }
    }

    private void CheckIfSatiated()
    {
        if(_predatorsKilled >= PREDATORS_KILLED_THRESHOLD_TO_DEACTIVATE)
        {
            Deactivate();
        }
    }

    private void Patrol()
    {
        MoveTo(_map.RandomPositionInsideBounds());
    }

    private void Update()
    {
        if(_active && _target != null) 
        {
            MoveTo(_target.transform.position);
        }
    }
    public override void ArrivedAtResource(GameObject resource){}
    public override void ArrivedAtRoom(Room room){}
    public override void WhenCombatWon(){}
}
