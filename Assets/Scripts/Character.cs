using UnityEngine;

public enum DamageType
{
    Physical, Magic, Fire
}

public class Character : MonoBehaviour
{
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    public float spellPower = 1f;
    public float meleeDamage = 10f;
    public Transform castPoint;

    [HideInInspector] public int direction = 1;
    [HideInInspector] public bool invulnerable = false;

    public virtual void Start()
    {
        if (maxHealth == 0)
        {
            invulnerable = true;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
