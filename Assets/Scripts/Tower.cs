using UnityEngine;
using System.Collections.Generic;

public class Tower : BasicObject
{
	public BasicObject CurrentTarget;
	
	public int Damage = 1;
	public float AttackCooldown = 2;
	
	float nextAttackCooldown = 0;
	
	Transform gun;
	
	protected override void Start ()
	{
		base.Start ();
		
		gun = transform.FindChild("GunPosition");
	}
	protected override void Update ()
	{
		base.Update ();
		
			
		// targeting visualization
		float cdRatio = nextAttackCooldown / AttackCooldown;
		
		Color lineColor = new Color(
			cdRatio > .5f ? 2-cdRatio*2 : 1, 
			cdRatio > .5f ? 1 : cdRatio*2, 
			0
		);
		
		if(! (CurrentTarget == null 
			|| DistanceTo(CurrentTarget) > RangeOfView
			|| !CurrentTarget.IsAlive) )
			Visualizer.DrawLine(gun.position, CurrentTarget.transform.position, lineColor);
		
		drawRange ();
	}
	
	protected override void FixedUpdate ()
	{
		// basic target and fire mechanism
		if(CurrentTarget == null 
			|| DistanceTo(CurrentTarget) > RangeOfView
			|| !CurrentTarget.IsAlive)
		{
			CurrentTarget = null;
			AcquireTarget();
		}
		else
		{
			nextAttackCooldown -= Time.fixedDeltaTime;
			
			// cooldown & fire
			if(nextAttackCooldown <= 0)
			{
				nextAttackCooldown = AttackCooldown;
				
				Fire();
			}
		}
	}
	
	protected virtual void Fire()
	{
		// we might want to fire a projectile here or something in the future
		// and move this code to the projectile on impact
		CurrentTarget.TakeDamage(Damage);
	}
	
	protected virtual void AcquireTarget()
	{
		// target the unit closest to the goal
		CurrentTarget = FindBestSuitedObjectInRange<Unit>(
			this, 
			Attacker.Units,
			Criterion_DistanceToGoal
		);
		
		// reset attack timer on acquiring a new target
		nextAttackCooldown = AttackCooldown;
	}
	
	public override void Die ()
	{
		base.Die ();
		Defender.RemoveObject(this);
	}
}