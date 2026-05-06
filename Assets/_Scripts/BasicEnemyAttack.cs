using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;



[RequireComponent(typeof(Enemy))]
public class BasicEnemyAttack : Effectable, EnemyAttackInterface
{
    public float AttackCooldown => attackCooldown / enemyParent.StatMods.AttackSpeedMod; // clamp to above 0.1 later
    public float AttackDamage => attackDamage * enemyParent.StatMods.DamageMod;

    [SerializeField] protected Enemy enemyParent;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected bool canAttack = true;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackLungeDistance;
    [SerializeField] protected float attackLungeTime = 0.1f;
    [SerializeField] protected float timeToDodge = 0.2f;
    [SerializeField ]protected  Collider2D attackCollider;


    // Start is called before the first frame update
    void Start()
    {
        attackCollider = GetComponent<Collider2D>();
        enemyParent = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyParent.InRange && canAttack ) StartCoroutine(Attack(enemyParent.Player.gameObject));
    }



    public virtual IEnumerator Attack(GameObject target)
    {
        if (!canAttack) yield break;
        canAttack = false;

        List<RaycastHit2D> hits = new List<RaycastHit2D>() ;
        ContactFilter2D contactFilter = new ContactFilter2D() ;

        Vector2 dir = (target.transform.position - transform.position).normalized;
        yield return new WaitForSeconds(timeToDodge); //gives the player a chance to dodge
        transform.DOPunchPosition( dir * attackLungeDistance, attackLungeTime) ; //animate lunge
        yield return new WaitForSeconds(attackLungeTime);

        //actual attack 
        attackCollider.Cast(dir, contactFilter, hits, distance:attackLungeDistance);
        foreach (RaycastHit2D hit in hits)
        {
            Health health = hit.transform.GetComponent<Health>();
            if(health != null) { health.TakeDamage(AttackDamage); }
        }

        yield return new WaitForSeconds(AttackCooldown);
        canAttack = true;
    }


}
