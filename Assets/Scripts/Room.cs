using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public string roomName;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI nameDisplay;
    public int count = 0;

    private void Start()
    {
        nameDisplay.text = roomName;
        UpdateDisplay();
    }
    public void Add(int num) // Add a resource (resource, egg, shell...) to the room storage.
    {
        count = count + num;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        textDisplay.text = count.ToString();
    }
}
