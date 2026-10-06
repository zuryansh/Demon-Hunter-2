using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DamageTypes
{
    Basic, Fire , Lightning , Poison , Magic
}

public class Health : MonoBehaviour
{
    public float MaxHealth => maxHealth;

    [SerializeField] Slider healthSlider;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    //[SerializeField] bool hasDied;
    [SerializeField] List<DamageTypes> resistantDamageTypes = new List<DamageTypes>();
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        UpdateSliderTo(currentHealth);
    }

    public void TakeDamage(float dmg , DamageTypes type = DamageTypes.Basic)
    {
        float effectiveDamage;
        effectiveDamage = (resistantDamageTypes.Contains(type))? dmg/2 :  dmg;
        currentHealth -= effectiveDamage;
        if (currentHealth <=0)
        {
            //hasDied = true;
            UpdateSliderTo(0);
            Die();
        }
        UpdateSliderTo(currentHealth);
    }

    private void UpdateSliderTo(float val)
    {
        if (healthSlider == null) return;
        healthSlider.value = val;
    }

    public void ChangeMaxHealth(float val)
    {
        maxHealth = val;
        healthSlider.maxValue = val;
    }

    void Die()
    {
        OnDeath?.Invoke();
    }

  
}
