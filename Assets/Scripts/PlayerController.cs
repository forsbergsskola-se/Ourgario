using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Blob _blob;

    private void Start()
    {
        _blob = GetComponent<Blob>();
    }

    private void Update()
    {
        var cursorInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _blob.direction = cursorInWorld - transform.position;
    }
}
