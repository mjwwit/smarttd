using UnityEngine;
using System;
using System.Collections.Generic;

public class ShieldUnitBeliefs : UnitBeliefs
{
	// for now, it has the same belief base as a normal unit 
	// (except of course the details about himself)
	public ShieldUnitBeliefs(BDI_Unit me) : base(me)
	{
		
	}
}
