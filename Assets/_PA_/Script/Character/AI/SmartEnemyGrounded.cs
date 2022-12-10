using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class SmartEnemyGrounded : Enemy {
	public bool isSocking{ get; set; }
	[HideInInspector]
	private Vector2 _direction;
	bool allowCheckAttack = true;
    
    public override void Start()
    {
        base.Start();

        controller = GetComponent<Controller2D>();

        _direction = isFacingRight() ? Vector2.right : Vector2.left;

        isPlaying = true;
        isSocking = false;

        controller.collisions.faceDir = 1;

        rangeAttack = GetComponent<EnemyRangeAttack>();
        meleeAttack = GetComponent<EnemyMeleeAttack>();
        throwAttack = GetComponent<EnemyThrowAttack>();
        bomberAttack = GetComponent<EnemyBomberAttack>();
        callMinion = GetComponent<EnemyCallMinion>();
        
        if (meleeAttack && meleeAttack.MeleeObj)
            meleeAttack.MeleeObj.SetActive(attackType == ATTACKTYPE.MELEE);
    }
    
	public override void Update ()
	{
		base.Update ();
		HandleAnimation ();
        
		if (!isPlaying || isSocking || !GameManager.Instance.Player.isPlaying || enemyEffect== ENEMYEFFECT.SHOKING) {
			velocity.x = 0;
			return;
		}

		if (!isPlayerDetected) {
			if (checkTarget.CheckTarget (isFacingRight () ? 1 : -1))
				DetectPlayer (delayAttackWhenDetectPlayer);
		} else {
			if (Vector2.Distance (transform.position, GameManager.Instance.Player.transform.position) > dismissPlayerDistance)
				DismissDetectPlayer ();
		}
		
		if ((isPlayerDetected && (enemyState == ENEMYSTATE.WALK)) || allowLookAtPlayer) {
            if ((isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x) || (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x))
            {
               Flip();
            }

			if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
			    || (!canBeFallDown && !controller.isGrounedAhead(isFacingRight()) && controller.collisions.below)) {
				isWaiting = true;
			} else
				isWaiting = false;
		} else {
			if (isWaiting) {
				waitingTime += Time.deltaTime;
				if (waitingTime >= waitingTurn && !isPlayerDetected) {
					isWaiting = false;
					waitingTime = 0;
					Flip ();
				}
			} else {
				if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)
				   || (!canBeFallDown && !controller.isGrounedAhead(isFacingRight()) && controller.collisions.below) || (usePatrol && !isPlayerDetected && ((!isFacingRight () && transform.position.x <= _patrolLimitLeft) || (isFacingRight () && transform.position.x > _patrolLimitRight)))) {
					isWaiting = true;
				}
			}
		}
	}

    public virtual void LateUpdate() {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING) {
            if (isDead && dieBehavior == DIEBEHAVIOR.FALLOUT)
            {
                velocity.y += -35 * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime, false);
            }
            else
            {
                velocity = Vector2.zero;
            }
            return;
        }

        if (!GameManager.Instance.Player.isPlaying)
            return;

        float targetVelocityX = _direction.x * moveSpeed;
        if (onlyMoveWhenGrounded && !controller.collisions.below)
            targetVelocityX = 0;

        velocity.x = isWaiting ? 0 : Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);

        velocity.y += -gravity * Time.deltaTime;

        if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE || (isPlayerDetected && (_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left)))
            velocity.x = 0;

        if (isPlayerDetected && (enemyType == ENEMYTYPE.INWATER || enemyType == ENEMYTYPE.FLY))
        {
            if (enemyState == ENEMYSTATE.WALK)
            {
                Vector2 targetPoint = (Vector2)GameManager.Instance.Player.transform.position + Vector2.right * (Mathf.Abs(chasingOffset.x)) * (isFacingRight() ? -1f : 1f) + Vector2.up * chasingOffset.y;
                
                velocity = (targetPoint - (Vector2)transform.position).normalized * moveSpeed;
                allowCheckAttack = Mathf.Abs(transform.position.x - targetPoint.x) < 0.3f;

                if (enemyType == ENEMYTYPE.INWATER)
                {
                    if (transform.position.y > waterLimitUp && velocity.y > 0)
                        velocity.y = 0;
                }
            }
            else if (enemyState == ENEMYSTATE.RUSHINTOPLAYER)
                velocity = rushIntoPlayerDirection * rushIntoPlayerSpeed;
            else
                velocity = Vector2.zero;
        }

        if (isStopping)
            velocity = Vector2.zero;

        controller.Move(velocity * Time.deltaTime, false);

        if (isPlayerDetected && velocity.x == 0 && enemyState == ENEMYSTATE.IDLE)
        {
            countingStanding += Time.deltaTime;
            if (isPlayerDetected && countingStanding >= dismissPlayerWhenStandSecond)
                DismissDetectPlayer();
        }
        else
            countingStanding = 0;


        if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

        

		if (controller.collisions.below)
			Parachute (false);
        
		if (isPlayerDetected && allowCheckAttack) {
			CheckAttack ();
		}
        else if (attackType == ATTACKTYPE.THROW && throwAttack.throwAction == EnemyThrowAttack.ThrowAction.ThrowAuto)
        {
            if (throwAttack.AllowAction())
            {
                SetEnemyState(ENEMYSTATE.ATTACK);

                if (throwAttack.CheckPlayer())
                {
                    throwAttack.Action();
                    AnimSetTrigger("throw");
                }
                else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                {
                    SetEnemyState(ENEMYSTATE.WALK);
                }
            }
        }
    }

	void Flip(){
		_direction = -_direction;
		transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.x, isFacingRight () ? 180 : 0, transform.rotation.z));
	}

    public override void DetectPlayer(float delayChase = 0)
    {
        isWaiting = false;
        base.DetectPlayer(delayChase);
        switch (detectPlayerAct)
        {
            case DETECTPLAYER.WalkAndCheckAttack:
                moveSpeed = walkSpeed;
                break;
            case DETECTPLAYER.RunAndCheckAttack:
                moveSpeed = runSpeed;
                break;
            case DETECTPLAYER.RushIntoPlayer:
                allowCheckAttack = false;
                break;
            default:

                break;
        }
    }
	public override void DismissDetectPlayer ()
	{
		base.DismissDetectPlayer ();
		switch (dismissDetectPlayerAct) {
		case DISMISSDETECTPLAYER.WalkAndPatrol:
			isPlayerDetected = false;
			moveSpeed = walkSpeed;
			break;
		case DISMISSDETECTPLAYER.RunAndPatrol:
			moveSpeed = runSpeed;
			break;
		case DISMISSDETECTPLAYER.Stand:
			moveSpeed = 0;
			break;
		default:
			break;
		}
	}

	//
	public void CallMinion(){
		AnimSetTrigger ("callMinion");
		SetEnemyState (ENEMYSTATE.ATTACK);
		allowCheckAttack = false;
	}

    void CheckAttack()
    {
        //CHECK AND CALL MINION IF THIS ENEMY HAS SCRIPT CALLMINION
        switch (attackType)
        {
            case ATTACKTYPE.RANGE:
                if (rangeAttack.AllowAction())
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);

                    AnimSetTrigger("shoot");
                    DetectPlayer();
                }
                break;
            case ATTACKTYPE.MELEE:
                if (meleeAttack.AllowAction())
                {
                    if (meleeAttack.CheckPlayer(isFacingRight()))
                    {
                        SetEnemyState(ENEMYSTATE.ATTACK);
                        meleeAttack.Action();
                        AnimSetTrigger("melee");
                    }
                    else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }
                break;

            case ATTACKTYPE.THROW:
                if (throwAttack.AllowAction())
                {
                    SetEnemyState(ENEMYSTATE.ATTACK);

                    if (throwAttack.CheckPlayer())
                    {
                        if (isFacingRight() && transform.position.x > GameManager.Instance.Player.transform.position.x)
                            Flip();
                        if (!isFacingRight() && transform.position.x < GameManager.Instance.Player.transform.position.x)
                            Flip();

                        throwAttack.Action();
                        AnimSetTrigger("throw");
                    }
                    else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
                    {
                        SetEnemyState(ENEMYSTATE.WALK);
                    }
                }
                break;
            case ATTACKTYPE.CALLMINION:
                if (callMinion && callMinion.CanCallMinion())
                {
                    CallMinion();
                }
                break;
            case ATTACKTYPE.BOMBER:
                bomberAttack.Attack();
                break;
            default:
                break;
        }
    }

	void AllowCheckAttack(){
		allowCheckAttack = true;
	}

	void HandleAnimation(){
		AnimSetFloat ("speed", Mathf.Abs (velocity.x));
		AnimSetBool ("isRunning", Mathf.Abs (velocity.x) > walkSpeed);
        AnimSetBool("isFrozen", enemyEffect == ENEMYEFFECT.FREEZE);
	}

	public void SetForce(float x, float y){
        velocity = new Vector3 (x, y, 0);
    } 

	public void AnimMeleeAttackStart(){
		meleeAttack.Check4Hit ();
	}

	public void AnimMeleeAttackEnd(){
		meleeAttack.EndCheck4Hit ();
	}

	public void AnimThrow(){
		throwAttack.Throw (isFacingRight());
	}

	public void AnimShoot(){
		rangeAttack.Shoot (isFacingRight ());
	}

	public void AnimCallMinion(){
		callMinion.CallMinion (isFacingRight ());
		Invoke ("AllowCheckAttack", 2);
	}

	public override void Die ()
	{
		if (isDead)
			return;

		base.Die ();


		CancelInvoke ();

		var cols= GetComponents<BoxCollider2D>();
		foreach (var col in cols)
			col.enabled = false;

		AnimSetTrigger ("die");

        if (enemyEffect == ENEMYEFFECT.BURNING)
			return;

		StopAllCoroutines ();

		StartCoroutine (DisableEnemy (2));
	}

	public override void Hit ()
	{
		if (!isPlaying)
			return;

		base.Hit ();
		if (isDead)
			return;

		AnimSetTrigger ("hit");
	}

	public override void KnockBack (Vector2 force)
	{
		base.KnockBack (force);
        SetForce(force.x, force.y);
	}

	IEnumerator DisableEnemy(float delay){
		yield return new WaitForSeconds (delay);
        Destroy(gameObject);
	}

    void OnDrawGizmosSelected()
    {
        if (startAction != STARTACTION.STAND && walkSpeed>0 && usePatrol)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position - Vector3.right * patrolLimitLeft, 0.3f);
            Gizmos.DrawWireSphere(transform.position + Vector3.right * patrolLimitRight, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * patrolLimitRight);
            Gizmos.DrawLine(transform.position, transform.position - Vector3.right * patrolLimitLeft);
        }

        if (!Application.isPlaying)
        {
            if (enemyType == ENEMYTYPE.FLY)
                gravity = 0;

            if (startAction == STARTACTION.STAND)
            {
                walkSpeed = 0;
            }
        }
    }
}
