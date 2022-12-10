using UnityEngine;
using System.Collections;

public class MonsterSimpleAI : EnemyAI {
	[Header("Owner Setup")]
	public Animator animator;
	public string hitEventName="n/a";
	public string deadEventName="n/a";

    public override void Start()
    {
        base.Start();
		if (animator == null)
			animator = GetComponent<Animator>();

	}

    protected override void HitEvent ()
	{
		base.HitEvent ();
		if (isDead)
			Dead ();

		if (animator != null && hitEventName.CompareTo ("n/a") != 0)
			animator.SetTrigger (hitEventName);
	}

	public override void Update ()
	{
		base.Update ();
		animator.SetBool("isFrozen", isFreezing);
	}

	protected override void Dead ()
	{
		base.Dead ();
		if (animator != null && deadEventName.CompareTo ("n/a") != 0)
			animator.SetTrigger (deadEventName);
	}

	protected override void OnRespawn ()
	{
		base.OnRespawn ();
		controller.HandlePhysic = true;
	}
}
