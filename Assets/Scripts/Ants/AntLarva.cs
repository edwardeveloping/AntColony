using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLarva : Ant
{

    //Variables
    public float bornTime = 10f;
    public string type = "Gatherer";
    [SerializeField] public Colony colony;

    private void Update()
    {
        bornTime -= Time.deltaTime;

        if ( bornTime <= 0)
        {
            Born();
        }
    }

    public void Born()
    {
        if (type == "Gatherer")
        {
            colony.Initialize("Gatherer");
        }

        if (type == "Worker")
        {
            colony.Initialize("Worker");
        }
    }
    public override void Initialize()
    {
        
    }
    public override void ArrivedAtResource(GameObject resource){}
    public override void ArrivedAtRoom(Room room){}
    public override void WhenCombatWon(){}
}
