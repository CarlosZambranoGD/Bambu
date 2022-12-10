using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("ADDP/Enemy AI/[ENEMY] Bomber Attack")]
public class EnemyBomberAttack : MonoBehaviour
{
    public LayerMask targetPlayer;
    public Transform checkPoint;
    public float checkRadius = 2;
    public Miner enemyBomb;
    [Tooltip("Damage depend on the distance of the bomb with the player")]
    public int damageMax = 100;
    public float damageRadius = 5;
    bool allowCheckTarget = false;
    public float delayBlowUp = 1.5f;
    public AudioClip soundActiveBomb;

   [HideInInspector] public bool isBlowingUp = false;
    Enemy ownerEnemy;

    private void Start()
    {
        ownerEnemy = GetComponent<Enemy>();
    }

    //called by the owner
    public void Attack()
    {
        allowCheckTarget = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowCheckTarget || isBlowingUp)
            return;

        var hit = Physics2D.CircleCast(checkPoint.position, checkRadius, Vector2.zero, 0, targetPlayer);
        if (hit)
        {
            allowCheckTarget = false;
            if (enemyBomb)
                BlowUp();
            else
                Debug.LogError("MUST PLACE THE EXPLOSION BOMB");
        }
    }

    IEnumerator BlowUpCo()
    {
        isBlowingUp = true;
        ownerEnemy.SetEnemyState(ENEMYSTATE.DEATH);
        ownerEnemy.AnimSetTrigger("bomberBlinking");

        var ascr = gameObject.AddComponent<AudioSource>();
        ascr.clip = soundActiveBomb;
        ascr.Play();

        yield return new WaitForSeconds(delayBlowUp);
        ownerEnemy.AnimSetTrigger("explosion");
        yield return new WaitForSeconds(0.2f);

        if (this)
        {
            StopAllCoroutines();
            Instantiate(enemyBomb, transform.position, Quaternion.identity).Init(true, damageMax, damageRadius);

            var spawnItem = GetComponent<EnemySpawnItem>();
            if (spawnItem != null)
            {
                spawnItem.SpawnItem();
            }

            ownerEnemy.TakeDamage(int.MaxValue, Vector2.zero, gameObject, Vector3.zero, BulletFeature.Explosion);     //destroy the owner
        }

        gameObject.SetActive(false);
    }

    public void BlowUp()
    {
        if (isBlowingUp)
            return;

        StartCoroutine(BlowUpCo());
    }

    void OnDrawGizmos()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkPoint.position, checkRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkPoint.position, damageRadius);
        }
    }
}
