using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackBase : MonoBehaviour
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected bool canAttack = true;
    [SerializeField] protected float attackDamage;
    
    protected Player player;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    virtual public IEnumerator Attack()
    {
        //if (!canAttack)  yield break ;
        //if(weaponAnimator != null) { weaponAnimator.Play(PLAYER_ATTACK_ANIM); spriteRenderer.enabled = true; }
        //canAttack = false;
        //weaponCollider.enabled = true;
        //yield return new WaitForSeconds(attackDuration);
        //weaponCollider.enabled = false;
        //yield return new WaitForSeconds(attackCooldown);
        //canAttack = true;
        //if (weaponAnimator != null) { spriteRenderer.enabled = false; }
        Debug.Log("ATTACK");
        yield break;

    }
}
