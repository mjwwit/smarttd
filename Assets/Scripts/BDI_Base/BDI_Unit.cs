using UnityEngine;
using System;
using System.Collections.Generic;

public class BDI_Unit : Unit
{
	public UnitAgent Agent;
	
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
		
		Vector3 cPos = transform.position;
		
		// update agent
		Agent.Update();
		
		velocity = this.Agent.unitBeliefs.OptimalVelocity;
		
		// velocity
		//velocity = MovementSpeed * (GoalPosition - cPos).normalized;
		
		// move unit
		transform.position += velocity * Time.fixedDeltaTime;
		
		//transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.fixedDeltaTime);
	}
}