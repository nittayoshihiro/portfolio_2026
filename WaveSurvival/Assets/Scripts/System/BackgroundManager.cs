using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] layerPrefabs;  // 各レイヤーの背景プレハブ
    [SerializeField] private Vector2 tileSize;

    private Vector2 cameraLastPos;
    private Dictionary<Vector2, List<GameObject>> activeTiles
        = new Dictionary<Vector2, List<GameObject>>();

    void Start()
    {
        SpriteRenderer sr = layerPrefabs[0].GetComponent<SpriteRenderer>();
        tileSize = sr.bounds.size;
        cameraLastPos = mainCamera.transform.position;
        GenerateInitialTiles();
    }

    void Update()
    {
        Vector2 camPos = mainCamera.transform.position;
        if ((camPos - cameraLastPos).sqrMagnitude > 0.1f)
        {
            UpdateTiles(camPos);
            cameraLastPos = camPos;
        }
    }

    void GenerateInitialTiles()
    {
        for (int x = -2; x <= 2; x++)
            for (int y = -2; y <= 2; y++)
                CreateTile(new Vector2(x, y));
    }

    void UpdateTiles(Vector2 camPos)
    {
        Vector2 gridPos = new Vector2(
            Mathf.Floor(camPos.x / tileSize.x),
            Mathf.Floor(camPos.y / tileSize.y)
        );

        for (int x = -2; x <= 2; x++)
            for (int y = -2; y <= 2; y++)
                CreateTile(gridPos + new Vector2(x, y));

        RemoveFarTiles(gridPos);
    }

    void CreateTile(Vector2 gridCoord)
    {
        if (activeTiles.ContainsKey(gridCoord)) return;

        activeTiles[gridCoord] = new List<GameObject>();

        foreach (var prefab in layerPrefabs)
        {
            Vector3 pos = new Vector3(
            gridCoord.x * tileSize.x,
            gridCoord.y * tileSize.y,
            prefab.transform.position.z
            );

            GameObject tile = Instantiate(prefab, pos, Quaternion.identity);
            activeTiles[gridCoord].Add(tile);
        }
    }

    void RemoveFarTiles(Vector2 center)
    {
        List<Vector2> keysToRemove = new List<Vector2>();

        foreach (var kv in activeTiles)
        {
            if (Vector2.Distance(kv.Key, center) > 3)  // 半径 3 タイル外
            {
                foreach (var obj in kv.Value)
                    Destroy(obj);

                keysToRemove.Add(kv.Key);
            }
        }

        foreach (var key in keysToRemove)
            activeTiles.Remove(key);
    }
}
