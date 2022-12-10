using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ATTACKTYPE {
	RANGE,
	MELEE,
	THROW,
	CALLMINION,
    BOMBER,
	NONE
}

public enum STARTACTION{
	PATROL,
	AIMPLAYER,
	CHASING,
    STAND
}

public enum DETECTPLAYER{
	WalkAndCheckAttack,
	RunAndCheckAttack,
    RushIntoPlayer,
	Nothing
}

public enum DISMISSDETECTPLAYER{
	WalkAndPatrol,
	RunAndPatrol,
	Stand
}

public enum ENEMYSTATE {
	IDLE,
	ATTACK,
    RUSHINTOPLAYER,
    WALK,
	RUN,
	HIT,
	DEATH
}

public enum ENEMYEFFECT {
	NONE,
	BURNING,
	FREEZE,
	SHOKING,
	EXPLOSION
}

public enum STARTBEHAVIOR {
	NONE,
    JUMPUP
}

public enum DIEBEHAVIOR {
	NORMAL,
	BLOWUP,
    FALLOUT
}

public enum ENEMYTYPE{
	ONGROUND,
	INWATER,
	FLY
}

[RequireComponent(typeof(CheckTargetHelper))]

public class Enemy : MonoBehaviour,ICanTakeDamage, IListener, ICanFreeze, ICanBurn, ICanShock {
    [Header("Behavier")]

    public STARTACTION startAction;
    public STARTBEHAVIOR startBehavior;
    public ATTACKTYPE attackType;
    public DETECTPLAYER detectPlayerAct;
    public DISMISSDETECTPLAYER dismissDetectPlayerAct;
    public DIEBEHAVIOR dieBehavior;
    public float spawnDelay = 1;
    
    [Header("Setup")]
    public ENEMYTYPE enemyType;
    [Tooltip("If anim = null, get compomnent itself")]
    public Animator anim;
    [Range(1, 100)]
    public int maxHealth = 1;
    public float gravity = 35f;
    public float walkSpeed = 3;
    public float runSpeed = 5;
    [Tooltip("If choose startBehavior = ShowUp => Will jump this force")]
    public float jumpShowUpForce = 10;
    protected Vector2 rushIntoPlayerPoint;

	[Header("++++++PATROL++++++")]
	public bool usePatrol = true;
	public float waitingTurn = 0.5f;
	public float patrolLimitLeft = 2;
	public float patrolLimitRight = 3;
	protected bool isWaiting = false;
	protected float waitingTime = 0;
	protected float _patrolLimitLeft, _patrolLimitRight;

	[Header("OPTION")]
    [Tooltip("For ONGROUNDED enemy type: only move horizontal when stand on ground")]
    public bool onlyMoveWhenGrounded = false;
	public bool canBeKnockBackWhenHit = true;
    [Header("Detect and Dismiss Player")]
    public float delayAttackWhenDetectPlayer = 0.5f;
    [ReadOnlyAttribute] public CheckTargetHelper checkTarget;
	public float dismissPlayerDistance = 10;
    public float dismissPlayerWhenStandSecond = 5;
    [ReadOnly] public float countingStanding = 0;
	public bool isPlayerDetected{get;set;}
	public GameObject warningIconDetectPlayer;
	public Vector2 chasingOffset = new Vector2(1.2f,1f);
    
    protected EnemyRangeAttack rangeAttack;
    protected EnemyMeleeAttack meleeAttack;
    protected EnemyBomberAttack bomberAttack;
    protected EnemyThrowAttack throwAttack;
    protected EnemyCallMinion callMinion;
    
    [Header("Rush Into Player Attack for Fly enemy")]
    public float rushIntoPlayerSpeed = 10;
    protected Vector2 rushIntoPlayerDirection;

    [Header("In Water Setup")]
	public LayerMask waterLayer;
	[ReadOnlyAttribute] public float waterLimitUp;

	public float sockingTime = 0.5f;
	public float destroyTime = 1.5f;
	public int pointToGivePlayer = 100;
	public LayerMask playerLayer;	//detect player to attack/chasing
	[Tooltip("if true, the enemy will be fall from the higher platform")]
	public bool canBeFallDown = false;

	public GameObject dieFX, hitFX;
	public GameObject explosionFX;

    [ReadOnly] public ENEMYSTATE enemyState;
	protected ENEMYEFFECT enemyEffect;
	[Space]
	public Vector2 healthBarOffset  = new Vector2(0,1.5f);

	[Header("Freeze Option")]
	public bool canBeFreeze = true;
	public float timeFreeze = 5;
	public GameObject frozenObjFX;

