using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DamageTrigger : SerializedMonoBehaviour
{
    [OdinSerialize] public List<IDamageable> Excluded; 
    [Flags]
    public enum DamageFrequency
    {
        None = 0,
        OnEnter = 1 << 0, 
        OnExit = 1 << 1,
        OnStayContinuous = 1 << 2,
        OnStayIntermittent = 1 << 3
    }

    public Action<DamageFrequency, bool> OnDamage;



    public List<Damager> Damagers;
    [Serializable]
    public class Damager
    {
        public DamageFrequency Frequency;
        public IDamageable.DamageType DamageTypes;
        [ShowIf("CanDamage")]
        public float DamageAmount;

        [HideInInspector]
        public bool IsDamageContinous => Frequency.HasFlag(DamageFrequency.OnStayContinuous);

        [Range(.05f, 5f)]
        [ShowIf("@this.Frequency.HasFlag(DamageFrequency.OnStayIntermittent) && this.CanDamage")]
        public float IntermittentDamageWaitTime; 
        [HideInInspector]
        public float lastDamageTime;

        [SerializeReference]
        public IPushCalculator PushCalculator;

         

        public bool TryApplyDamage(IDamageable damageable, DamageFrequency requiredFlag, Transform source, Collider collider)
        {
            if (!WillDamage)
                return false;

            var damageAmount = DamageAmount * (requiredFlag == DamageFrequency.OnStayContinuous ? Time.fixedDeltaTime : 1f);

            if(PushCalculator != null) PushCalculator.Source = source;
            Func<Vector3, Vector3> pushCalculator = null;
            if (PushCalculator != null)
            {
                pushCalculator = PushCalculator.CalculatePush;
            } 
            

            damageable.TryApplyDamager(DamageTypes, damageAmount, pushCalculator: pushCalculator);
            lastDamageTime = Time.time;
            return true; 
        }

        public bool CanDamage => DamageTypes != IDamageable.DamageType.None && Frequency != DamageFrequency.None;
        public bool WillDamage => CanDamage && DamageAmount > 0f;
    }

    public List<IDamageable> EnteredDamageables;
    public List<IDamageable> StayDamageables;

    private List<IDamageable> currentDamageables;

    private void OnTriggerEnter(Collider other)
    {
        foreach (Damager damager in Damagers)
        {
            damager.lastDamageTime = Time.time;//setting this manually so that intermittent damage doesn't trigger immediately
        } 
        TryInflictDamages(DamageFrequency.OnEnter, other, onDamageResult: _ => Debug.Log("ON ENTER"));
    }

    private Func<Damager, bool> intermittentCondition => dmg => Time.time > (dmg.lastDamageTime + dmg.IntermittentDamageWaitTime);
    private void OnTriggerStay(Collider other)
    {
        TryInflictDamages(DamageFrequency.OnStayContinuous, other, onDamageResult: _ => Debug.Log("ON STAY CONTINUOUS"));

        TryInflictDamages(DamageFrequency.OnStayIntermittent, other, 
            onDamageResult: _ => Debug.Log("ON STAY INTERMITTENT"),
            additionalCondition: intermittentCondition);
    }

    private void OnTriggerExit(Collider other)
    {
        TryInflictDamages(DamageFrequency.OnExit, other, onDamageResult: _ => Debug.Log("ON EXIT"));
    }

    private void TryInflictDamage(Damager damager, DamageFrequency requiredFlag, Collider collider, Action<bool> onDamageResult = null, Func<Damager,bool> additionalCondition = null)
    { 
        if ((additionalCondition?.Invoke(damager) ?? true) && damager.Frequency.HasFlag(requiredFlag))
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null && (Excluded == null || !Excluded.Contains(damageable)))
            { 
                var damageSuccess = damager.TryApplyDamage(damageable, requiredFlag, transform, collider);
                onDamageResult?.Invoke(damageSuccess);
                OnDamage?.Invoke(requiredFlag, damageSuccess);
            }
        }
    }

    private void TryInflictDamages(DamageFrequency requiredFlag, Collider collider, Action<bool> onDamageResult = null, Func<Damager,bool> additionalCondition = null)
    {
        foreach(Damager damager in Damagers)
        {
            TryInflictDamage(damager, requiredFlag, collider, onDamageResult, additionalCondition);
        }
    }
}
