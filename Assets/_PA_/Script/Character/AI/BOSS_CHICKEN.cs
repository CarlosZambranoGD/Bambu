using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS_CHICKEN : BossManager, ICanTakeDamage, IListener
{
    public float movingSpeed = 3;
    [Range(1, 1000)]
    public int health = 500;
    [ReadOnly] public int currentHealth;
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);
    protected HealthBarEnemyNew healthBar;

    [Header("EARTH QUAKE")]
    public float _eqTime = 0.3f;
    public float _eqSpeed = 60;
    public float _eqSize = 1.5f;

    [Header("SOUND")]
    public AudioClip attackSound;
    public AudioClip deadSound;
    public AudioClip hurtSound;
    public AudioClip[] hitGroundSound;
    public AudioClip jumpUpSound;
    public AudioClip warningSound;
    public AudioClip jumpDownSound;

    [HideInInspector]
    public bool isDead = false;

    Animator anim;
    [ReadOnly] public bool moving = false;
    [ReadOnly] public bool isPlaying = false;

    Rigidbody2D rig;

    public bool isFacingRight()
    {
        return transform.rotation.y == 0 ? true : false;
    }

    // ACTION
    [Header("*** CHICKEN ***")]
    public float minDelay = 4;
    public float maxDelay = 6;

    public float localLeftPosX = -5f;
    public float localRightPosX = 5f;
    public GameObject shieldFX;
    public GameObject hitGroundFX;
    public GameObject stunningFX;
    public float stunningTime = 3;
    public float jumpForce = 100;

    public float warningTime = 1.5f;
    public GameObject warningFX;

    Vector2 leftPos, rightPos;
    [ReadOnly] public Vector2 _direction;
    bool isJumpingSkill = false;

    void Flip()
    {
        _direction = -_direction;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 180 : 0, transform.rotation.z));
    }

    IEnumerator ACTION_CO()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            shieldFX.SetActive(true);
            isJumpingSkill = true;
            rig.isKinematic = true;
            anim.SetBool("prepare", true);
            yield return new WaitForSeconds(1);
            anim.SetBool("prepare", false);

            rig.isKinematic = false;
            rig.velocity = new Vector2(0, jumpForce);
            SoundManager.PlaySfx(jumpUpSound);

            while (Mathf.Abs(transform.position.y - Camera.main.transform.position.y) < 8 )
            {
                yield return null;
            }
            rig.isKinematic = true;
            rig.velocity = Vector2.zero;

            yield return new WaitForSeconds(3);
            Vector2 warningPos = new Vector2(GameManager.Instance.Player.transform.position.x, Camera.main.transform.position.y + Camera.main.orthographicSize - 2);
            Instantiate(warningFX, warningPos, Quaternion.identity).GetComponent<AutoDestroy>().destroyAfterTime = warningTime;
            SoundManager.PlaySfx(warningSound);
            yield return new WaitForSeconds(warningTime);

            anim.SetTrigger("jumpDown");
            SoundManager.PlaySfx(jumpDownSound);
            transform.position = new Vector2(warningPos.x, transform.position.y);
            rig.isKinematic = false;
            rig.velocity = new Vector2(0, -jumpForce);

            //check hit ground
            bool hitGround = false;
            while(hitGround == false)
            {
                if (Physics2D.Raycast(transform.position, Vector2.down, 1f, GameManager.Instance.groundLayer))
                    hitGround = true;

                yield return null;
            }

            anim.SetBool("isStunning", true);
            rig.velocity = Vector2.zero;
            rig.isKinematic = false;
            Instantiate(hitGroundFX, transform.position, Quaternion.identity);

            foreach (var sound in hitGroundSound)
                SoundManager.PlaySfx(sound);

            shieldFX.SetActive(false);
            stunningFX.SetActive(true);

            yield return new WaitForSeconds(stunningTime);
            stunningFX.SetActive(false);
            anim.SetBool("isStunning", false);

            yield return new WaitForSeconds(1);
            isJumpingSkill = false;
            SoundManager.PlaySfx(attackSound);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
        healthBar.Init(transform, (Vector3)healthBarOffset);
        stunningFX.SetActive(false);
        shieldFX.SetActive(false);
        currentHealth = health;
        rig = GetComponent<Rigidbody2D>();

        leftPos = transform.position + Vector3.right * localLeftPosX;
        rightPos = transform.position + Vector3.right * localRightPosX;

        _direction = isFacingRight() ? Vector2.right : Vector2.left;
    }

    public override void Play()
    {
        if (isPlaying)
            return;

        isPlaying = true;

        StartCoroutine(ACTION_CO());
        SoundManager.PlaySfx(attackSound);
    }

    void Update()
    {
       
        if (!isPlaying || isJumpingSkill || isDead || GameManager.Instance.State != GameManager.GameState.Playing || GameManager.Instance.Player.isFinish)
        {
            anim.SetBool("moving", false);
            return;
        }else
            anim.SetBool("moving", true);

        transform.Translate(movingSpeed * Time.deltaTime * _direction.x, 0, 0, Space.World);

        if (isFacingRight())
        {
            if (transform.position.x >= rightPos.x)
            {
                Flip();
                SoundManager.PlaySfx(attackSound);
            }
        }
        else
        {
            if (transform.position.x <= leftPos.x)
            {
                Flip();
                SoundManager.PlaySfx(attackSound);
            }
        }
    }

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (!isPlaying || isDead || isJumpingSkill)
            return;

        currentHealth -= (int)damage;
        isDead = currentHealth <= 0 ? true : false;

        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        if (isDead)
        {
            StopAllCoroutines();
            CancelInvoke();

            anim.SetBool("isDead", true);
            var boxCo = GetComponents<BoxCollider2D>();
            foreach (var box in boxCo)
            {
                box.enabled = false;
            }
            var CirCo = GetComponents<CircleCollider2D>();
            foreach (var cir in CirCo)
            {
                cir.enabled = false;
            }

            StartCoroutine(BossDieBehavior());
        }
        else
        {
            anim.SetTrigger("hurt");
            SoundManager.PlaySfx(hurtSound, 0.7f);
        }
    }

    [Header("EFFECT WHEN DIE")]
    public GameObject dieExplosionFX;
    public Vector2 dieExplosionSize = new Vector2(2, 3);
    public AudioClip dieExplosionSound;

    IEnumerator BossDieBehavior()
    {
        SoundManager.Instance.PauseMusic(true);
        anim.enabled = false;
        GameManager.Instance.MissionStarCollected = 3;
        ControllerInput.Instance.StopMove();
        MenuManager.Instance.TurnController(false);
        MenuManager.Instance.TurnGUI(false);
        SoundManager.PlaySfx(deadSound);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            if (dieExplosionFX)
                Instantiate(dieExplosionFX, transform.position + new Vector3(Random.Range(-dieExplosionSize.x, dieExplosionSize.x), Random.Range(0, dieExplosionSize.y), 0), Quaternion.identity);
            SoundManager.PlaySfx(dieExplosionSound);
            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
            yield return new WaitForSeconds(0.5f);
        }

        BlackScreenUI.instance.Show(2, Color.white);
        for (int i = 0; i < 4; i++)
        {
            if (dieExplosionFX)
                Instantiate(dieExplosionFX, transform.position + new Vector3(Random.Range(-dieExplosionSize.x, dieExplosionSize.x), Random.Range(0, dieExplosionSize.y), 0), Quaternion.identity);
            SoundManager.PlaySfx(dieExplosionSound);
            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
            yield return new WaitForSeconds(0.5f);
        }

        BlackScreenUI.instance.Hide(1);
        GameManager.Instance.GameFinish(1);
        gameObject.SetActive(false);
    }

    public void IPlay()
    {

    }

    public void ISuccess()
    {

    }

    public void IPause()
    {

    }

    public void IUnPause()
    {

    }

    public void IGameOver()
    {
        StopAllCoroutines();
    }

    public void IOnRespawn()
    {

    }

    public void IOnStopMovingOn()
    {

    }

    public void IOnStopMovingOff()
    {

    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(leftPos, 0.3f);
            Gizmos.DrawWireSphere(rightPos, 0.3f);
            Gizmos.DrawLine(leftPos, rightPos);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.right * localLeftPosX, 0.3f);
            Gizmos.DrawWireSphere(transform.position + Vector3.right * localRightPosX, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * localRightPosX);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * localLeftPosX);
        }
    }
}
