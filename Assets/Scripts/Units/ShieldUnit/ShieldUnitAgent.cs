using UnityEngine;
using System.Collections.Generic;

public class ShieldUnitAgent : UnitAgent
{
	ShieldUnitBeliefs shieldUnitBeliefs;
	public ShieldUnitAgent(BDI_Unit me) : base(me)
	{
		
	}
	
	public override void SetBeliefs ()
	{
		// different typed references to the same beliefs for convenience
		beliefs = new ShieldUnitBeliefs(me);
		unitBeliefs = beliefs as UnitBeliefs;
		shieldUnitBeliefs = beliefs as ShieldUnitBeliefs;
		
		unitBeliefs.NewEnemies += HandleBeliefsNewEnemies;
	}
}
