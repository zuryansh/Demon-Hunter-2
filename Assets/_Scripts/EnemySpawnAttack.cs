using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawnAttack : BasicEnemyAttack
{

    [SerializeField] GameObject minionPrefab;
    [SerializeField] int maxMinionCount;
    [SerializeField] int minionsSpawnedPerAttack;
    [SerializeField] float spawnDistance;
    [Space]

    [SerializeField]List<Enemy> spawned = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyParent.InRange && canAttack) StartCoroutine(Attack(enemyParent.Player.gameObject));
    }



    public override IEnumerator Attack(GameObject target)
    {
        if (!canAttack) yield break;
        canAttack = false;
        Vector2 dir = (target.transform.position - transform.position).normalized;

        Vector3 offset = dir * spawnDistance;

        if (spawned.Count <= maxMinionCount)
        {
            for (int i = 0; i < minionsSpawnedPerAttack; ++i)
            {
                Enemy minion = Instantiate(minionPrefab, transform.position + offset, Quaternion.identity).GetComponent<Enemy>();
                minion.OnDeathE += OnMinionDeath;
                spawned.Add(minion);
            }
        }




        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;


    }

    void OnMinionDeath(Enemy minion)
    {
        spawned.Remove(minion);
    }
}
