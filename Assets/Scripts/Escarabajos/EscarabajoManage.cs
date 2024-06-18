using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscarabajoManage : MonoBehaviour
{
    [SerializeField] GameObject beetlePrefab;
    [SerializeField] public int initialNumBeetles;
    [SerializeField] Map map;
    public Transform beetleSpawn;

    public List<Escarabajos> beetlesList = new List<Escarabajos>();

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

    public void GenerateBeetle()
    {
        Escarabajos beetle = Instantiate(beetlePrefab, beetleSpawn.position, Quaternion.identity).GetComponent<Escarabajos>();
        beetle.map = map;
        beetlesList.Add(beetle);
    }

    public bool KillBeetle(Escarabajos beetleToKill)
    {
        // Remove ant from list.
        if (!beetlesList.Remove(beetleToKill)) // Try to remove ant object from antObject list.
        {
            Debug.Log("Tried to kill a predator, but it was not found in predatorList");
            return false;
        }

        Destroy(beetleToKill.gameObject); // Destroy game object.

        return true;
    }

}
