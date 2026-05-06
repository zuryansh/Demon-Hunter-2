using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.U2D.Animation;
using System.Runtime.CompilerServices;

public enum EnemyTypes
{
    BasicMelee, Ranged
}
public class Enemy : Effectable , IMoveable
{
    public bool InRange => inRange;
    public Player Player => player;
    public int PointCost => ((int)(pointCost*statsMod.CostMod));
    public float MoveSpeed => moveSpeed * statsMod.MoveSpeedMod;
    public EnemyStatsModifier StatMods => statsMod;

    public event Action OnHitEvent;
    public EnemySpawner parentSpawner;
    public bool moveTowardsPlayer;
    public event Action<Enemy> OnDeathE;
    public EnemyOnSpawnEffects onSpawnEffects;

    [SerializeField] string enemyName;
    [SerializeField] int pointCost;
    [SerializeField] float moveSpeed;
    [SerializeField] float stoppingDist=5f;
    [SerializeField] float sightRange=15f;
    [SerializeField] bool drawGizmos;
    [SerializeField] bool inRange;
    [SerializeField] Sprite onHitSprite;
    [SerializeField] float onSpwanInactivityTime;
    [SerializeField] EnemyStatsModifier statsMod;

    bool isInactive;
    bool hitImmunity;
    AnimationManager animManager;
    Vector2 vectorToPlayer;
    Sprite OGSprite;
    Health healthScript;
    Player player;

    new SpriteRenderer renderer;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        OGSprite = renderer.sprite;
        animManager = GetComponent<AnimationManager>();
        healthScript = GetComponent<Health>();
        player = UniversalConstants.inst._Player;
        if (healthScript != null) { healthScript.OnDeath += OnDeathEffects; }
        transform.localScale *= statsMod.SizeMod;

        isInactive = true;
        hitImmunity = true;
        Invoke("OnSpawnActive", onSpwanInactivityTime);
        if (parentSpawner == null) onSpawnEffects.StartEffects(0f);

        
    }

    void OnSpawnActive() { isInactive = false; hitImmunity = false; }

    private void FixedUpdate()
    {
        if(isInactive ) { return; }

        vectorToPlayer = player.transform.position - transform.position;
        if( moveTowardsPlayer ) 
        {
            MovetoPoint(player.transform.position);
        }
        if (vectorToPlayer.sqrMagnitude<= stoppingDist * stoppingDist)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        //if (animManager != null)
        //{
        //    if (rb.velocity == Vector2.zero)
        //    {
        //        animManager.PlayAnim(enemyName + "Idle");
        //    }
        //    else { animManager.PlayAnim(enemyName + "Walk"); }
        //}

        animManager.PlayAnim(enemyName + "WalkAnim");
    }

    public void MovetoPoint(Vector2 target)
    {
        Vector2 dir = target - transform.position.ToV2();
        if (dir.magnitude > sightRange) return;
        if (dir.sqrMagnitude < stoppingDist * stoppingDist) dir = Vector2.zero;


        Vector2 targetSpeed = dir.normalized * MoveSpeed;
        Vector2 speedDif = targetSpeed - rb.linearVelocity;
        rb.AddForce(speedDif, ForceMode2D.Impulse); // impulse feels more snappy but FORCE feels more floaty
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(OnHit());
    }

    IEnumerator OnHit()
    {

        if (hitImmunity) yield break;
        hitImmunity = true;
        OnHitEffectsStart();
        OnHitEvent?.Invoke();
        yield return new WaitForSeconds(0.1f);
        OnHitEffectsEnd();
        hitImmunity = false;
    }

    void OnHitEffectsStart()
    {
        if (renderer == null) return;
        animManager.Animator.enabled = false;
        renderer.sprite = onHitSprite;
    }

    void OnHitEffectsEnd()
    {
        if (renderer == null) return;
        animManager.Animator.enabled = true;
        renderer.sprite = OGSprite;
    }
    
    public  void OnDeathEffects()
    {
        //if (parentSpawner!= null) parentSpawner.EnemyCount--;
        if (OnDeathE != null) OnDeathE(this);
        Destroy(gameObject);
    }


    public void SetStatsMod(EnemyStatsModifier stats)
    {
        statsMod = stats;
    }

    private void OnDrawGizmos()
    {
        if(!drawGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDist);
    }

    public void SetMoveSpeed(float val)
    {
        moveSpeed = val/statsMod.MoveSpeedMod;
    }
    public float GetMoveSpeed() => MoveSpeed;
}
