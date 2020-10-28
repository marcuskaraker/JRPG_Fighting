using System;
using UnityEngine;

public class Destructible
{
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }

    public Action onDeath = delegate { };
    public Action onHurt = delegate { };

    public Destructible(float health, float maxHealth)
    {
        if (maxHealth > health)
        {
            health = maxHealth;
        }

        MaxHealth = maxHealth;
        Health = health;
    }

    /// <summary>
    /// Hurt the destructible.
    /// </summary>
    /// <returns>true if died.</returns>
    public bool Hurt(float value)
    {
        Health -= value;
        Health = Mathf.Clamp(Health, 0f, MaxHealth);

        onHurt.Invoke();

        if (Health <= 0f)
        {
            Die();
            return true;
        }

        return false;
    }

    public void Die()
    {
        onDeath.Invoke();
    }
}
