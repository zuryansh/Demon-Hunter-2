using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttack : PlayerAttackBase
{
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

    }

    public override IEnumerator Attack()
    {
        if (!canAttack) yield break;
        canAttack = false;
        Vector3 target = UniversalConstants.inst.MainCam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 dir = (target - transform.position).normalized;


        for (int i = 0; i < projectileCount; i++)
        {
            if (rangeAttackType == RangeAttackTypes.Straight)
            {
                dir = (target - transform.position).normalized;
            }
            else if (rangeAttackType == RangeAttackTypes.SpreadShot)
            {
                dir = (target - transform.position).normalized;
                var randomRotationAngle = Random.Range(-maxSpreadAngle / 2, maxSpreadAngle / 2);
                dir = Quaternion.AngleAxis(randomRotationAngle, Vector3.forward) * dir;
            }
            else if (rangeAttackType == RangeAttackTypes.BurstShot)
            {
                var rotationAngle = maxSpreadAngle / projectileCount * i;
                rotationAngle = (i % 2 == 0) ? rotationAngle : -rotationAngle;
                dir = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * dir;
                timeBetweenProjectiles = 0;
            }


            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectiles.Add(projectile);
            projectile.Initialise(gameObject, projectileSpeed, dir.normalized, attackDamage, 1, LayerMask.NameToLayer("Enemies"));

            yield return new WaitForSeconds(timeBetweenProjectiles);
        }




        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;


    }
}
