using UnityEngine;

public class SpellPickup : Interactable
{
    public Spell spell;

    public override void Activate()
    {
        GameManager.instance.player.LearnSpell(spell);
        Destroy(gameObject);
    }
}
