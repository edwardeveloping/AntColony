using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] float mapWidth;
    [SerializeField] float mapHeight;

    [SerializeField] int initialNumResources;
    [SerializeField] float generationRate; // Rate at wich resources are generated.
    [SerializeField] int maxNumResources = 10;

    [SerializeField] GameObject walkableArea;
    [SerializeField] GameObject exterior; // Where resources spawn and enemies patrol.

    public GameObject storageRoom;
    public GameObject queenRoom; // Where the queen lay eggs.
    public GameObject breedingRoom; // Where shells are stored and filled with eggs.
    public GameObject raisingRoom; // Where baby ants are taken, after being born, then fed, and where they wait until they grow.
    public Transform refugeZone; // Zone to flee when ants are in danger.

    [SerializeField] GameObject resourcePrefab;

    public List<GameObject> unasignedResources = new List<GameObject>(); // List of resources with no ant assigned to gather.
    SemaphoreSlim meUnasignedResources = new SemaphoreSlim(1);

    private float _exteriorXPos, _exteriorYPos, _exteriorWidth, _exteriorHeight;

    public void Initilize()
    {
        for (int i = 0; i< initialNumResources; i++)
        {
            GenerateResource();
        }

        if(generationRate < 0)
        {
            InvokeRepeating("GenerateResource", generationRate, generationRate);
        }
    }

    private void OnValidate()
    {
        walkableArea.transform.localScale = new Vector3(mapWidth, mapHeight, 1); // Adapt the walkable plane when changing map dimensions.
    }

    public Vector2 RandomPositionInsideBounds()
    {
        _exteriorXPos = exterior.transform.position.x;
        _exteriorYPos = exterior.transform.position.y;
        _exteriorWidth = exterior.transform.localScale.x;
        _exteriorHeight = exterior.transform.localScale.y;

        float x;
        float y;

        // Generate random coordinate inside the exterior area.
        x = Random.Range(_exteriorXPos - _exteriorWidth / 2f, _exteriorXPos + _exteriorWidth / 2f);
        y = Random.Range(_exteriorYPos - _exteriorHeight / 2f, _exteriorYPos + _exteriorHeight / 2f);

        return new Vector2(x, y);
    }

    // RESOURCE MANAGMENT.

    /// <summary>
    /// Genereates a resource object (given by the resource prefab) within the exterior object's limits.
    /// </summary>
    /// <returns></returns>
    private GameObject GenerateResource()
    {
        if(unasignedResources.Count < maxNumResources)
        {
            GameObject resource = Instantiate(resourcePrefab, RandomPositionInsideBounds(), Quaternion.identity);
            unasignedResources.Add(resource);
            return resource;
        }
        return null;
    }
    public bool AvialableUnassignedResource()
    {
        return unasignedResources.Count > 0;
    }
    public GameObject RequestResource()
    {
        if(unasignedResources.Count > 0)
        {
            GameObject resource = unasignedResources[0];
            unasignedResources.RemoveAt(0);
            Debug.Log($"Resource requested. UnasignedResources length: {unasignedResources.Count}");
            return resource;
        }
        return null;
    }

    public void ReturnResource(GameObject resource)
    {
        unasignedResources.Add(resource);
    }
}
