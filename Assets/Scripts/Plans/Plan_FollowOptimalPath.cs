using UnityEngine;
using System.Collections.Generic;

public class Plan_FollowOptimalPath : UnitPlan 
{
	public Plan_FollowOptimalPath(UnitAgent agent) : base(agent)
	{
		Type |= PlanType.Movement;
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
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
	}
	
	// heuristic: how many units will survive when we use this plan, including ourselves?
	public override float ContributionHeuristic ()
	{
		return 1;
	}
	
	protected override void StartPlan ()
	{
		// reset any existing optimal path
		agent.me.path = null;
	}
	
	public override void ExecuteStep ()
	{
		Vector3 totalForce = Vector3.zero;
		
		BDI_Unit me = this.agent.unitBeliefs.Me;
		
		// goal vector component
	 	totalForce += me.getGoalVector(me.transform.position, me.GetGridGoal()) * me.GoalForceStrength;
		
		// velocity
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
