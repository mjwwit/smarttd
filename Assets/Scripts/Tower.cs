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
			nextAttackCooldown -= Time.deltaTime;
			
			// cooldown & fire
			if(nextAttackCooldown <= 0)
			{
				nextAttackCooldown = AttackCooldown;
				
				Fire();
			}
			
			// targeting visualization
			float cdRatio = nextAttackCooldown / AttackCooldown;
			
			Color lineColor = new Color(
				cdRatio > .5f ? 2-cdRatio*2 : 1, 
				cdRatio > .5f ? 1 : cdRatio*2, 
				0
			);
			
			Visualizer.DrawLine(gun.position, CurrentTarget.transform.position, lineColor);
		}
		base.Update ();
		
		drawRange ();
	}
	
	void Fire()
	{
		// we might want to fire a projectile here or something in the future
		CurrentTarget.TakeDamage(Damage);	
	}
	
	void AcquireTarget()
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