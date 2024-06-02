using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLarva : Ant
{

    //Variables
    private float bornTime = 2000f;
    private float hungry = 1500f;
    public string type = "Gatherer";

    public Room raisingRoom;
    [SerializeField] public Colony colony;

    GameObject assignedResource;
    bool alimentada = false;

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
    }

    public override void ArrivedAtRoom(Room room)
    {
        if (raisingRoom == room)
        {
            StartCoroutine(PollForFood());
        }
    }
    public override void WhenCombatWon() { }

    private IEnumerator PollForFood()
    {
        while (!alimentada) // Seguir en el bucle hasta que este alimentada
        {
            if (raisingRoom.count > 0)
            {
                raisingRoom.Remove(1);
                hungry += 1500f;
                alimentada = true;
                break;
            }
            // Esperar un corto tiempo antes de volver a verificar
            yield return new WaitForSeconds(1f);
        }
    }
}