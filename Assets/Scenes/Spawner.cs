using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float xPadding = 0.8f;
    public float spawnTopY = 6f;

    private Camera cam;
    private float minX, maxX;
    private Coroutine loop;

    void Awake()
    {
        cam = Camera.main;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        minX = left.x;
        maxX = right.x;
    }

    public void StartSpawning()
    {
        StopSpawning();
        loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    IEnumerator SpawnLoop()
    {
        while (GameManager.Instance.IsPlaying)
        {
            SpawnOne();
            float wait = GameManager.Instance.CurrentSpawnRate;
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnOne()
    {
        float x = Random.Range(minX + xPadding, maxX - xPadding);
        Vector3 pos = new Vector3(x, spawnTopY, 0f);
        var go = Instantiate(obstaclePrefab, pos, Quaternion.identity);

        var ob = go.GetComponent<Obstacle>();
        ob.fallSpeed = GameManager.Instance.CurrentObstacleSpeed;
    }
}


