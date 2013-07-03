using UnityEngine;
using System.Collections;

public class Plan_MoveToPosition : UnitPlan 
{
	public Vector3 GoalPosition;
	public float SuccessDistance;
	
	public Plan_MoveToPosition(UnitAgent agent, Vector3 position, float successDistance) : base(agent)
	{
		this.GoalPosition = position;
		this.SuccessDistance = successDistance;
	}
	
	public override bool SatisfiesPreCondition ()
	{
		return true;
	}
	public override bool SatisfiesInvocationCondition ()
	{
		return true;
	}
	public override bool SatisfiesTerminationCondition ()
	{
		return false;
	}
	
	public override bool SatisfiesSuccessCondition ()
	{
		float distanceToTarget = BasicObject.Distance(agent.GetPosition(), this.GoalPosition);
		
		return distanceToTarget < this.SuccessDistance;
	}
	
	public override float ContributionHeuristic ()
	{
		return 0;
	}
	
	public override void ExecuteStep ()
	{
		Vector3 totalForce = Vector3.zero;
		
		BDI_Unit me = this.agent.unitBeliefs.Me;
		
		// goal vector component
	 	totalForce += me.getGoalVector(me.transform.position, GoalPosition) * me.GoalForceStrength;
		
		// velocity
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
