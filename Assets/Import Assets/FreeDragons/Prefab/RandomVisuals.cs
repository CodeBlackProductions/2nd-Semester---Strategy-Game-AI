using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVisuals : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> visuals = new List<GameObject>();

    void Start()
    {
        int randomNr = Random.Range(0,visuals.Count);
        GameObject newVis = GameObject.Instantiate<GameObject>(visuals[randomNr]);
        newVis.transform.position = transform.position;
        newVis.transform.forward = transform.forward;
        newVis.transform.SetParent(transform);
    }
}
