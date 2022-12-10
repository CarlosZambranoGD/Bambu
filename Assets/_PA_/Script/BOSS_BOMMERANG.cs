using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS_BOMMERANG : BossManager, ICanTakeDamage, IListener
{
    public float speed = 1f;
    [Range(1, 1000)]
    public int health = 10;
    [ReadOnly] public int currentHealth;
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);
    protected HealthBarEnemyNew healthBar;
    public float gravity = 35f;

    [Header("EARTH QUAKE")]
    public float _eqTime = 0.3f;
    public float _eqSpeed = 60;
    public float _eqSize = 1.5f;

    [Header("SOUND")]
    public AudioClip showupSound;
    public AudioClip attackSound;
    public AudioClip deadSound;
    public AudioClip hurtSound;

    [HideInInspector]
    protected Vector3 velocity;
    protected float velocityXSmoothing = 0;
    Controller2D controller;
    Animator anim;
    bool moving = false;

    CheckTargetHelper checkTargetHelper;
    [ReadOnly] public bool isPlaying = false;

    [HideInInspector]
    public bool isDead = false;

    public bool isFacingRight()
    {
        return transform.rotation.y == 0 ? true : false;
    }

    void Start()
    {
        controller = GetComponent<Controller2D>();
        checkTargetHelper = GetComponent<CheckTargetHelper>();
        anim = GetComponent<Animator>();
        
        _direction = isFacingRight() ? Vector2.right : Vector2.left;

        currentHealth = health;

        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);
        healthBar.Init(transform, (Vector3)healthBarOffset);

    }

    public override void Play()
    {
        if (isPlaying)
            return;

        isPlaying = true;
        moving = true;
        StartCoroutine(BoomerangAttackCo());
    }

    [HideInInspector] public bool isPlayerInRange = false;
    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(velocity.x));

        if (!isPlaying || isDead || GameManager.Instance.State != GameManager.GameState.Playing || GameManager.Instance.Player.isFinish || isBoomerangeAttacking)
        {
            velocity.x = 0;

            return;
        }

        bool allowChasing = true;
        if (moving)
        {
            var hitTarget = checkTargetHelper.CheckTarget();
            if (hitTarget)
            {
                isPlayerInRange = true;
                if (Mathf.Abs(hitTarget.transform.position.x - transform.position.x) < 0.3f && !GameManager.Instance.Player.controller.collisions.below)
                {
                    allowChasing = false;
                }
                else if (Mathf.Abs(hitTarget.transform.position.x - transform.position.x) > 0.1f)
                {
                    if ((isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x) || (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x))
                    {
                        Flip();
                    }
                }
                else
                    allowChasing = false;
            }
            else
            {
                allowChasing = false;
                isPlayerInRange = false;
            }
        }

        float targetVelocityX = _direction.x * speed;
        velocity.x = moving ? Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f) : 0;
        velocity.y += -gravity * Time.deltaTime;
        if (!allowChasing || !moving)
            velocity.x = 0;
    }



    void LateUpdate()
    {
        if (isDead)
            return;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;
    }

    private Vector2 _direction;

    void Flip()
    {
        _direction = -_direction;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 180 : 0, transform.rotation.z));
    }


    void LookAtPlayer()
    {
        if ((isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x) || (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x))
        {
            Flip();
        }
    }

    public void SetMoving(bool move)
    {
        velocity.x = 0;
    }

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (!isPlaying || isDead)
            return;

        //Debug.LogError(damage);
        currentHealth -= (int)damage;
        isDead = currentHealth <= 0 ? true : false;

        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);

        if (isDead)
        {
            StopAllCoroutines();
            CancelInvoke();

            if (_BA_bullet != null)
                Destroy(_BA_bullet);

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
                anim.SetTrigger("hit");
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

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            Instantiate(dieExplosionFX, transform.position + new Vector3(Random.Range(-dieExplosionSize.x, dieExplosionSize.x), Random.Range(0, dieExplosionSize.y), 0), Quaternion.identity);
            SoundManager.PlaySfx(dieExplosionSound);
            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
            yield return new WaitForSeconds(0.5f);
        }

        BlackScreenUI.instance.Show(2, Color.white);
        for (int i = 0; i < 4; i++)
        {
            Instantiate(dieExplosionFX, transform.position + new Vector3(Random.Range(-dieExplosionSize.x, dieExplosionSize.x), Random.Range(0, dieExplosionSize.y), 0), Quaternion.identity);
            SoundManager.PlaySfx(dieExplosionSound);
            CameraPlay.EarthQuakeShake(_eqTime, _eqSpeed, _eqSize);
            yield return new WaitForSeconds(0.5f);
        }

        BlackScreenUI.instance.Hide(1);
        GameManager.Instance.GameFinish(1);
        gameObject.SetActive(false);

    }

    #region BOOMERANG ATTACK
    [Header("BOOMERANG ATTACK")]
    public float minDelay = 2;
    public float maxDelay = 3;

    public GameObject BA_bullet;
    public Transform BA_startPoint;
    public float BA_distance = 8;
    public float BA_bulletSpeed = 3;
    public AudioClip BA_soundBegin, BA_soundEnd;
    GameObject _BA_bullet;
    [ReadOnly] public bool isBoomerangeAttacking = false;
    bool allowThrowObject = false;

    IEnumerator BoomerangAttackCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            isBoomerangeAttacking = true;
            anim.SetBool("boomerangAttack", true);

            SetMoving(false);

            allowThrowObject = false;
            while (!allowThrowObject) { yield return null; }        //wait Anim event
            allowThrowObject = false;
            SoundManager.PlaySfx(BA_soundBegin);
            //float startTime = Time.time;
            _BA_bullet = Instantiate(BA_bullet, BA_startPoint.position, Quaternion.identity) as GameObject;
            Vector3 BA_target = (Vector2)BA_startPoint.position + BA_distance * (isFacingRight() ? Vector2.right : Vector2.left);

            float angle = 0;
            while (angle < 180)
            {
                angle += BA_bulletSpeed * Time.deltaTime;
                _BA_bullet.transform.position = Vector2.Lerp(BA_startPoint.position, BA_target, Mathf.Sin(angle * Mathf.Deg2Rad));
                yield return null;
            }

            Destroy(_BA_bullet);
            isBoomerangeAttacking = false;
            anim.SetBool("boomerangAttack", false);
            SoundManager.PlaySfx(BA_soundEnd);
            SetMoving(true);
        }
    }

    public void AnimBoomerang()
    {
        allowThrowObject = true;
    }
    #endregion



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
}
