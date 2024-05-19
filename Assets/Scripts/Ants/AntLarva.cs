using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLarva : Ant
{

    //Variables
    private float bornTime = 6000f;
    private float hungry = 5500f;
    public string type = "Gatherer";
    [SerializeField] public Colony colony;

    GameObject assignedResource;

    private void FixedUpdate()
    {
        bornTime -= 1;
        hungry -= 1;

        if (bornTime <= 0)
        {
            Born();
        }

        if (hungry <= 0)
        {
            Debug.Log(hungry);
            base.Die();
        }
    }



    public void Born()
    {
        if (type == "Gatherer")
        {
            // colony.Initialize("Gatherer");
            antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Gatherer);
        }

        if (type == "Worker")
        {
            // colony.Initialize("Worker");
            antManager.GenerateAnt(transform.position.x, transform.position.y, AntManager.Role.Worker);
            antManager.antLarvaList.Remove(gameObject);
        }

        Destroy(this.gameObject);
    }

    public void FollowTo(Vector3 pos)
    {
        MoveTo(pos);
    }
    public override void Initialize()
    {

    }

    public override void ArrivedAtResource(GameObject resource)
    {
        if (resource.CompareTag("Food"))
        {
            Debug.Log("hungry");
            hungry += 10;
            Destroy(resource.gameObject);
        }
    }
    public override void ArrivedAtRoom(Room room) { }
    public override void WhenCombatWon() { }
}
