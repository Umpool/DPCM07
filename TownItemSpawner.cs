using UnityEngine;

public class TownItemSpawner : MonoBehaviour
{
    [Header("거대한 '마을' 오브젝트를 여기에 넣어주세요")]
    public RectTransform townObject;

    [Header("마을에 떨어질 재화 프리팹(오브젝트)")]
    public GameObject coinPrefab;

    [Header("몇 초마다 하나씩 생성할지 설정")]
    public float spawnInterval = 10f;

    private float timer = 0f;
    private float minX, maxX, minY, maxY;

    void Start()
    {
        if (townObject != null)
        {
            CalculateSpawnBounds();
        }
    }

    void CalculateSpawnBounds()
    {
        Vector3[] corners = new Vector3[4];
        townObject.GetWorldCorners(corners);

        float margin = 50f;

        minX = corners[0].x + margin;
        maxX = corners[2].x - margin;
        minY = corners[0].y + margin;
        maxY = corners[2].y - margin;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomCoin();
            timer = 0f;
        }
    }

    void SpawnRandomCoin()
    {
        if (coinPrefab == null) return;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

        GameObject newCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, townObject);
        newCoin.transform.localScale = Vector3.one;
    }
}
