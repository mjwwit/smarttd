using UnityEngine;
using System.Collections;

public class ShieldBuff : Buff
{
	private ShieldUnit Shield;
	
	public ShieldBuff (float duration) : base(duration)
	{
		
	}
	
	public void SetShieldUnit(ShieldUnit sUnit)
	{
		Shield = sUnit;
	}
	
	public ShieldUnit GetShieldUnit()
	{
		return Shield;
	}
	
	protected override void applyEffect ()
	{
		base.applyEffect ();
		
		subject.numImmunityBuffs++;
		subject.SetImmunity(true);
	}
	
	protected override void removeEffect ()
	{
		Shield = null;
		base.removeEffect ();
		
		subject.numImmunityBuffs--;
		
		if(subject.numImmunityBuffs == 0)
			subject.SetImmunity(false);
	}
}

