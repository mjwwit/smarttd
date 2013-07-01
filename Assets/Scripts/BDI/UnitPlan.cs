using UnityEngine;
using System.Collections;

public abstract class UnitPlan : Plan
{
	protected UnitAgent agent;
	
	public UnitPlan(UnitAgent agent) : base()
	{
		this.agent = agent;
	}
}
