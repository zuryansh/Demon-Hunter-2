using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public enum RangeAttackTypes
{
    Straight, SpreadShot, BurstShot, Random
}

public class EnemyRangedAttack : BasicEnemyAttack
{
    public int ProjectileCount => projectileCount * enemyParent.StatMods.ProjectileCountMod;
    public float ProjectileSpeed => projectileSpeed * enemyParent.StatMods.ProjectileSpeedMod;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] int projectileCount;
    [Space]
    [SerializeField] RangeAttackTypes rangeAttackType;
    [SerializeField] float timeBetweenProjectiles;
    [SerializeField] float maxSpreadAngle;
    List<Projectile> projectiles = new List<Projectile>();

    // Start is called before the first frame update
    void Start()
    {
        if(rangeAttackType == RangeAttackTypes.Random)
        {
            int n = Random.Range(0,3);
            switch(n)
            {
                case 0: rangeAttackType = RangeAttackTypes.Straight; break;
                case 1:rangeAttackType = RangeAttackTypes.SpreadShot; break;
                case 2: rangeAttackType= RangeAttackTypes.BurstShot; break;
            }
        }
        Debug.Log(rangeAttackType.ToString());
    }

    public override IEnumerator Attack(GameObject target)
    {
        if (!canAttack) yield break;
        canAttack = false;
        Vector2 dir = (target.transform.position - transform.position).normalized;


        for (int i = 0; i < ProjectileCount; i++)
        {
            if(rangeAttackType == RangeAttackTypes.Straight)
            {
                dir = (target.transform.position - transform.position).normalized;
            }
            else if (rangeAttackType == RangeAttackTypes.SpreadShot)
            {
                dir = (target.transform.position - transform.position).normalized;
                var randomRotationAngle = Random.Range(-maxSpreadAngle/2,maxSpreadAngle/2);
                dir = Quaternion.AngleAxis(randomRotationAngle, Vector3.forward) * dir;
            }
            else if( rangeAttackType == RangeAttackTypes.BurstShot)
            {
                var rotationAngle = maxSpreadAngle / ProjectileCount * i;
                rotationAngle = (i%2==0) ? rotationAngle : -rotationAngle;
                dir = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * dir;
                timeBetweenProjectiles = 0;
            }


            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectiles.Add(projectile);
            projectile.Initialise(gameObject, ProjectileSpeed, dir, attackDamage, 1, LayerMask.NameToLayer("Player"), enemyParent.StatMods.SizeMod);

            yield return new WaitForSeconds(timeBetweenProjectiles);
        }


        

        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;


    }
}
