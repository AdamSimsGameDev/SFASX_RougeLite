using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConnection : MonoBehaviour
{
    public bool created;
    public GameObject[] objects;

    public void ForceCreate()
    {
        objects[0].SetActive(true);
        objects[1].SetActive(true);
        objects[2].SetActive(true);
    }

    public void StartCreation()
    {
        StartCoroutine(DoStartCreation());
        created = true;
    }

    private IEnumerator DoStartCreation()
    {
        yield return new WaitForSeconds(0.25F);
        objects[0].SetActive(true);
        yield return new WaitForSeconds(0.25F);
        objects[1].SetActive(true);
        yield return new WaitForSeconds(0.25F);
        objects[2].SetActive(true);
    }
}
