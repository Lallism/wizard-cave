using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    private enum AttackState
    {
        None, Attacking, Casting
    }

    [System.Serializable]
    public class EquippedSpell
    {
        public Spell spell;
        [HideInInspector] public float currentCooldown;

        public EquippedSpell(Spell spell)
        {
            this.spell = spell;
        }
    }

    public float maxMana = 100f;
    [HideInInspector] public float currentMana;
    public float manaRegen = 10f;
    public float manaPerHit = 5f;
    public Vector2 meleeKnockback;

    public EquippedSpell[] equippedSpells;
    [HideInInspector] public int activeSlot = 0;

    public List<Key> keys = new List<Key>();

    public float invulnerabilityTime = 3f;
    public SpriteRenderer sprite;
    public Collider2D attackHitbox;
    public LayerMask interactionMask;
    [HideInInspector] public Interactable interactable;

    private AttackState attackState = AttackState.None;
    private Animator animator;
    private Camera mainCamera;
    private GameManager gm;

    public override void Start()
    {
        base.Start();
        currentMana = maxMana;
        gm = GameManager.instance;
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        for (int i = 0; i < equippedSpells.Length; i++)
        {
            EquippedSpell equippedSpell = equippedSpells[i];
            if (equippedSpell.spell != null)
            {
                gm.SetSpellIcon(equippedSpell.spell.icon, i);
            }
        }
    }

    private void Update()
    {
        foreach (EquippedSpell equippedSpell in equippedSpells)
        {
            if (equippedSpell.currentCooldown > 0)
            {
                equippedSpell.currentCooldown -= Time.deltaTime;
            }
        }

        if (currentMana < maxMana)
        {
            currentMana += manaRegen * Time.deltaTime;
        }

        interactable = null;

        // check for interactable objects
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size * 0.95f, 0, Vector2.right * direction, 1f, interactionMask);

        if (hit && hit.collider.CompareTag("Interactable"))
        {
            interactable = hit.collider.GetComponent<Interactable>();
            gm.interactPromptBg.gameObject.SetActive(true);
            gm.interactPromptBg.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position + Vector3.up);
            gm.interactPrompt.sprite = interactable.interactSprite;
            gm.interactPrompt.SetNativeSize();
        }
        else
        {
            gm.interactPromptBg.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (gm.interactPromptBg != null)
        {
            gm.interactPromptBg.gameObject.SetActive(false);
        }
    }

    public void CastSpell()
    {
        if (attackState == AttackState.None)
        {
            castPoint.transform.localPosition = new Vector2(Mathf.Abs(castPoint.transform.localPosition.x) * direction, castPoint.transform.localPosition.y);

            EquippedSpell selectedSpell = equippedSpells[activeSlot];

            if (selectedSpell.spell != null)
            {
                if (currentMana >= selectedSpell.spell.manaCost && selectedSpell.currentCooldown <= 0)
                {
                    attackState = AttackState.Casting;
                    Invoke(nameof(EndCast), selectedSpell.spell.castTime);
                    selectedSpell.spell.ActivateSpell(this, castPoint.position, null, "Enemy");
                    currentMana -= selectedSpell.spell.manaCost;
                    selectedSpell.currentCooldown += selectedSpell.spell.cooldown;
                    gm.UpdateUI();
                }
            }
        }
    }

    public void MeleeAttack()
    {
        if (attackState == AttackState.None)
        {
            animator.SetTrigger("Attack");
            attackHitbox.transform.localPosition = new Vector2(Mathf.Abs(attackHitbox.transform.localPosition.x) * direction, attackHitbox.transform.localPosition.y);
            attackHitbox.gameObject.SetActive(true);
            attackState = AttackState.Attacking;
        }
    }

    public void EndAttack()
    {
        attackHitbox.gameObject.SetActive(false);
        attackState = AttackState.None;
    }

    public void EndCast()
    {
        attackState = AttackState.None;
    }

    public void LearnSpell(Spell spell)
    {
        for (int i = 0; i < equippedSpells.Length; i++)
        {
            if (equippedSpells[i].spell == null)
            {
                equippedSpells[i] = new EquippedSpell(spell);
                gm.SetSpellIcon(equippedSpells[i].spell.icon, i);
                return;
            }
        }

        equippedSpells[activeSlot] = new EquippedSpell(spell);
        gm.SetSpellIcon(equippedSpells[activeSlot].spell.icon, activeSlot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Collectible collectible = collision.gameObject.GetComponent<Collectible>();
            currentHealth += collectible.heal;

            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }

            if (collectible.key != null)
            {
                keys.Add(collectible.key);
                gm.AddToKeyList(collectible.key);
            }

            gm.score += collectible.score;
            gm.SpawnParticles(collectible.particles, collectible.transform.position);
            gm.UpdateUI();
            Destroy(collision.gameObject);
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
        gm.UpdateUI();
        invulnerable = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.6f);
        Invoke(nameof(EndInvulnerability), invulnerabilityTime);
    }

    public override void Die()
    {
        gm.KillPlayer();
    }

    private void EndInvulnerability()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        invulnerable = false;
    }
}
