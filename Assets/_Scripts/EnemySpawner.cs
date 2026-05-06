using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Room connectedRoom;
    public int EnemyCount;

    [SerializeField] List<GameObject> enemyPrefabList;
    [SerializeField] int availablePoints = 30;
    [SerializeField] int maxEnemyCount =10;
    [SerializeField] List<EnemyStatsModifier> statMods = new List<EnemyStatsModifier>();
    List<Enemy> enemies = new List<Enemy>();


    public void SpawnEnemies()
    {
        availablePoints = connectedRoom.RoomTiles.Count/20  +10;
        maxEnemyCount = connectedRoom.RoomTiles.Count/100 +5;
        while (availablePoints >= 0 && EnemyCount<= maxEnemyCount)
        {
            enemies.Add(SpawnEnemy(enemyPrefabList[Random.Range(0, enemyPrefabList.Count - 1)]));
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].onSpawnEffects.StartEffects(0.1f * i);
        }

    }

    Enemy SpawnEnemy(GameObject enemyObj)
    {
        int n = Random.Range(0, connectedRoom.RoomTiles.Count );
        Vector2 pos = connectedRoom.RoomTiles.AtIndex<Vector2Int>(n);

        Enemy enemy = Instantiate(enemyObj, pos, Quaternion.identity).GetComponent<Enemy>();
        enemy.SetStatsMod(statMods[Random.Range(0, statMods.Count-1)]); //TODO add weighted probability depending on enemy type and stat mod
        enemy.parentSpawner = this;
        enemy.OnDeathE += OnEnemyDeath;
        availablePoints -= enemy.PointCost;
        EnemyCount++;
        return enemy;
    }

    void OnEnemyDeath(Enemy enemy)
    {
        EnemyCount--;
    }
}
