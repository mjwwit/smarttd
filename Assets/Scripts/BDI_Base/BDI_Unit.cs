using UnityEngine;
using System;
using System.Collections.Generic;

// enumeration of all plan types used for selecting plans in the editor	
public enum Plan
{
	FlockAndFollowPath,
	FollowOptimalPath,
	FollowTankUnit,
	GetUnderShield,
	MoveToGoalPosition,
}

// class on top of Unit, enabling use of the BDI agent
public class BDI_Unit : Unit
{
	public UnitAgent Agent;
	public List<Plan> AvailablePlans = new List<Plan>()
	{
		Plan.FollowOptimalPath
	};
	
	protected override void Start ()
	{
		base.Start ();
		SetAgent();
		Agent.SetBeliefs();
	}
	
	protected virtual void SetAgent()
	{
		Agent = new UnitAgent(this);
	}
	
	protected override void FixedUpdate ()
	{
		// todo: replace this with base.FixedUpdate() when merging with Unit.cs
		updateBuffs(Time.fixedDeltaTime);
		
		// update agent
		Agent.Update();
		
		velocity = this.Agent.unitBeliefs.OptimalVelocity;
		
		// move unit
		transform.position += velocity * Time.fixedDeltaTime;
	}
}