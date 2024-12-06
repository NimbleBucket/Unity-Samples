using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float MaxHealth { get; }
    public float CurrentHealth { get; set; } 
    public Action<(float oldHealth, float newHealth, float normalizedOldHealth, float normalizedNewHealth)> OnHealthChange { get; }
    public bool CanHealthExceedMax { get; }
    public bool CanHealthGoBelowZero { get; }

    public DamageType Healers { get; }
    public DamageType Immunities { get; }
    public DamageType Resistances { get; }
    public DamageType Weaknesses { get; } 

    public Vector3 PushPosition { get; }

    public void OnHealthGone(); 

    public void Push(Vector3 pushForce);

    public void TryApplyDamager(DamageType damageTypes, float damageAmount, Func<Vector3, Vector3> pushCalculator = null)
    { 
        if((damageTypes & Healers) != 0)
        {
            Heal(damageAmount);
        }
        else if((damageTypes & Immunities) != 0)
        {

        }
        else if((damageTypes & Resistances) != 0)
        {
            Damage(damageAmount / 2f);
            TryPush(pushCalculator, .66f);
        }
        else if((damageTypes & Weaknesses) != 0)
        {
            Damage(damageAmount * 2f);
            TryPush(pushCalculator, 1.2f);
        }
        else
        {
            Damage(damageAmount);
            TryPush(pushCalculator);
        }
    }

    private void TryPush(Func<Vector3, Vector3> pushCalculator, float forceModifier = 1f)
    {
        if (pushCalculator == null)
            return;
        var pushForce = pushCalculator.Invoke(PushPosition);
        Push(pushForce * forceModifier);
    }

    public void Heal(float damageToHeal)
    {
        var previousHealth = CurrentHealth;
        CurrentHealth += damageToHeal;
        if (!CanHealthExceedMax)
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth);

        OnHealthChange?.Invoke((CurrentHealth, previousHealth, CurrentHealth / MaxHealth, previousHealth / MaxHealth));
    }

    public void Damage(float damageTaken)
    {
        var previousHealth = CurrentHealth;
        CurrentHealth -= damageTaken;
        if (!CanHealthGoBelowZero)
            CurrentHealth = Mathf.Max(0, CurrentHealth);

        OnHealthChange?.Invoke((CurrentHealth, previousHealth, CurrentHealth / MaxHealth, previousHealth / MaxHealth));
        if (CurrentHealth <= 0)
            OnHealthGone();
    }

    [Flags]
    public enum DamageType
    {
        None = 0,
        Smack = 1 << 0,
        Squash = 1 << 1,
        Zap = 1 << 2,
        Wet = 1 << 3,
        Burn = 1 << 4,
    } 
}
