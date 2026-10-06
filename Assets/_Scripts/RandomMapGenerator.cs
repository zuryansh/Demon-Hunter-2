using EditorAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileTypes {  Air = 0,Floor=1, Wall=2, Path =3, Border = 4}

public class RandomMapGenerator : MonoBehaviour
{
    public static RandomMapGenerator inst;
    public HashSet<Vector2Int> WalkerStartPositions => walkerStartPositions;
    public int[,] Map => map;
    public int MapWidth => mapWidth;
    public int MapHeight => mapHeight;
    public static event Action EMapGenerationFinished;

    [FoldoutGroup("Generation Parameters", nameof(iterations), nameof(walkerCount), nameof(walklength), nameof(mapWidth), nameof(mapHeight), nameof(smoothing), nameof(smoothingIterations), nameof(minRoomSize), nameof(mergeRoomsAfterGen))]
    [SerializeField, HideProperty] int iterations =10;
    [SerializeField, HideProperty] int walklength =10;
    [SerializeField, HideProperty] int mapWidth;
    [SerializeField, HideProperty] int mapHeight;
    [SerializeField, HideProperty] bool smoothing;
    [SerializeField, HideProperty] int smoothingIterations;
    [SerializeField, HideProperty] private int walkerCount;
    [SerializeField, HideProperty] int minRoomSize;
    [SerializeField] bool mergeRoomsAfterGen;

    [SerializeField]Texture2D mapPreviewTexture;
    [SerializeField] TileTypes debugTileLayer;
    [SerializeField] bool ShowDebug = true;
    [SerializeField] TilemapVisualiser tilemapVisualiser;
    [SerializeField] bool connectRooms;
    [SerializeField] GameObject roomPrejab;
    [SerializeField] MapManager mapManager;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap colliderTilemap;
    [SerializeField] List<Room> rooms = new List<Room>();

    int[,] map;
    HashSet<Vector2Int> walkerStartPositions =  new HashSet<Vector2Int>();
    HashSet<Vector2Int> roomPositions =  new HashSet<Vector2Int>();
    GameObject roomParent;
    HashSet<Vector2Int> duplicateWalkerPositions = new HashSet<Vector2Int>();

    private void Awake()
    {
        inst = this;
        
    }

    void Start()
    {
        Camera.main.transform.position = new Vector3(mapWidth / 2, mapHeight / 2, -10);
        Camera.main.orthographicSize = mapWidth / 2;
        mapManager = GetComponent<MapManager>();
        RunProceduralGeneration();

    }
    public void Restart()
    {
        ClearAllData();
        RunProceduralGeneration();
    }

    [ContextMenu("Clear")]
    public void ClearAllData()
    {
        walkerStartPositions.Clear();
        roomPositions.Clear();
        foreach (Room room in rooms)
        {
            Destroy(room.gameObject);
        }
        rooms.Clear();
    }

