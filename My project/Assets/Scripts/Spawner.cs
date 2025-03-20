using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject glidefinPrefab; 
    public GameObject kelpPrefab;
    public GameObject dartfishPrefab; 
    public float spawnInterval = 0.5f; 

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomObject();
        }
    }

    void SpawnRandomObject()
    {
        GameObject prefabToSpawn;
        float randomValue = Random.value;

      if (randomValue < 0.20f)
    {
        prefabToSpawn = kelpPrefab; // 20% 
    }
    else if (randomValue < 0.80f)
    {
        prefabToSpawn = dartfishPrefab; // 60% 
    }
    else
    {
        prefabToSpawn = glidefinPrefab; // 20% 
    }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomSpawnPosition()
    {
        float screenX = Random.Range(0.0f, Screen.width);
        float screenY = Random.Range(0.0f, Screen.height);
        Vector3 screenPosition = new Vector3(screenX, screenY, Camera.main.nearClipPlane + 10.0f);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }
}