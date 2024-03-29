using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class AntGatherer : Ant
{
    public Map map;
    public GameObject storageRoom;

    GameObject assignedResource;

    public override void Initialize()
    {
        LookForResource();
    }
    public void LookForResource()
    {
        assignedResource = map.RequestResource(); // Request a resource to go pick up (it will remove the resource from the list)
        if(assignedResource != null)
        {
            MoveTo(assignedResource.transform.position); // Head that way.
        }
        
    }
    public override void ArrivedAtResource(GameObject resource)
    {
        if (assignedResource == resource) // If the ant collided with its assigned resource pick it up
        {
            Destroy(resource);
            //carryingResource = true;
            MoveTo(storageRoom.transform.position);
        }
        
    }
    public override void ArrivedAtRoom(Room room)
    {
        storageRoom.GetComponent<Room>().Add(1);
        LookForResource();
    }

    public override void WhenCombatWon()
    {
        Flee();
    }
}
