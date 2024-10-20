using UnityEngine;

public class Blob : MonoBehaviour
{
    public Vector2 Direction { get; set; }
    [SerializeField] private float _size = 1f;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Blob trigger stayed");
        var otherBlob = other.GetComponent<Blob>();

        if (otherBlob.Size < Size)
        {
            Size += otherBlob.Size;
            Destroy(otherBlob.gameObject);
        }
        else
        {
            Debug.Log("PLAYER DESTROYED!");
            //Destroy(gameObject);
        }
    }

}