	//[Header("Burning Option")]
	[HideInInspector] public bool canBeBurn = true;
	[HideInInspector] public float timeBurn = 2;
	[HideInInspector] public GameObject dieBurnFX;
	float damageBurningPerFrame;

	//[Header("Shocking Option")]
	[HideInInspector] public bool canBeShock = true;
	[HideInInspector] public float timeShocking = 2f;
	float damageShockingPerFrame;
    
	[Header("Sound")]
    public AudioClip[] soundDetectPlayer;
    public AudioClip[] soundHit;
	public AudioClip[] soundDie;
    public AudioClip soundRushToPlayerAttack;

	int currentHealth;
	Vector2 hitPos;
	public bool isPlaying{ get; set; }
    public bool isDead { get; set; }

    public bool isStopping { get; set; }

	protected HealthBarEnemyNew healthBar;
	protected ShakingHelper shakingHelper;
	
	protected float moveSpeed;

	public bool isFacingRight(){
		return transform.rotation.y == 0 ? true : false;
	}

	protected virtual void OnEnable(){
		isPlaying = true;
	}

	public virtual void Start(){
        controller = GetComponent<Controller2D>();
        currentHealth = maxHealth;
		moveSpeed = walkSpeed;
		_patrolLimitLeft = transform.position.x - patrolLimitLeft;
		_patrolLimitRight = transform.position.x + patrolLimitRight;

        if (enemyType == ENEMYTYPE.FLY)
        {
            controller.HandlePhysic = false;
            gravity = 0;
        }

		var healthBarObj = (HealthBarEnemyNew) Resources.Load ("HealthBar", typeof(HealthBarEnemyNew));
		healthBar = (HealthBarEnemyNew) Instantiate (healthBarObj, healthBarOffset, Quaternion.identity);
        healthBar.Init(transform, (Vector3) healthBarOffset);

        if (anim == null)
            anim = GetComponent<Animator>();
		shakingHelper = GetComponent<ShakingHelper> ();
		checkTarget = GetComponent<CheckTargetHelper> ();
		if (shakingHelper)
			shakingHelper.enabled = false;

		if (enemyType == ENEMYTYPE.INWATER) {
			RaycastHit2D hit = Physics2D.CircleCast (transform.position, 0.1f, Vector2.zero, 0, waterLayer);
			if (hit) {
				waterLimitUp = hit.collider.gameObject.GetComponent<BoxCollider2D> ().bounds.max.y - 1;
			} else
				Debug.LogError ("YOU NEED PLACE THIS: " + gameObject.name + " TO A WATER ZONE");
		}

        if(warningIconDetectPlayer)
		warningIconDetectPlayer.SetActive (false);

        switch (startBehavior)
        {
            case STARTBEHAVIOR.NONE:
                SetEnemyState(ENEMYSTATE.WALK);
                break;
            case STARTBEHAVIOR.JUMPUP:
                SetEnemyState(ENEMYSTATE.WALK);
                velocity.y = jumpShowUpForce;
                break;
        }
			

		//set up start action
		switch (startAction) {
		case STARTACTION.PATROL:

			break;
		case STARTACTION.AIMPLAYER:
			moveSpeed = 0;
			LookAtPlayer ();
			break;
		case STARTACTION.CHASING:
			DetectPlayer ();
			break;
		default:
			break;
		}

		if (frozenObjFX)
			frozenObjFX.SetActive(false);
	}
	public bool allowLookAtPlayer{ get; set; }
	void LookAtPlayer(){
		allowLookAtPlayer = true;
	}

	public void AnimSetTrigger(string name){
		anim.SetTrigger (name);
	}

	public void AnimSetBool(string name, bool value){
		anim.SetBool (name, value);
	}

	public void AnimSetFloat(string name, float value){
		anim.SetFloat (name, value);
	}

	public void SetEnemyState(ENEMYSTATE state){
		enemyState = state;
	}

	public void SetEnemyEffect(ENEMYEFFECT effect){
		enemyEffect = effect;
	}

