using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Helper
{
    public static int[,] CreateEmpty2dArray(int height, int width, int defaultValue)
    {
        int[,] ar = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ar[i, j] = defaultValue;
            }
        }
        return ar;
    }

    public static int[] FindDimensionsOfPositionHashSet(HashSet<Vector2Int> hash)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int pos in hash)
        {
            if (pos.x > maxX) maxX = pos.x;
            else if (pos.x < minX) minX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
            else if (pos.y < minY) minY = pos.y;
        }

        int height = Math.Abs(maxY - minY) + 1;
        int width = Math.Abs(maxX - minX) + 1;
        return new int[2] { height, width };
    }



    public static Vector2Int[] GetPositionOf4Neighbours(Vector2Int startPos)
    {
        Vector2Int[] neighbours = new Vector2Int[4];
        neighbours[0] = startPos + new Vector2Int(0, 1);
        neighbours[1] = startPos + new Vector2Int(1, 0);
        neighbours[2] = startPos + new Vector2Int(0, -1);
        neighbours[3] = startPos + new Vector2Int(-1, 0);
        return neighbours;
    }

    public static void GetFloodFill(int[,] map, Vector2Int startPos, int fillVal, HashSet<Vector2Int> outPositions)
    {
        // start with a point
        // find its neighhbours that have the same value
        // set the neighbour as start point 
        //repeat

        if (map[startPos.x, startPos.y] != fillVal) return; //not a part of room
        if (outPositions.Contains(startPos)) return; //already included in roompositions
        outPositions.Add(startPos);
        GetFloodFill(map, startPos + new Vector2Int(0, 1), fillVal, outPositions);
        GetFloodFill(map, startPos + new Vector2Int(1, 0), fillVal, outPositions);
        GetFloodFill(map, startPos + new Vector2Int(0, -1), fillVal, outPositions);
        GetFloodFill(map, startPos + new Vector2Int(-1, 0), fillVal, outPositions);

    }

    public static Vector3 ToV3(this Vector2Int vector, float buffer = 0f)
    {
        return new Vector3(vector.x+ buffer, vector.y + buffer, 0);
    }

    public static Vector2 ToV2(this Vector3 vector, float buffer = 0f)
    {
        return new Vector2(vector.x + buffer, vector.y + buffer);
    }

    public static Vector2Int ToV2Int(this Vector2 vector, float buffer = 0f)
    {
        return new Vector2Int(((int)(vector.x + buffer)), ((int)(vector.y + buffer)));
    } 
    public static T AtIndex<T>(this HashSet<T> hashSet, int index)
    {
        int i = 0;
        foreach (T val in hashSet)
        {
            if (i == index) return val;
            i++;
        }
        return default(T);
    }
}
