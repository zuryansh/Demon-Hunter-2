using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpecialAttack : BasicEnemyAttack
{
    public int ProjectileCount => projectileCount * enemyParent.StatMods.ProjectileCountMod;
    public float ProjectileSpeed => projectileSpeed * enemyParent.StatMods.ProjectileSpeedMod;

    [SerializeField] float jumpPower;
    [SerializeField] bool canDoSpecialAttack;
    [SerializeField] int projectileCount;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject dangerZoneIndicator;
    public override IEnumerator Attack(GameObject target)
    {
        

        if (!canAttack) yield break;
        canAttack = false;

        if (!canDoSpecialAttack) { StartCoroutine(base.Attack(target)); yield break; }

        if(dangerZoneIndicator!=null) Destroy(Instantiate(dangerZoneIndicator, target.transform.position, Quaternion.identity), timeToDodge);

        transform.DOJump(target.transform.position, jumpPower, 1, timeToDodge).SetEase(Ease.InOutBack);
        yield return new WaitForSeconds(timeToDodge);

        List<RaycastHit2D> hits = new List<RaycastHit2D>() ;
        ContactFilter2D contactFilter = new ContactFilter2D() ;

        //actual attack 
        attackCollider.Cast(Vector3.forward, contactFilter, hits, distance:1);
        foreach (RaycastHit2D hit in hits)
        {
            Health health = hit.transform.GetComponent<Health>();
            if(health != null) { health.TakeDamage(attackDamage); }
        }

        for (int i = 0; i < ProjectileCount; i++)
        {
            var rotationAngle = 360 / ProjectileCount * i;
            //rotationAngle = (i % 2 == 0) ? rotationAngle : -rotationAngle;
            Vector2 dir = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * Vector2.right;



            Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.Initialise(gameObject, ProjectileSpeed, dir, attackDamage, 1, LayerMask.NameToLayer("Player"), enemyParent.StatMods.SizeMod);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

    }

}
