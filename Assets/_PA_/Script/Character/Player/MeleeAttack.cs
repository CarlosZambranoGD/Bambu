using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    [Tooltip("What layers should be hit")]
    public LayerMask CollisionMask;
    [Tooltip("Hit more than one enemy at the same time")]
    public bool multiDamage = false;
    [Tooltip("Set the delay attack to sync with the animation")]
    public float delayCheckTarget = 0.1f;
    [Tooltip("Give damage to the enemy or object")]
    public int meleeDamage = 30;
    [Range(0,100)]
    public  int criticalChance = 1;     //percent perform the critical x2 damage

    [Tooltip("Apply the force to enemy if they are hit, only for Rigidbody object")]
    public float pushObject = 10;
    public Transform MeleePoint;
    public float areaSize;

    public float attackRate = 0.2f;

    public AudioClip soundAttack;
    public AudioClip[] soundHitEnemy;
    public AudioClip critSound;
    public GameObject hitEffect;
    public GameObject critFX;

    float timer = 0;

    [Header("HIT EFFECT")]
    public bool playEarthQuakeOnHit = true;
    public float _eqTime = 0.1f;
    public float _eqSpeed = 60;
    public float _eqSize = 1.5f;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    public bool Attack()
    {
        if (timer > 0)
            return false;

       
        timer = attackRate + delayCheckTarget;
        Invoke("Check4Hit", delayCheckTarget);
        return true;
    }

    void Check4Hit()
    {
        SoundManager.PlaySfx(soundAttack);
        var hits = Physics2D.CircleCastAll(MeleePoint.position, areaSize, Vector2.zero, 0, CollisionMask);

        if (hits == null)
            return;

        foreach (var hit in hits)
        {
            var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (damage == null)
                continue;

            var isProjectile = (Projectile)hit.collider.gameObject.GetComponent(typeof(Projectile));

            var finalDamage = (int)( meleeDamage * Random.Range(0.95f,1.05f));

            bool isCritical = false;
            if ((hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemies")) && (Random.Range(0, 100) < (criticalChance)))
                isCritical = true;


            if (isProjectile)
            {
                isProjectile.Speed *= -1;
                isProjectile.LayerCollision = isProjectile.LayerCollision | GameManager.Instance.enemyLayer;
            }
            else
            {
                damage.TakeDamage(finalDamage, new Vector2(pushObject * (transform.position.x < hit.transform.position.x ? 1:-1), 0), gameObject, Vector2.zero);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
                {
                    if (isCritical)
                        FloatingTextManager.Instance.ShowText(finalDamage * 2 + "\nCritical!", Vector2.up * 1f, Color.white, hit.transform.position,2f);
                    else
                        FloatingTextManager.Instance.ShowText(finalDamage + "", Vector2.up * 1f, Color.white, hit.transform.position);

                }
            }
            if (playEarthQuakeOnHit)
            {
                CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
            }

            if (isCritical)
                SoundManager.PlaySfx(critSound);
            else
                SoundManager.PlaySfx(soundHitEnemy);

            if (isCritical && critFX)
                SpawnSystemHelper.GetNextObject(critFX, true).transform.position = hit.point;
            else if (hitEffect)
                SpawnSystemHelper.GetNextObject(hitEffect, true).transform.position = hit.point;


            if (!multiDamage)
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (MeleePoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(MeleePoint.position, areaSize);
    }
}