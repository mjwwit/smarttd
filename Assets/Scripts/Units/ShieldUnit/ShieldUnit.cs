using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldUnit : BDI_Unit
{
	public float ShieldRange = 2.0f;
	public int ShieldHP = 10;
	
	public ShieldUnit()
	{
		
	}
	
	protected override void SetAgent ()
	{
		Agent = new ShieldUnitAgent(this);
	}
	
	public override void TakeDamage(int damage)
	{
		Debug.Log(this + " is now taking damage!");
		if(ShieldHP > 0)
		{
			Blink(Color.blue);
			ShieldHP -= damage;
		}
		else
		{
			base.TakeDamage(damage);
		}
	}
	
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
}
