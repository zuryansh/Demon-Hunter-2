using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyStatsModifier : ScriptableObject
{

    public float HealthMod => healthMod;
    public float MoveSpeedMod => moveSpeedMod;
    public float DamageMod=> damageMod;
    public int ProjectileCountMod=> projectileCountMod;
    public float AttackSpeedMod => attackSpeedMod;
    public float SizeMod => sizeMod;
    public float CostMod=> costMod;
    public float ProjectileSpeedMod => projectileSpeedMod;

    [SerializeField] float healthMod =1f;
    [SerializeField] float moveSpeedMod =1f;
    [SerializeField] float damageMod =1f;
    [SerializeField] int projectileCountMod =1;
    [SerializeField] float attackSpeedMod =1f;
    [SerializeField] float sizeMod =1f;
    [SerializeField] float costMod =1f;
    [SerializeField] float projectileSpeedMod = 1f;
    //range mod



}
