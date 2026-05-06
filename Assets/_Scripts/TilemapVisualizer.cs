using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class TilemapVisualiser : MonoBehaviour
{
    [SerializeField]
    public Tilemap floorTilemap;
    [SerializeField]
    TileBase floorTile;
    [SerializeField]
    public Tilemap wallTilemap;
    public Tilemap colliderTilemap;
    [SerializeField] TileBase colliderTile;
    [SerializeField] TileBase wallTile;

    public void PaintFloorTiles(HashSet<Vector2Int> fillTiles, bool clearOld = true)
    {
        //if (clearOld) floorTilemap.ClearAllTiles();
        PaintTiles(floorTilemap,floorTile, fillTiles);
    }

    private void PaintTiles(Tilemap tilemap, TileBase tile, HashSet<Vector2Int> fillTiles)
    {
        foreach (Vector2Int pos in fillTiles)
        {
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
        }
    }

    public void PaintWallTiles(int[,] map, bool clearOld =true)
    {
        //if (clearOld) wallTilemap.ClearAllTiles();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if (map[i, j] == ((int)TileTypes.Wall))
                {
                    wallTilemap.SetTile(new Vector3Int(i, j, 0), wallTile);
                    colliderTilemap.SetTile(new Vector3Int(i, j, 0), colliderTile);

                }

            }
        }
    }
    public void PaintWallTiles(HashSet<Vector2Int> fillTiles, bool clearOld = true)
    {
        //if (clearOld) floorTilemap.ClearAllTiles();
        PaintTiles(colliderTilemap, colliderTile, fillTiles); // colliders
        PaintTiles(floorTilemap, wallTile, fillTiles); //visuals
    }

    //public void PaintBorderTiles(int[,] map, bool clearOld = true)
    //{
    //    PaintTiles(floorTilemap, floorTile, map, ((int)TileTypes.Border));
    //}

    //[ContextMenu("CLEAR TILES")]
    //public void ClearTilemaps()
    //{
    //    floorTilemap.ClearAllTiles();
    //    wallTilemap.ClearAllTiles();
    //}

    


}
