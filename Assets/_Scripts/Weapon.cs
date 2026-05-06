using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float attackDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health targetHealth = collision.GetComponent<Health>();
        if(targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }
    }

}
