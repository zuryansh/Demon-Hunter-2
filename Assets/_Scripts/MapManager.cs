using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// RETURNS USEFUL INFO ABOUT THE MAP BUT CANNOT EDIT IT
public class MapManager : MonoBehaviour
{
    [SerializeField]SimpleRandomWalkMapGenerator mapGenerator;


    // Start is called before the first frame update
    void Start()
    {
        //mapGenerator = GetComponent<SimpleRandomWalkMapGenerator>();
    
    }

    public HashSet<Vector2Int> GetSurroundingWalls(HashSet<Vector2Int> tiles)
    {
        HashSet<Vector2Int> walls = new HashSet<Vector2Int>();

        foreach (Vector2Int tile in tiles)
        {
            try
            {
                if (mapGenerator.Map[tile.x + 1, tile.y] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x + 1, tile.y)); //right
                if (mapGenerator.Map[tile.x - 1, tile.y] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x - 1, tile.y)); //left
                if (mapGenerator.Map[tile.x, tile.y + 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x, tile.y + 1)); //up
                if (mapGenerator.Map[tile.x, tile.y - 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x, tile.y - 1)); //down

                if (mapGenerator.Map[tile.x + 1, tile.y + 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x + 1, tile.y + 1)); //top right
                if (mapGenerator.Map[tile.x + 1, tile.y - 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x + 1, tile.y - 1)); //bottom right
                if (mapGenerator.Map[tile.x - 1, tile.y - 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x - 1, tile.y - 1)); //bottom left
                if (mapGenerator.Map[tile.x - 1, tile.y + 1] == ((int)TileTypes.Wall)) walls.Add(new Vector2Int(tile.x - 1, tile.y + 1)); //top left
            }
            catch (IndexOutOfRangeException) { continue; }
        }

        return walls;

    }

    public Vector2Int FindClosestRoom(Vector2Int startRoomPos)
    {
        int minDist = int.MaxValue;
        Vector2Int closestRoomPos = new Vector2Int();
        foreach (Vector2Int roomPos in mapGenerator.WalkerStartPositions)
        {
            if (roomPos != startRoomPos)
            {
                int dist = (startRoomPos - roomPos).sqrMagnitude;
                if (dist <= minDist)
                {
                    minDist = dist;
                    closestRoomPos = roomPos;
                }
            }
        }
        return closestRoomPos;
    }

    public Vector2Int FindFurthestRoom(Vector2Int startRoomPos)
    {
        int maxDist = int.MinValue;
        Vector2Int furthestRoomPos = new Vector2Int();
        foreach (Vector2Int roomPos in mapGenerator.WalkerStartPositions)
        {
            if (roomPos != startRoomPos)
            {
                int dist = (startRoomPos - roomPos).sqrMagnitude;
                if (dist >= maxDist)
                {
                    maxDist = dist;
                    furthestRoomPos = roomPos;
                }
            }
        }
        return furthestRoomPos;
    }

    public int GetSurroundingTileCount(int gridX, int gridY, int tileVal)
    {
        int count = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < mapGenerator.MapWidth && neighbourY >= 0 && neighbourY < mapGenerator.MapHeight)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        if (mapGenerator.Map[neighbourX, neighbourY] == tileVal) count++;
                    }
                }

            }
        }
        return count;
    }

    public void MergeRoomTilemaps(List<Room> rooms, Tilemap mergeToFloorTilemap, Tilemap mergeToColliderTilemap)
    {
        foreach (Room room in rooms)
        {
            foreach (Vector2Int pos in room.RoomTiles)
            {
                mergeToFloorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), room.TilemapVisualiser.floorTilemap.GetTile(new Vector3Int(pos.x,pos.y,0)));
            }
            

            foreach (Vector2Int pos in room.WallTiles)
            {
                mergeToColliderTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), room.TilemapVisualiser.colliderTilemap.GetTile(new Vector3Int(pos.x, pos.y, 0)));
                mergeToFloorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), room.TilemapVisualiser.wallTilemap.GetTile(new Vector3Int(pos.x, pos.y, 0)));

            }
            Destroy(room.TilemapVisualiser.colliderTilemap.transform.parent.gameObject);
            
        }
    }
}
