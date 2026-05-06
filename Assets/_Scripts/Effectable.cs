using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public abstract class Effectable: MonoBehaviour
{
    [SerializeField] protected List<IEffect> activeEffects =  new List<IEffect>(); //TODO MAKE CUSTOM EDITOR FOR SERIALIS

    
    public virtual void RemoveEffect(IEffect effect)
    {
        activeEffects.Remove(effect);
        
    }

    public virtual void AddNewEffect(IEffect effect)
    {
        activeEffects.Add(effect);
        effect.ApplyEffect();
    }

}
