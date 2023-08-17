using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();

        if (collision.gameObject.CompareTag("Enemy") && !character.invulnerable)
        {
            player.currentMana += player.manaPerHit;

            if (player.currentMana > player.maxMana)
            {
                player.currentMana = player.maxMana;
            }

            character.TakeDamage(player.meleeDamage);
            character.GetComponent<EnemyController>().Knockback(new Vector2(player.meleeKnockback.x * player.direction, player.meleeKnockback.y));
        }
    }
}
