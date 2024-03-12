using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public abstract class Ant : MovableObject
{
    public AntManager antManager;
    public Transform refugeZone; // Location to flee when in danger.

    protected float fightingStrength = 50;

    // BASICS.
    public abstract void Initialize();
    public void ChangeRole(AntManager.Role role)
    {
        antManager.GenerateAnt(transform.position.x, transform.position.y, role);
        Die();
    }

    // COMBAT.
    public bool EnterCombat()
    {
        Stop();
        
        if (Random.Range(0, 100) > fightingStrength)
        {
            // if == 0 preedator wins.
            return true;
        }
        // if == 1 ant wins.

        WhenCombatWon();
        return false;
    }
    public void Flee() 
    {
        MoveTo(refugeZone.position);
    }
    public void Die()
    {
        antManager.KillAnt(this);
    }
    public abstract void WhenCombatWon();
    // UTILITIES.
    public abstract void ArrivedAtRoom(Room room);
    public abstract void ArrivedAtResource(GameObject resource);
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Resource>() != null) // Collision with a resource.
        {
            ArrivedAtResource(collision.gameObject);
        }
        else if(collision.GetComponent<Room>() != null) // Collision with a room area.
        {
            Room room = collision.GetComponent<Room>();
            ArrivedAtRoom(room);
        }
    }
}
