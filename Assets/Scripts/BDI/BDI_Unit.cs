using UnityEngine;
using System;
using System.Collections.Generic;

public class BDI_Unit : Unit
{
	UnitAgent agent;
	
	public Vector3 GoalPosition;
	
	protected override void Start ()
	{
		base.Start ();
		agent = new UnitAgent(this);
		GoalPosition = this.transform.position;
	}
	
	protected override void FixedUpdate ()
	{
		Vector3 cPos = transform.position;
		
		// update agent
		agent.Update();
		
		// velocity
		velocity = MovementSpeed * (GoalPosition - cPos).normalized;
		
		// move unit
		transform.position += velocity * Time.fixedDeltaTime;
		
		//transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.fixedDeltaTime);
	}
}