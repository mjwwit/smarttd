using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldUnit : BDI_Unit
{
	public float ShieldRange = 2.0f;
	public int ShieldHP = 10;
	
	private List<Unit> ShieldedUnits = new List<Unit>();
	
	public ShieldBuff UnitBuff = new ShieldBuff(0f);
	
	public ShieldUnit()
	{
		UnitBuff.SetShieldUnit(this);
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
			if(ShieldHP <= 0)
			{
				// Shield has been destroyed
				foreach(Unit u in ShieldedUnits)
				{
					u.RemoveBuff(UnitBuff);
				}
			}
		}
		else
		{
			base.TakeDamage(damage);
		}
	}
	
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		
		foreach(Unit u in Attacker.Units)
		{
			float distance = Vector3.Distance(this.transform.position, u.transform.position);
			if(distance <= ShieldRange)
			{
				// Unit is inside the shield
				if(!ShieldedUnits.Contains(u))
				{
					Debug.Log(u + " is now protected by the shield!");
					ShieldedUnits.Add(u);
					u.ApplyBuff(UnitBuff);
				}
					
			}
			else if(ShieldedUnits.Contains(u))
			{
				Debug.Log(u + " left the protection of the shield!");
				ShieldedUnits.Remove(u);
				u.RemoveBuff(UnitBuff);
			}
				
		}
	}
}
