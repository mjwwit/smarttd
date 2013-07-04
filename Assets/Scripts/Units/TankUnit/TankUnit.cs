using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankUnit : BDI_Unit
{
	public TankUnit()
	{
		
	}
	
	protected override void SetAgent ()
	{
		Agent = new TankUnitAgent(this);
	}
	
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
}
