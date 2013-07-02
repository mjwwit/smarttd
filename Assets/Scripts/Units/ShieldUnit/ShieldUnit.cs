using UnityEngine;
using System.Collections;

public class ShieldUnit : BDI_Unit
{
	protected override void SetAgent ()
	{
		Agent = new ShieldUnitAgent(this);
	}
}
