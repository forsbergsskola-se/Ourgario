using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blob : MonoBehaviour
{
    public Vector2 direction { get; set; }
    private float _size = 1f;
    public string Guid { get;set; }

    public float Size
    {
        get => _size;
        set
        {
            transform.localScale = new Vector3(value, value, 1f);
        }

    }
    public float speed = 3f;

    private void Update()
    {
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed / Size;
    }


    private void OnTriggerStay(Collider other)
    {
        var otherBlob = other.GetComponent<Blob>();
        if (otherBlob.Size >= Size) return;
        var distance = Vector2.Distance(other.transform.position, transform.position);
        if (distance < Size * 0.5f)
        {
            
        }


    }
}
