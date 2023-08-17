using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Character caster;
    [HideInInspector] public float speed;
    [HideInInspector] public float direction;
    [HideInInspector] public float gravity;
    [HideInInspector] public float lifespan;
    [HideInInspector] public float damage;
    [HideInInspector] public float knockback;
    [HideInInspector] public DamageType damageType;
    [HideInInspector] public ParticleSystem collisionParticle;
    [HideInInspector] public int pierce;
    [HideInInspector] public Spell spawnSpell;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool followTarget;
    [HideInInspector] public float turnSpeed;
    [HideInInspector] public string targetTag;
    public bool collisionActive = false;
    // public LayerMask layerMask;
    private Rigidbody2D rb;

    private const float upwardsKnock = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot()
    {
        //transform.rotation = Quaternion.Euler(0, 0, direction);
        rb.velocity = transform.right * speed;
        Invoke(nameof(ActivateCollision), 0.05f);
        Invoke(nameof(DestroyProjectile), lifespan);
    }

    private void Update()
    {
        rb.velocity += gravity * Time.deltaTime * Vector2.down;

        if (followTarget)
        {
            transform.right = Vector3.Lerp(transform.right, target.position - transform.position, turnSpeed * Time.deltaTime);
            rb.velocity = transform.right * speed;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && collisionActive)
        {
            GameManager.instance.SpawnParticles(collisionParticle, transform.position);
            DestroyProjectile();
        }

        /* if (collision.gameObject.layer == layerMask && collisionActive)
        {
            GameManager.instance.SpawnParticles(collisionParticle, transform.position);
            DestroyProjectile();
        } */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();

        if (collision.gameObject.CompareTag(targetTag) && !character.invulnerable)
        {
            character.TakeDamage(damage);
            character.GetComponent<EnemyController>().Knockback(knockback * (rb.velocity.normalized + (Vector2.up * upwardsKnock)));
            GameManager.instance.SpawnParticles(collisionParticle, transform.position);
            if (pierce == 0)
            {
                DestroyProjectile();
            }
            else
            {
                pierce -= 1;
            }
        }
    }

    private void ActivateCollision()
    {
        collisionActive = true;
    }

    public void DestroyProjectile()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject, 5);
        }

        transform.DetachChildren();

        if (spawnSpell != null)
        {
            spawnSpell.ActivateSpell(caster, transform.position, null, targetTag);
        }

        Destroy(gameObject);
    }
}
