using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLarva : Ant
{

    //Variables
    private float bornTime = 1600f;
    private float hungry = 1500f;
    public string type;

    public Room raisingRoom;
    [SerializeField] public Colony colony;

    GameObject assignedResource;
    bool alimentada = false;

    //Sprite
    private float flipTime;
    private float flipTimeActual;

    // Referencia al componente SpriteRenderer
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Obtener el componente SpriteRenderer del GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipTime = 0.5f;
        flipTimeActual = flipTime;
    }
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
            //Debug.Log(hungry);
            base.Die();
        }

        SpriteMove();
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
        }
        base.Die();
        //Destroy(this.gameObject);
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

    private void SpriteMove()
    {
        // Manejar el flip del sprite
        flipTimeActual -= Time.deltaTime;
        if (flipTimeActual <= 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipTimeActual = flipTime;
        }
    }
}