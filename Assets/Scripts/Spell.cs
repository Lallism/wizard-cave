using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public float manaCost = 10;
    public float cooldown = 1;
    public float castTime = 1;
    public Sprite icon;

    public abstract void ActivateSpell(Character caster, Vector2 castPoint, Transform target, string targetTag);
}
