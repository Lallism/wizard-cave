using UnityEngine;

[CreateAssetMenu(menuName = "Spells/ProjectileSpell")]
public class ProjectileSpell : Spell
{
    public Projectile projectile;
    public ParticleSystem collisionParticle;
    [Min(1)] public int projectileAmount = 1;
    public float direction;
    public bool randomizeDirection;
    public float gravity = 0;
    public float arc;
    public float spacing;
    public float speed = 10f;
    public float range = 25f;
    public float baseDamage = 10f;
    public float knockback;
    public DamageType damageType;
    [Min(0)] public int pierce = 0;
    public bool aimAtTarget;
    public bool followTarget;
    public float turnSpeed;
    public Spell spawnSpell;

    public override void ActivateSpell(Character caster, Vector2 castPoint, Transform target, string targetTag)
    {
        for (int i = 0; i < projectileAmount; i++)
        {
            Vector2 position = new Vector2(castPoint.x, castPoint.y + (spacing * i) - (spacing * (projectileAmount - 1) / 2));
            Projectile clone = Instantiate(projectile, position, Quaternion.identity);

            if (aimAtTarget && target != null)
            {
                clone.transform.right = target.position - clone.transform.position;

                if (randomizeDirection)
                {
                    clone.transform.Rotate(0, 0, Random.Range(direction - arc / 2, direction + arc / 2));
                }
                else if (projectileAmount > 1)
                {
                    clone.transform.Rotate(0, 0, direction + (arc / (projectileAmount - 1) * i) - (arc / 2));
                }
                else
                {
                    clone.transform.Rotate(0, 0, direction);
                }
            }
            else
            {
                if (randomizeDirection)
                {
                    clone.transform.Rotate(0, 0, (Random.Range(direction - arc / 2, direction + arc / 2) * caster.direction) + (caster.direction * 90) - 90);
                }
                else if (projectileAmount > 1)
                {
                    clone.transform.Rotate(0, 0, (direction * caster.direction) + (arc / (projectileAmount - 1) * i * caster.direction) - (caster.direction * arc / 2) + (caster.direction * 90) - 90);
                }
                else
                {
                    clone.transform.Rotate(0, 0, (direction * caster.direction) + (caster.direction * 90) - 90);
                }
            }

            clone.speed = speed;
            clone.gravity = gravity;
            clone.lifespan = range / speed;
            clone.damage = baseDamage * caster.spellPower;
            clone.knockback = knockback;
            clone.damageType = damageType;
            clone.pierce = pierce;
            clone.collisionParticle = collisionParticle;
            clone.spawnSpell = spawnSpell;
            clone.caster = caster;
            clone.followTarget = followTarget;
            clone.target = target;
            clone.turnSpeed = turnSpeed;
            clone.targetTag = targetTag;
            clone.Shoot();
        }
    }

}
