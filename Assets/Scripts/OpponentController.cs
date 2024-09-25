using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Blob))]
public class OpponentController : MonoBehaviour
{
    private Blob _blob;

    private void Start()
    {
        _blob = GetComponent<Blob>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
