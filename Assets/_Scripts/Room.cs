//using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public HashSet<Vector2Int> RoomTiles => roomTiles;
    public HashSet<Vector2Int> WallTiles => wallTiles;
    public TilemapVisualiser TilemapVisualiser => tilemapVisualiser;
    public SimpleRandomWalkMapGenerator MapGenerator => generator;
    public HashSet<Vector2Int> GetRoomTiles() => roomTiles;
    public SimpleRandomWalkMapGenerator generator;
    public Vector2Int connectedRoomPos;
    public MapManager manager;
    public bool IsBossRoom;

    bool quitting;

    [SerializeField] bool spawned = false;
    [SerializeField] bool playerInRoom = false;
    [SerializeField]TilemapVisualiser tilemapVisualiser;
    [SerializeField] bool visualise;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] bool roomCleared;
    [SerializeField] Teleporter teleporterPrefab;

    Teleporter teleporter;
    Vector2Int positions;
    HashSet<Vector2Int> roomTiles;
    HashSet<Vector2Int> wallTiles;
    Player player;


    public void SetRoomTiles(HashSet<Vector2Int> tiles) { roomTiles = tiles; wallTiles = manager.GetSurroundingWalls(roomTiles); }

    //[Button("YAYAYY")]
    //void ASDS() { Debug.Log("ASDS"); }

    private void Awake()
    {
        SimpleRandomWalkMapGenerator.EMapGenerationFinished += OnMapGenFinished;
    }


    private void Start()
    {
        player = UniversalConstants.inst._Player;
        if(generator== null)
        {
            VisualiseRoom();
            OnMapGenFinished();
        }

        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.connectedRoom = this;
    }

    private void Update()
    {
        if(roomTiles.Contains(player.transform.position.ToV2().ToV2Int()))
        {
            playerInRoom = true;
            OnPlayerEntry();
        }

        if(playerInRoom)
        {
            if(enemySpawner.EnemyCount == 0)
            {
                roomCleared = true;
                OnRoomClear();
            }
        }

    }

    [ContextMenu("SPWAN ENEMIES")]
    public void OnPlayerEntry()
    {
        if (!spawned)
        {
            enemySpawner.SpawnEnemies();
            spawned = true;
        }

    }

    void OnRoomClear()
    {
        teleporter.gameObject.SetActive(true);
    }

    public void OnMapGenFinished()
    {
        Vector2Int tpPos = roomTiles.AtIndex<Vector2Int>(Random.Range(0, roomTiles.Count - 1));
        teleporter = Instantiate(teleporterPrefab,tpPos.ToV3(), Quaternion.identity );
        teleporter.teleportToPosition = connectedRoomPos;
        teleporter.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (roomTiles != null)
        {
            foreach (Vector2Int tile in roomTiles)
            {
                Gizmos.DrawCube(tile.ToV3(0.5f), Vector3.one);
            }
            Gizmos.color = Color.blue;
            foreach (Vector2Int tile in wallTiles)
            {
                Gizmos.DrawCube(tile.ToV3(0.5f), Vector3.one);

            }
            Gizmos.DrawSphere(connectedRoomPos.ToV3(), 5);
        }
    }

    public void VisualiseRoom()
    {
        if (!visualise) return;
        tilemapVisualiser.PaintFloorTiles(roomTiles, true);
        tilemapVisualiser.PaintWallTiles(wallTiles, true);
    }

    private void OnDestroy()
    {
        SimpleRandomWalkMapGenerator.EMapGenerationFinished -= OnMapGenFinished;

        if (!quitting)
        {
        generator.RemoveTiles(roomTiles);
        generator.RemoveTiles(wallTiles);

        }
    }

    void OnApplicationQuit()
    {
        quitting = true;
    }

}
