using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float moveSpeed;
    Vector2 dir;
    Rigidbody2D rb;
    float damage;
    GameObject shooter;
    [SerializeField] int pierce;
    LayerMask targetLayer;
    int numObjsHit=0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * moveSpeed;
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Walls")) Destroy(gameObject);

        if (collision.gameObject.layer != targetLayer) return;
        if (collision.gameObject == shooter) return;
        Health health = collision.transform.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            numObjsHit++;
        }

        if(numObjsHit>=pierce) Destroy(gameObject);
    }

    public void Initialise(GameObject shooter, float moveSpeed, Vector2 dir, float damage, int pierce, LayerMask targetLayer, float sizeMod =1f)
    {
        this.shooter = shooter;
        this.moveSpeed = moveSpeed;
        this.dir = dir;
        this.damage = damage;
        this.pierce = pierce;
        this.targetLayer = targetLayer;
        transform.localScale *= sizeMod;
    }
}
