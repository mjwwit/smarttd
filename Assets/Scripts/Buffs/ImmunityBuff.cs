using UnityEngine;
using System.Collections;

public class ImmunityBuff : Buff
{
	public ImmunityBuff(float duration) : base(duration)
	{
		
	}
	
	protected override void applyEffect ()
	{
		base.applyEffect ();
		
		subject.numImmunityBuffs++;
		subject.SetImmunity(true);
	}
	protected override void removeEffect ()
	{
		base.removeEffect ();
		
		subject.numImmunityBuffs--;
		
		if(subject.numImmunityBuffs == 0)
			subject.SetImmunity(false);
	}
}