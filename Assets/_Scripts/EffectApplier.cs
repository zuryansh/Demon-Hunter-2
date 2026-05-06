using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectApplier : MonoBehaviour
{
    [SerializeField] EffectType effectType;
    [SerializeField] float duration;
    [SerializeField] float factor;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Effectable effectableScript = collision.GetComponent<Effectable>();

        if(effectableScript != null)
        {
            if (effectType == EffectType.DOT)
            {
                Health healthScript = collision.GetComponent<Health>();
                if(healthScript != null) effectableScript.AddNewEffect(new DOTEffect(duration, factor, healthScript));
            }
            else if( effectType == EffectType.Health)
            {
                Health healthScript = collision.GetComponent <Health>();
                if(healthScript != null) effectableScript.AddNewEffect( new HealthEffect(duration, factor, healthScript));
            }
            else if( effectType == EffectType.Speed)
            {
                IMoveable moveable = collision.GetComponent<IMoveable>();
                if (moveable != null) effectableScript.AddNewEffect(new SpeedEffect(duration, factor, collision.gameObject));
            }
            else
            {
                throw new NotImplementedException("Effect application is not implemented");
            }
            
        }
    }

}
