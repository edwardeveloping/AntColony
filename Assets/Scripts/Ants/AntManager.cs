using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    //public string larvaType;
    public enum Role
    {
        Worker,
        Builder,
        Soldier,
        Gatherer,
        Larva,
        Queen
    }

    [SerializeField] Map map;
    [SerializeField] PredatorManager predatorManager;

    [SerializeField] Transform soldiersWaittingZone;

    [SerializeField] GameObject antWorkerPrefab;
    [SerializeField] GameObject antGathererPrefab;
    [SerializeField] GameObject antBuilderPrefab;
    [SerializeField] GameObject antSoldierPrefab;
    [SerializeField] GameObject antLarvaPrefab;
    [SerializeField] GameObject antQueenPrefab;

    [SerializeField] public Colony colony;

    public Sprite spriteGathererObj;

    private List<GameObject> antObjectList = new List<GameObject>();
    public List<GameObject> antGathererObjectList = new List<GameObject>();
    public List<GameObject> antWorkerObjectList = new List<GameObject>();
    public List<GameObject> antSoldierObjectList = new List<GameObject>();
    public List<GameObject> antLarvaList = new List<GameObject>();

    public bool weatherFavorable;

    private void Start()
    {
        weatherFavorable = true;
    }
    public GameObject GenerateAnt(float x, float y, Role role)
    {
        GameObject antObj = null;

        switch (role)
        {
            case Role.Worker:
                antObj = Instantiate(antWorkerPrefab, new Vector2(x, y), Quaternion.identity);
                antObj.GetComponent<AntWorker>().map = map;
                antObj.GetComponent<AntWorker>().storageRoom = map.storageRoom.GetComponent<Room>();
                antObj.GetComponent<AntWorker>().raisingRoom = map.raisingRoom.GetComponent<Room>();
                antObj.GetComponent<AntWorker>().breedingRoom = map.breedingRoom.GetComponent<Room>();
                antObj.GetComponent<AntWorker>().queenRoom = map.queenRoom.GetComponent<Room>();

                antWorkerObjectList.Add(antObj);
                break;

            case Role.Builder:
                antObj = Instantiate(antBuilderPrefab, new Vector2(x, y), Quaternion.identity);
                break;

            case Role.Soldier:
                antObj = Instantiate(antSoldierPrefab, new Vector2(x, y), Quaternion.identity);
                antObj.GetComponent<AntSoldier>()._predatorManager = predatorManager;
                antObj.GetComponent<AntSoldier>()._map = map;
                antObj.GetComponent<AntSoldier>().waittingZone = soldiersWaittingZone;
                antSoldierObjectList.Add(antObj);
                break;

            case Role.Gatherer:
                antObj = Instantiate(antGathererPrefab, new Vector2(x, y), Quaternion.identity);
                antObj.GetComponent<AntGatherer>().map = map;
                antObj.GetComponent<AntGatherer>().storageRoom = map.storageRoom;
                antObj.GetComponent<AntGatherer>().spriteObj = spriteGathererObj;
                antGathererObjectList.Add(antObj); // Add ant to antGathererList.
                break;
            case Role.Larva:
                antObj = Instantiate(antLarvaPrefab, new Vector2(x, y), Quaternion.identity);
                antObj.GetComponent<AntLarva>().colony = colony;
                antObj.GetComponent<AntLarva>().raisingRoom = map.raisingRoom.GetComponent<Room>();
                //antObj.GetComponent<AntLarva>().type = larvaType;
                antLarvaList.Add(antObj);
                break;

            case Role.Queen:
                antObj = Instantiate(antQueenPrefab, new Vector2(x, y), Quaternion.identity);
                antObj.GetComponent<AntQueen>().breedingRoom = map.breedingRoom.GetComponent<Room>();
                antObj.GetComponent<AntQueen>().queenRoom = map.queenRoom.GetComponent<Room>();
                break;


            default:
                Debug.Log("Error: Ant type not valid. Parameter given: " + role.ToString());
                break;
        }

        // Configure Ant script:
        antObj.GetComponent<Ant>().antManager = this;
        antObj.GetComponent<Ant>().refugeZone = map.refugeZone;
        antObj.GetComponent<Ant>().Initialize(); // Initialize Ant script.

        antObjectList.Add(antObj); // Add created ant to ant list.

        return antObj;
    }
    public bool KillAnt(Ant antToKill)
    {
        // Remove ant from list.
        if (!antObjectList.Remove(antToKill.gameObject)) // Try to remove ant object from antObject list.
        {
            Debug.Log("Tried to kill an ant, but it was not found in antObjectList");
            return false;
        }

        if (antToKill is AntGatherer) // If the ant to kill is a gatherer.
        {
            if (!antGathererObjectList.Remove(antToKill.gameObject)) // Try to remove antGatherer object from antGathererObject list.
            {
                Debug.Log("Tried to kill an ant, but it was not found in antGathererObjectList");
                return false;
            }
        }

        if (antToKill is AntWorker) // If the ant to kill is a gatherer.
        {
            if (!antWorkerObjectList.Remove(antToKill.gameObject)) // Try to remove antGatherer object from antGathererObject list.
            {
                Debug.Log("Tried to kill an ant, but it was not found in antWorkerObjectList");
                return false;
            }
        }

        if (antToKill is AntLarva) // If the ant to kill is a gatherer.
        {
            if (!antLarvaList.Remove(antToKill.gameObject)) // Try to remove antGatherer object from antGathererObject list.
            {
                Debug.Log("Tried to kill an ant, but it was not found in antLarvaList");
                return false;
            }
        }

        Destroy(antToKill.gameObject); // Destroy game object.

        //Debug.Log("AntGathererList size: " + antGathererObjectList.Count + ", AntList size: " + antObjectList.Count);
        return true;
    }

    private void Update()
    {
        foreach (GameObject gatherer in antGathererObjectList)
        {
            gatherer.GetComponent<AntGatherer>().climaFavorable = weatherFavorable;
        }
    }
}
