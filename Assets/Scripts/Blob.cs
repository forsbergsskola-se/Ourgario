using System.Collections;
using UnityEngine;

public class Blob : MonoBehaviour
{
    public Vector2 Direction { get; set; }
    private float _size = 1f;
    public string Guid { get; set; }

    public float Size
    {
        get => _size;
        set
        {
            transform.localScale = new Vector3(value, value, 1f);
            _size = value;
        }
    }

    public float baseSpeed = 3f;

    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Direction.normalized * baseSpeed / Size;
    }

    void OnTriggerStay(Collider other)
    {
        var otherBlob = other.GetComponent<Blob>();
        if (otherBlob == null || otherBlob.Size >= Size) return;

        var distance = Vector2.Distance(other.transform.position, transform.position);
        if (distance < Size * 0.5f)
        {
            Size += otherBlob.Size;
            Destroy(otherBlob.gameObject);
        }
    }

}