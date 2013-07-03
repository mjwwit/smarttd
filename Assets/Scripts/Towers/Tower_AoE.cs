using UnityEngine;
using System.Collections.Generic;

public class Tower_AoE : Tower
{
	// this should be a projectile property I guess :p
	public float AreaOfEffectRadius = 2;
	
	protected override void Fire ()
	{
		// we might want to fire a projectile here or something in the future
		// and move this code to the projectile on impact
		
		foreach(ShieldUnit su in Attacker.ShieldUnits)
		{
			if(IsInRange(su.transform.position, su.ShieldRange, CurrentTarget) >= 0)
			{
				su.TakeDamage(Damage);
				return;
			}
		}
		
		// find all attackers in range of our AoE at the current target's position
		List<BasicObject> targets = FindAllInRange<BasicObject>(
				CurrentTarget.transform.position, AreaOfEffectRadius, Attacker.Objects);
		
		for(int i = 0; i < targets.Count; i++)
			targets[i].TakeDamage(this.Damage);
	}
}