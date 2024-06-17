using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscarabajoManage : MonoBehaviour
{
    [SerializeField] GameObject beetlePrefab;
    [SerializeField] public int initialNumBeetles;
    [SerializeField] Map map;

    private List<Escarabajos> beetles = new List<Escarabajos>();

    void Start()
    {
        /*for (int i = 0; i < initialNumBeetles; i++)
        {
            GenerateBeetle();
        }*/

        //SE LLAMA DESDE COLONY
    }

    public void GenerateBeetle()
    {
        Escarabajos beetle = Instantiate(beetlePrefab, map.RandomPositionInsideBounds(), Quaternion.identity).GetComponent<Escarabajos>();
        beetle.map = map;
        beetles.Add(beetle);
    }
}
