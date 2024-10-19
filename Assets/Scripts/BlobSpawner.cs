using UnityEngine;

public class BlobSpawner : MonoBehaviour
{
    public GameObject blobPrefab;  
    public int blobCount = 10;     
    public float spawnRange = 10f; 

    void Start()
    {
        SpawnBlobs();
    }

    void SpawnBlobs()
    {
        for (int i = 0; i < blobCount; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-spawnRange, spawnRange), 
                Random.Range(-spawnRange, spawnRange)
            );
            
            Instantiate(blobPrefab, randomPosition, Quaternion.identity);
        }
    }
}