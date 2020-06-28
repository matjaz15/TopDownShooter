using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamagable
{
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void OnDeathTrigger()
    {
        if (OnDeath != null) OnDeath();
    }

    [ContextMenu("SelfDestruct")]
    public virtual void Die() {
        dead = true;
        OnDeathTrigger();
        GameObject.Destroy(gameObject);
    }
}