    void GeneratePreviewTexture()
    {
        mapPreviewTexture = new Texture2D(mapWidth, mapHeight);

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (map[i, j] == ((int)TileTypes.Floor))
                {
                    mapPreviewTexture.SetPixel(i, j, Color.white);
                }
                else if (map[i, j] == ((int)TileTypes.Wall))
                {
                    mapPreviewTexture.SetPixel(i, j, Color.black);
                }
            }
        }
        mapPreviewTexture.Apply();

    }

    public void RunProceduralGeneration()
    {
        try
        {
            map = Helper.CreateEmpty2dArray(mapHeight, mapWidth, 0);

            GenerateFloorTiles();


            GenerateWalls();
            Debug.Log("tile generation done");


            roomPositions = AssignRooms();
            Debug.Log("room assignment done");
            SetBossRoom();
            ConnectRooms();
            Debug.Log("room connections done");


            //make Borders
            Connect2PointsOnMap(new Vector2Int(1, 1), new Vector2Int(1, mapHeight - 2), ((int)TileTypes.Border));
            Connect2PointsOnMap(new Vector2Int(1, 1), new Vector2Int(mapWidth - 2, 1), ((int)TileTypes.Border));
            Connect2PointsOnMap(new Vector2Int(mapWidth - 2, 1), new Vector2Int(mapWidth - 2, mapHeight - 2), ((int)TileTypes.Border));
            Connect2PointsOnMap(new Vector2Int(1, mapHeight - 2), new Vector2Int(mapWidth - 2, mapHeight - 2), ((int)TileTypes.Border));
            GeneratePreviewTexture();


            VisualizeMap();
            Debug.Log("room visualisation done");

            EMapGenerationFinished?.Invoke();
        }
        catch(StackOverflowException ){ Restart(); return; }
        

    }

    private void VisualizeMap()
    {

        foreach (Room room in rooms)
        {
            //write tiles to the rooms tilemaps so later we can merge them all
            room.VisualiseRoom();
        }
        if(mergeRoomsAfterGen)
        mapManager.MergeRoomTilemaps(rooms, floorTilemap, colliderTilemap);
    }

    private void GenerateFloorTiles()
    {
        for (int i = 0; i < walkerCount; i++)
        {
            int walkerStartX = (int)UnityEngine.Random.Range(5, map.GetLength(0) / 1.15f);
            int walkerStartY = (int)UnityEngine.Random.Range(5, map.GetLength(1) / 1.15f);
            walkerStartPositions.Add(new Vector2Int(walkerStartX, walkerStartY));
            map = ProceduralGenerationAlgorithims.SimpleRandomWalk(map, walkerStartX, walkerStartY, walklength, iterations, ((int)TileTypes.Floor));
        }

        if (smoothing)
        {
            for (int i = 0; i < smoothingIterations; i++)
            {
                Smooth(((int)TileTypes.Floor), ((int)TileTypes.Air));
            }
        }
    }

    private HashSet<Vector2Int> AssignRooms() // I KNOW THIS IS HORRIBLE BUT IT WORKS
    {
        //if(roomParent == null ) roomParent = Instantiate(new GameObject("ROOM PARENT"));

        //finding walker positions that are in the same room
        foreach (Vector2Int pos in walkerStartPositions)
        {
            if (duplicateWalkerPositions.Contains(pos)) continue;
            //loop through each walker position and check if its the only one in the room add it to the duplicate positions
            HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
            Helper.GetFloodFill(map, pos, ((int)TileTypes.Floor), roomTiles);
            
            foreach(Vector2Int walkerPos in walkerStartPositions)
            {
                if(walkerPos != pos && roomTiles.Contains(walkerPos) && !duplicateWalkerPositions.Contains(walkerPos)) duplicateWalkerPositions.Add(walkerPos);
            }

        }

        // spawning room objects and assigning their roomTiles
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in walkerStartPositions)
        {
            if(duplicateWalkerPositions.Contains(pos) ) continue;
            roomPositions.Add(pos);
            HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
            Helper.GetFloodFill(map,pos,((int)TileTypes.Floor),roomTiles);

            Debug.Log("here");
            Room room = Instantiate(roomPrejab, pos.ToV3(), Quaternion.identity).GetComponent<Room>();
            room.generator = this;
            room.manager = mapManager;
            room.SetRoomTiles(roomTiles);
            rooms.Add(room);
            room.transform.position = room.RoomTiles.AtIndex(0).ToV3();

        }

        return roomPositions;
            
    }

    public void RemoveTiles(HashSet<Vector2Int> tiles, bool removeWallTilesAroundSelection = true)
    {
        Debug.LogWarning("Tilemap Clearing is Disabled For now");

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (tiles.Contains(new Vector2Int(i,j))) map[i, j] = ((int)TileTypes.Air);
            }
        }

        if(removeWallTilesAroundSelection)
        {
            foreach (Vector2Int wall in mapManager.GetSurroundingWalls(tiles))
            {
                map[wall.x, wall.y] = ((int)TileTypes.Air);
            }
        }

        walkerStartPositions.ExceptWith(tiles);
        roomPositions.ExceptWith(tiles);

        VisualizeMap();
    }

    /// <summary>
    /// Connects all the closest walker start positions together. may leave lone rooms sometimes
    /// </summary>
    void ConnectRooms()
    {

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].IsBossRoom) continue;    

            Vector3 pos = rooms[i + 1].transform.position;
            rooms[i].connectedRoomPos = pos.ToV2().ToV2Int();
            
        }

        

    }

    int[,] Smooth(int fillValue, int emptyVal)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int tileCount = mapManager.GetSurroundingTileCount(i, j, fillValue);

                if (tileCount > 3) map[i, j] = fillValue;
                else if (tileCount <3) map[i, j] = emptyVal;
            }
        }
        


        return map;
    }

    void GenerateWalls()
    {
        //loop through each floor pos 
        // check each cardinal direction and if there is no floor add wall instead
        for (int i = 1; i < map.GetLength(0)-1; i++)
        {
            for (int j = 1; j < map.GetLength(1)-1; j++)
            {
                if (map[i,j] == 1)
                {
                    //loop through each direction
                    if (map[i+1,j] == 0) map[i+1,j] = ((int)TileTypes.Wall); //right
                    if (map[i-1,j] == 0) map[i-1,j] = ((int)TileTypes.Wall); //left
                    if (map[i,j+1] == 0) map[i,j + 1] = ((int)TileTypes.Wall); //up
                    if (map[i,j-1] == 0) map[i,j - 1] = ((int)TileTypes.Wall); //down

                    if (map[i + 1, j+1] == 0) map[i + 1, j + 1] = ((int)TileTypes.Wall); //top right
                    if (map[i + 1, j-1] == 0) map[i + 1, j - 1] = ((int)TileTypes.Wall); //bottom right
                    if (map[i-1, j -1] == 0) map[i - 1, j - 1] = ((int)TileTypes.Wall); //bottom left
                    if (map[i-1, j + 1] == 0) map[i - 1, j + 1] = ((int)TileTypes.Wall); //top left
                }
            }
        }
    } 

    void Connect2PointsOnMap(Vector2Int p1, Vector2Int p2, int fillVal) //NO TOUCHIE
    {
        int startX=0;
        int startY=0;
        int endX=0;
        int endY=0;
        if (p1.x >= p2.x) { startX = p2.x; endX = p1.x; startY = p2.y;endY = p1.y; }
        if(p1.x< p2.x) { startX = p1.x; startY = p1.y; endX = p2.x; endY = p2.y; }
        //x

            for (int j = startX; j <= endX; j++)
            {
                map[j, startY] = fillVal;
            }
        

        //y
        if (startY < endY)
        {
            for (int j = startY; j <= endY; j++)
            {

                map[endX, j] = fillVal;
            }
        }
        else
        {
            for (int j = endY; j <= startY; j++)
            {

                map[endX, j] = fillVal;
            }
        }
    }



    public Room GetBiggestRoom()
    {
        int max = 0;
        Room biggestRoom = null;
        foreach (Room room in rooms)
        {
            if(room.RoomTiles.Count > max)
            {
                max = room.RoomTiles.Count;
                biggestRoom = room;
            }
        }
        return biggestRoom;
    }

    public void SetBossRoom()
    {
        Room bossRoom = GetBiggestRoom();
        bossRoom.IsBossRoom = true;
        rooms.Remove(bossRoom);  //makes boss room the last one in the order
        rooms.Add(bossRoom);

        

    }

    [Button("Save Selected")]
    static void Save()
    {
        GameObject obj = Selection.activeGameObject;

        if (obj == null) return;

        PrefabUtility.SaveAsPrefabAsset(
            obj,
            "Assets/RoomPrefabs/savedRoom.prefab"
        );
    }

    private void OnDrawGizmos()
    {
        if (ShowDebug && map!= null)
        {
            if (mapHeight != 0 && mapWidth != 0)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (map[i, j] == ((int)debugTileLayer)) Gizmos.DrawCube(new Vector3(i+0.5f, j+0.5f, 0), Vector3.one);
                    }
                }
            }

        }

        Gizmos.color = Color.yellow;
        foreach (Vector2Int pos in walkerStartPositions)
        {
            Gizmos.DrawSphere(pos.ToV3(0.5f), 1);
        }
        Gizmos.color = Color.red;

        foreach (Vector2Int pos in duplicateWalkerPositions)
        {
            Gizmos.DrawSphere(pos.ToV3(0.5f), 1);
        }


    }




}
