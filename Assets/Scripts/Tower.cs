using UnityEngine;
using System.Collections.Generic;

public class Tower : BasicObject
{
	public BasicObject CurrentTarget;
	
	public int Damage = 1;
	public float AttackCooldown = 2;
	
	float nextAttackTime = 0;
	
	protected override void Start ()
	{
		base.Start ();
	}
	protected override void Update ()
	{
		// basic target and fire mechanism
		if(CurrentTarget == null 
			|| DistanceTo(CurrentTarget) > RangeOfView
			|| !CurrentTarget.IsAlive)
		{
			AcquireTarget();
		}
		else
		{
			if(Time.time >= nextAttackTime)
			{
				nextAttackTime += AttackCooldown;
				
				Fire();
				
				if( !CurrentTarget.IsAlive )
				{
					AcquireTarget();
				}
			}
		}
		base.Update ();
	}
	
	void Fire()
	{
		// we might want to fire a projectile here or something in the future
		CurrentTarget.TakeDamage(Damage);	
	}
	
	void AcquireTarget()
	{
		// target the closest!
		CurrentTarget = FindClosestAttackerInRange(this);
		
		// reset attack timer on acquiring a new target
		nextAttackTime = Time.time + AttackCooldown;
	}
	
	public override void Die ()
	{
		base.Die ();
		Defender.RemoveObject(this);
	}
}