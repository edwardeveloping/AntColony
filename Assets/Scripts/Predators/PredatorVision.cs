using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{
    public bool insideVision;
    public GameObject ant;

    public SpriteRenderer barkPanel;
    public Sprite[] barkList;
    private void Start()
    {
        insideVision = false;
        barkPanel.gameObject.SetActive(false);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ant")
        {
            insideVision = true;
            ant = collision.gameObject;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ant")
            StartCoroutine(Bark("Detecta Hormiga"));
    }
    IEnumerator Bark(string text)
    {
        barkPanel.gameObject.SetActive(true);
        switch (text)
        {
            case "Detecta Hormiga":
                barkPanel.sprite = barkList[0];
                break;

        }

        yield return new WaitForSeconds(2f);

        barkPanel.gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == ant)
        {
            insideVision = false;
        }
    }

    private void Update()
    {
        if (insideVision)
        {
            transform.parent.GetComponent<Predator>().antTarget = ant;
        }

        else
        {
            transform.parent.GetComponent<Predator>().antTarget = null;
        }
        
    }
}
