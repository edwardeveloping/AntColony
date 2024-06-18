using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscarabajoManage : MonoBehaviour
{
    [SerializeField] GameObject beetlePrefab;
    [SerializeField] public int initialNumBeetles;
    [SerializeField] Map map;
    public Transform beetleSpawn;

    private List<Escarabajos> beetles = new List<Escarabajos>();

    void Start()
    {
        /*for (int i = 0; i < initialNumBeetles; i++)
        {
            GenerateBeetle();
        }*/

        //SE LLAMA DESDE COLONY
    }

    public IEnumerator GenerateBeetleOverTime(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GenerateBeetle();
            yield return new WaitForSeconds(2f);
        }
        
    }

    private void GenerateBeetle()
    {
        Escarabajos beetle = Instantiate(beetlePrefab, beetleSpawn.position, Quaternion.identity).GetComponent<Escarabajos>();
        beetle.map = map;
        beetles.Add(beetle);
    }

}
