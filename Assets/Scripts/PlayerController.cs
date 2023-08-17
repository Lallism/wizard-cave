using UnityEngine;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(Player))]
public class PlayerController : MonoBehaviour
{
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float maxFallSpeed = 10f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float airJumpTime = .1f;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;
    float currentAirTime = 0;
    bool canJump = false;

    Controller2D controller;
    Player player;
    GameManager gm;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        player = GetComponent<Player>();
        gm = GameManager.instance;

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        //Debug.Log("Gravity =" + gravity);
    }

    void Update()
    {
        if (!gm.gamePaused)
        {
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }

            if (controller.collisions.below)
            {
                currentAirTime = 0;
                canJump = true;
            }
            else
            {
                currentAirTime += Time.deltaTime;
                if (currentAirTime > airJumpTime)
                {
                    canJump = false;
                }
            }

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (Input.GetButtonDown("Jump") && canJump)
            {
                velocity.y = maxJumpVelocity;
                canJump = false;
            }

            if (Input.GetButtonUp("Jump"))
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }

            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne);
            velocity.y += gravity * Time.deltaTime;
            if (velocity.y < -maxFallSpeed)
            {
                velocity.y = -maxFallSpeed;
            }
            controller.Move(velocity * Time.deltaTime);

            if (input.x > 0)
            {
                player.sprite.flipX = false;
                player.direction = 1;
            }
            else if (input.x < 0)
            {
                player.sprite.flipX = true;
                player.direction = -1;
            }

            if (Input.GetButtonDown("Swap Spell Left"))
            {
                player.activeSlot--;
                if (player.activeSlot < 0)
                {
                    player.activeSlot = player.equippedSpells.Length - 1;
                }
                gm.SetBorder();
            }

            if (Input.GetButtonDown("Swap Spell Right"))
            {
                player.activeSlot++;
                if (player.activeSlot >= player.equippedSpells.Length)
                {
                    player.activeSlot = 0;
                }
                gm.SetBorder();
            }

            if (Input.GetButton("Cast Spell") && player.equippedSpells[player.activeSlot] != null)
            {
                player.CastSpell();
            }

            if (Input.GetButtonDown("Attack"))
            {
                player.MeleeAttack();
            }

            if (Input.GetButtonDown("Activate") && player.interactable != null)
            {
                player.interactable.Activate();
            }
        }
    }
}
