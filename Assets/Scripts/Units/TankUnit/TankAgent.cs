using UnityEngine;
using System.Collections.Generic;

public class TankUnitAgent : UnitAgent
{
	TankUnitBeliefs tankUnitBeliefs;
	public TankUnitAgent(BDI_Unit me) : base(me)
	{
		
	}
	
	public override void SetBeliefs ()
	{
		// different typed references to the same beliefs for convenience
		beliefs = new TankUnitBeliefs(me);
		unitBeliefs = beliefs as UnitBeliefs;
		tankUnitBeliefs = beliefs as TankUnitBeliefs;
		
		unitBeliefs.NewEnemies += HandleBeliefsNewEnemies;
	}
}
