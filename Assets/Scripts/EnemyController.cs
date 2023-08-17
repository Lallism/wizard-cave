using UnityEngine;

[RequireComponent (typeof(Enemy))]
[RequireComponent (typeof(Controller2D))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5;
    public float weight = 1;
    public bool turnSprite = true;
    public bool followPlayer = false;
    public float knockbackRecovery = 50;
    public float airKnockbackRecovery = 25;
    public LayerMask floor;
    
    private Vector2 velocity;
    private float knockbackX;
    private int directionX = 1;
    private bool ignoreFall = false;
    private bool knockup = false;
    private const float gravity = -40.625f;

    private Controller2D controller;
    private new Collider2D collider;
    private SpriteRenderer sprite;
    private Enemy enemy;
    private GameManager gm;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        enemy = GetComponent<Enemy>();
        gm = GameManager.instance;
    }

    void Update()
    {
        if (followPlayer)
        {
            if (transform.position.x < gm.player.transform.position.x)
            {
                directionX = 1;
            }
            else
            {
                directionX = -1;
            }
        }

        if ((controller.collisions.above || controller.collisions.below) && !knockup)
        {
            velocity.y = 0;
        }

        if (controller.collisions.left || controller.collisions.right)
        {
            directionX *= -1;
        }

        // turn on edge
        if (weight > 0 && controller.collisions.below)
        {
            Vector2 origin = new Vector2(directionX == 1 ? collider.bounds.max.x : collider.bounds.min.x, collider.bounds.min.y);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, collider.bounds.size.x + 0.1f, floor);
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            Debug.DrawLine(origin, origin + Vector2.down);

            if (slopeAngle < 80 && slopeAngle != 0)
            {
                ignoreFall = true;
            }

            if ((!hit || hit.distance > 0.05) && !ignoreFall)
            {
                directionX *= -1;
            }

            if (ignoreFall && hit.distance < 0.05)
            {
                ignoreFall = false;
            }
        }
        
        if (turnSprite)
        {
            sprite.flipX = directionX != 1;
        }

        enemy.direction = directionX;
        velocity.x = moveSpeed * directionX + knockbackX;
        velocity.y += gravity * weight * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (knockbackX > 0)
        {
            knockbackX -= (controller.collisions.below ? knockbackRecovery : airKnockbackRecovery) * Time.deltaTime;
            if (knockbackX < 0)
            {
                knockbackX = 0;
            }
        }
        else if (knockbackX < 0)
        {
            knockbackX += (controller.collisions.below ? knockbackRecovery : airKnockbackRecovery) * Time.deltaTime;
            if (knockbackX > 0)
            {
                knockbackX = 0;
            }
        }

        knockup = false;
    }

    public void Knockback(Vector2 knockback)
    {
        ignoreFall = true;

        if (weight > 0)
        {
            knockup = true;
            velocity.y = knockback.y;
        }

        if (knockback.x > 0)
        {
            knockbackX = Mathf.Max(knockbackX, knockback.x);
        }
        else
        {
            knockbackX = Mathf.Min(knockbackX, knockback.x);
        }
    }
}
