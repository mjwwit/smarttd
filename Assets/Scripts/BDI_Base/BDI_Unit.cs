using UnityEngine;
using System;
using System.Collections.Generic;

public class BDI_Unit : Unit
{
	public UnitAgent Agent;
	
	public Vector3 GoalPosition;
	
	protected override void Start ()
	{
		base.Start ();
		GoalPosition = this.transform.position;
		SetAgent();
		Agent.SetBeliefs();
	}
	
	protected virtual void SetAgent()
	{
		Agent = new UnitAgent(this);
	}
	
	protected override void FixedUpdate ()
	{
		Vector3 cPos = transform.position;
		
		// update agent
		Agent.Update();
		
		// velocity
		velocity = MovementSpeed * (GoalPosition - cPos).normalized;
		
		// move unit
		transform.position += velocity * Time.fixedDeltaTime;
		
		//transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.fixedDeltaTime);
	}
}