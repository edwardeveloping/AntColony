using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] float mapWidth;
    [SerializeField] float mapHeight;

    [SerializeField] GameObject walkableArea;
    [SerializeField] GameObject exterior; // Where resources spawn and enemies patrol.

    public GameObject storageRoom;
    public GameObject queenRoom; // Where the queen lay eggs.
    public GameObject breedingRoom; // Where shells are stored and filled with eggs.
    public GameObject raisingRoom; // Where baby ants are taken, after being born, then fed, and where they wait until they grow.
    public Transform refugeZone; // Zone to flee when ants are in danger.

    [SerializeField] GameObject resourcePrefab;

    public List<GameObject> unasignedResources = new List<GameObject>(); // List of resources with no ant assigned to gather.

    private float _exteriorXPos, _exteriorYPos, _exteriorWidth, _exteriorHeight;

    public void Initilize()
    {

        for (int i = 0; i<5; i++)
        {
            unasignedResources.Add(GenerateResource());
        }
    }

    private void OnValidate()
    {
        walkableArea.transform.localScale = new Vector3(mapWidth, mapHeight, 1); // Adapt the walkable plane when changing map dimensions.
    }

    // RESOURCE MANAGMENT.

    /// <summary>
    /// Genereates a resource object (given by the resource prefa) within the exterior object's limits.
    /// </summary>
    /// <returns></returns>
    private GameObject GenerateResource()
    {
        _exteriorXPos = exterior.transform.position.x;
        _exteriorYPos = exterior.transform.position.y;
        _exteriorWidth = exterior.transform.localScale.x;
        _exteriorHeight = exterior.transform.localScale.y;

        float x;
        float y;

        x = Random.Range(_exteriorXPos - _exteriorWidth / 2f, _exteriorXPos + _exteriorWidth / 2f);
        y = Random.Range(_exteriorYPos - _exteriorHeight / 2f, _exteriorYPos + _exteriorHeight / 2f);

        return Instantiate(resourcePrefab, new Vector3(x, y, 0), Quaternion.identity);
    }

    public GameObject RequestResource()
    {
        if(unasignedResources.Count != 0)
        {
            GameObject resource = unasignedResources[0];
            unasignedResources.RemoveAt(0);
            return resource;
        }
        return null;
    }

    public void ReturnResource(GameObject resource)
    {
        unasignedResources.Add(resource);
    }
}
