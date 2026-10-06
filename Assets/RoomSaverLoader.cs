using EditorAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSaverLoader : MonoBehaviour
{
    [SerializeField] Vector2Int placePos;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] Tilemap mergeToFloorTilemap;
    [SerializeField] Tilemap mergeToColliderTilemap;

    [SerializeField] MapManager mapManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent<MapManager>(out mapManager)) Debug.LogWarning("Map Manager not found!");
    }

    [Button("Place Prefab")]
    void PlaceRoomm()
    {
        Instantiate(roomPrefab, placePos.ToV3(),Quaternion.identity);
        mapManager.MergeRoomTilemaps(roomPrefab, mergeToFloorTilemap, mergeToColliderTilemap);
    }
}
