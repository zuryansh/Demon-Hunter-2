using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public static class ProceduralGenerationAlgorithims
{


    public static int[,] SimpleRandomWalk(int[,] map , int startX , int startY,int walkLenght,int iterations, int fillVal)
    {
        //fill the start position fully so it stays after smoothing iterations
        for (int neighbourX = startX - 1; neighbourX <= startX + 1; neighbourX++)
        {
            for (int neighbourY = startY - 1; neighbourY <= startY + 1; neighbourY++)
            {
                map[neighbourX,neighbourY] = fillVal;    
            }
        }
                int prevX = startX;
        int prevY=startY;
        int xindex = 0;
        int yindex = 0;

        System.Random rng = new System.Random();

        for (int j = 0; j < iterations; j++)
        {
            xindex = prevX;
            yindex = prevY;
            for (int i = 0; i < walkLenght; i++)
            {
                

                int n = rng.Next(0, 4);
                if (n == 0) xindex++; //right
                else if (n == 1) xindex--; //left
                else if (n == 2) yindex--; //down
                else if (n == 3) yindex++; //up
                if (!(xindex >= map.GetLength(0) - 5 || yindex >= map.GetLength(1) - 5 || xindex <= 0 || yindex <= 0))
                {

                    prevX = xindex;
                    prevY = yindex;
                    map[xindex, yindex] = fillVal;
                }
                
            }
        }

        return map;
    }


}


//public static class Direction2D
//{
//    public static List<Vector2Int> cardinalDirections = new List<Vector2Int>() {
//        new Vector2Int(1, 0), //right
//        new Vector2Int(-1, 0), //left
//        new Vector2Int(0, 1), // up
//        new Vector2Int(0, -1) //down
//    };

//    public static Vector2Int GetRandomDirection()
//    {
//        return cardinalDirections[UnityEngine.Random.Range(0, cardinalDirections.Count)];
//    }
//}