	public virtual void Update(){
		if (enemyEffect == ENEMYEFFECT.BURNING)
			CheckDamagePerFrame (damageBurningPerFrame);

		if (enemyEffect == ENEMYEFFECT.SHOKING)
			CheckDamagePerFrame (damageShockingPerFrame);
        if (healthBar != null)
            healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);


	}

	//can call by Alarm action of other Enemy
	public virtual void DetectPlayer(float delayChase = 0){
		if (isPlayerDetected)
			return;
		
		isPlayerDetected = true;
        SoundManager.PlaySfx(soundDetectPlayer);
		StartCoroutine (DelayBeforeChasePlayer (delayChase));
	}


	protected IEnumerator DelayBeforeChasePlayer(float delay){
		SetEnemyState (ENEMYSTATE.IDLE);
        if(warningIconDetectPlayer)
		warningIconDetectPlayer.SetActive (true);
		yield return new WaitForSeconds (delay);
        if(warningIconDetectPlayer)
        warningIconDetectPlayer.SetActive(false);
        if (enemyState == ENEMYSTATE.ATTACK)
        {
            yield break;
        }

		SetEnemyState (ENEMYSTATE.WALK);
		

        if (detectPlayerAct == DETECTPLAYER.RushIntoPlayer)
        {
            AnimSetTrigger("rushIntoPlayer");
            rushIntoPlayerPoint = GameManager.Instance.Player.transform.position + Vector3.up * 0.5f;
            rushIntoPlayerDirection = (rushIntoPlayerPoint - (Vector2)transform.position).normalized;
            SetEnemyState(ENEMYSTATE.RUSHINTOPLAYER);
            SoundManager.PlaySfx(soundRushToPlayerAttack);

            if ((isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x) || (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x))
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 180 : 0, transform.rotation.z));
           
            Destroy(gameObject, 2);
        }
    }

	public virtual void DismissDetectPlayer(){
		if (!isPlayerDetected)
			return;
		
		isPlayerDetected = false;
	}

	public virtual void Parachute(bool openParachute){
	
	}

	public virtual void FixedUpdate(){

	}

	public virtual void Hit(){
		SoundManager.PlaySfx (soundHit);
	}

	public virtual void KnockBack(Vector2 force){
		
	}

	public virtual void Die()
	{
		isPlaying = false;
		isDead = true;
		isPlayerDetected = false;
		SetEnemyState(ENEMYSTATE.DEATH);

		Parachute(false);
		if (warningIconDetectPlayer)
			warningIconDetectPlayer.SetActive(false);

		if (dieFX)
			Instantiate(dieFX, transform.position, dieFX.transform.rotation);

		//try spawn random item
		var spawnItem = GetComponent<EnemySpawnItem>();
		if (spawnItem != null)
		{
			spawnItem.SpawnItem();
		}

		if (enemyEffect == ENEMYEFFECT.SHOKING)
			UnShock();

		if (enemyEffect == ENEMYEFFECT.FREEZE)
			UnFreeze();

		if (enemyEffect == ENEMYEFFECT.EXPLOSION)
		{
			if (explosionFX)
			{
				for (int i = 0; i < Random.Range(1, 3); i++)
				{
					var obj = SpawnSystemHelper.GetNextObject(explosionFX, false);
					obj.transform.position = transform.position;
					obj.SetActive(true);
				}
			}
		}

		SoundManager.PlaySfx(soundDie);
	}

    public virtual void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        TakeDamage(damage, force, instigator, transform.position, BulletFeature.Normal);
    }

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    protected Vector3 velocity;
    protected float velocityXSmoothing = 0;

    public virtual void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector2 hitPosition, BulletFeature bulletType = BulletFeature.Normal)
    {
        if (isDead)
            return;

        if (enemyState == ENEMYSTATE.DEATH)
            return;

        if (isStopping)
            return;

        bool isExplosion = false;

        hitPos = hitPosition;
        currentHealth -= (int)damage;
        if (hitFX)
            SpawnSystemHelper.GetNextObject(hitFX, true).transform.position = hitPos;
      
        switch (bulletType)
        {
            case BulletFeature.Normal:
                break;
            case BulletFeature.Explosion:
                isExplosion = true;
                break;
            case BulletFeature.Shocking:
                Shoking(damage * Time.deltaTime, gameObject);
                break;
            default:
                break;
        }

        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)maxHealth);
		if (currentHealth <= 0)
		{
			if (isExplosion || dieBehavior == DIEBEHAVIOR.BLOWUP || attackType == ATTACKTYPE.BOMBER)
			{
				if (explosionFX)
					Instantiate(explosionFX, transform.position, Quaternion.identity);

				//check if have bomb then active it
				if (attackType == ATTACKTYPE.BOMBER && bomberAttack != null)
				{
					bomberAttack.BlowUp();
					return;
				}

				Destroy(gameObject);
			}
			else if (dieBehavior == DIEBEHAVIOR.FALLOUT)
			{
				velocity = new Vector2(0, 8);
				controller.HandlePhysic = false;
				Die();
			}
			else
				Die();
		}
		else
		{
			Hit();
			if (canBeKnockBackWhenHit)
				KnockBack(force);
		}
    }

	private void CheckDamagePerFrame(float _damage){
		if (enemyState == ENEMYSTATE.DEATH)
			return;

		currentHealth -= (int) _damage;
		if (healthBar)
			healthBar.UpdateValue (currentHealth / (float)maxHealth);
		
		if (currentHealth <= 0)
			Die ();
	}

	#region IListener implementation

	public virtual void IPlay ()
	{
	}

	public virtual void ISuccess ()
	{
	}

	public virtual void IPause ()
	{
	}

	public virtual void IUnPause ()
	{
	}

	public virtual void IGameOver ()
	{
		isPlaying = false;
		SetEnemyState(ENEMYSTATE.IDLE);
	}

	public virtual void IOnRespawn ()
	{
		isPlaying = true;
		SetEnemyState (ENEMYSTATE.WALK);
		DismissDetectPlayer ();
	}

	public virtual void IOnStopMovingOn ()
	{
	}

	public virtual void IOnStopMovingOff ()
	{
	}

	#endregion

	#region ICanFreeze implementation
	public virtual void Freeze (GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.FREEZE)
			return;

		if (enemyEffect == ENEMYEFFECT.BURNING)
			BurnOut ();

		if (enemyEffect == ENEMYEFFECT.SHOKING) {
			UnShock ();
		}

		if (canBeFreeze) {
			enemyEffect = ENEMYEFFECT.FREEZE;
			//anim.enabled = false;
			StartCoroutine (UnFreezeCo ());
			if (frozenObjFX)
				frozenObjFX.SetActive(true);
		}
	}

	IEnumerator UnFreezeCo(){
		if (enemyEffect != ENEMYEFFECT.FREEZE)
			yield break;

		float wait = timeFreeze - 1;
		yield return new WaitForSeconds(wait);
		UnFreeze ();
	}

	void UnFreeze(){
		if (enemyEffect != ENEMYEFFECT.FREEZE)
			return;
		
		enemyEffect = ENEMYEFFECT.NONE;
		if (frozenObjFX)
			frozenObjFX.SetActive(false);
	}

	#endregion



	#region ICanBurn implementation

	public virtual void Burning (float damage, GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.BURNING)
			return;

		if (enemyEffect == ENEMYEFFECT.FREEZE) {
			UnFreeze ();
		}

		if (enemyEffect == ENEMYEFFECT.SHOKING) {
			UnShock ();
		}
		
		if (canBeBurn) {
			damageBurningPerFrame = damage;
			enemyEffect = ENEMYEFFECT.BURNING;

			StartCoroutine (BurnOutCo ());
		}
	}

	IEnumerator BurnOutCo(){
		if (enemyEffect != ENEMYEFFECT.BURNING)
			yield break;
		
		float wait = timeBurn - 1;
		yield return new WaitForSeconds(wait);

		if (enemyState == ENEMYSTATE.DEATH) {
			BurnOut ();
            Destroy(gameObject);
        }

		BurnOut ();
	}

	void BurnOut(){
		if (enemyEffect != ENEMYEFFECT.BURNING)
			return;
		
		enemyEffect = ENEMYEFFECT.NONE;
	}
    #endregion

    #region ICanShock implementation

    public virtual void Shoking (float damage, GameObject instigator)
	{
		if (enemyEffect == ENEMYEFFECT.SHOKING)
			return;
		
		if (enemyEffect == ENEMYEFFECT.FREEZE) {
			UnFreeze ();
		}

		if (enemyEffect == ENEMYEFFECT.BURNING)
			BurnOut ();

		if (canBeShock) {
			damageShockingPerFrame = damage;
			enemyEffect = ENEMYEFFECT.SHOKING;

			if (shakingHelper) {
				if (!shakingHelper.enabled)
					shakingHelper.enabled = true;
				
				shakingHelper.DoShake (true);;
			}

			StartCoroutine (UnShockCo ());
		}
	}

	IEnumerator UnShockCo(){
		if (enemyEffect != ENEMYEFFECT.SHOKING)
			yield break;

		yield return new WaitForSeconds (timeShocking);

		UnShock ();
	}

	void UnShock(){
		if (enemyEffect != ENEMYEFFECT.SHOKING)
			return;
		
		enemyEffect = ENEMYEFFECT.NONE;

		if (shakingHelper) {
			shakingHelper.StopShake ();;

			if (shakingHelper.enabled)
				shakingHelper.enabled = false;
		}
	}
	#endregion
}
