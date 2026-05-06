using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum EffectType
{
    Health, Speed, AttackDmg, DOT
}


public interface IEffect
{
    public EffectType EffectType{ get;  set;}
    public float durationSec { get;  set; }
    public float factor{ get;  set;}
    public bool effectApplied { get;  set;}

    public void ApplyEffect();
    public void StopEffect();
}

#region DOT 
[System.Serializable]
public class DOTEffect : IEffect
{
    public EffectType EffectType { get; set; }
    public float durationSec { get; set; }
    public float factor { get; set; }
    public bool effectApplied { get; set; }
    float timeAtApplication;
    Health healthScript;

    public DOTEffect(float _durationSec, float _factor, Health _target)
    {
        EffectType = EffectType.DOT;
        durationSec = _durationSec;
        factor = _factor;
        effectApplied = false;
        healthScript = _target;
        timeAtApplication = Time.time;

    }

    public async void ApplyEffect()
    {
        if (Time.time - timeAtApplication > durationSec) { StopEffect(); return; }
       
        healthScript.TakeDamage(factor);
        await Task.Delay(1000);
        ApplyEffect();
        
    }

    public void StopEffect()
    {
        healthScript.GetComponent<Effectable>().RemoveEffect(this);
    }
}

#endregion

#region Health
public class HealthEffect : IEffect
{
    public EffectType EffectType { get; set; }
    public float durationSec { get; set; }
    public float factor { get; set; }
    public bool effectApplied { get; set; }
    Health healthScript;

    public HealthEffect(float _durationSec, float _factor, Health _target)
    {
        EffectType = EffectType.Health;
        durationSec = _durationSec;
        factor = _factor;
        effectApplied = false;
        healthScript = _target;

    }

    public async void ApplyEffect()
    {
        healthScript.ChangeMaxHealth(healthScript.MaxHealth * factor);
        await Task.Delay(((int)(durationSec*1000)));
        StopEffect();
    }

    public void StopEffect()
    {
        healthScript.ChangeMaxHealth(healthScript.MaxHealth / factor);
        healthScript.GetComponent<Effectable>().RemoveEffect(this);
        Debug.Log("Health Effect was finished");
    }
}

#endregion

#region Speed
public class SpeedEffect : IEffect
{
    public EffectType EffectType { get; set; }
    public float durationSec { get; set; }
    public float factor { get; set; }
    public bool effectApplied { get; set; }
    GameObject target;

    public SpeedEffect(float _durationSec, float _factor, GameObject _target)
    {
        EffectType = EffectType.Health;
        durationSec = _durationSec;
        factor = _factor;
        effectApplied = false;
        target = _target;

    }

    public async void ApplyEffect()
    {
        IMoveable moveableScript = target.GetComponent<IMoveable>();
        moveableScript.SetMoveSpeed(moveableScript.GetMoveSpeed() * factor);
        await Task.Delay(((int)(durationSec * 1000)));
        StopEffect();
    }

    public void StopEffect()
    {
        IMoveable moveableScript = target.GetComponent<IMoveable>();
        moveableScript.SetMoveSpeed(moveableScript.GetMoveSpeed() / factor);
        target.GetComponent<Effectable>().RemoveEffect(this);
        Debug.Log("Speed Effect was finished");
    }
}

#endregion
