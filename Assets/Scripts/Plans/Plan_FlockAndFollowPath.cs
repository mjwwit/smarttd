using UnityEngine;
using System.Collections;

// legacy plan
// this is the plan executing the behaviour of the units pre-BDI
public class Plan_FlockAndFollowPath : UnitPlan 
{	
	public Plan_FlockAndFollowPath(UnitAgent agent) : base(agent)
	{
		Type |= PlanType.Movement;
	}
	
	public override bool SatisfiesPreCondition ()
	{
		if(agent.unitBeliefs.TargetedBy > 0) return false;
		return true;
	}
	public override bool SatisfiesInvocationCondition ()
	{
		return agent.unitBeliefs.FriendsInRange.Count > 0;
	}
	public override bool SatisfiesTerminationCondition ()
	{
		if(agent.unitBeliefs.TargetedBy > 0) return true;
		return agent.unitBeliefs.FriendsInRange.Count <= 0;
	}
	public override bool SatisfiesSuccessCondition ()
	{
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
	}
	
	// heuristic: how many units will survive when we use this plan, including ourselves?
	public override float ContributionHeuristic ()
	{
		return agent.unitBeliefs.FriendsInRange.Count + 1;
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
		
		// apply flocking force
		totalForce += Unit.getFlockingVector(
				agent.unitBeliefs.Me,
				agent.unitBeliefs.FriendsInRange, me.RangeOfView,
				agent.unitBeliefs.FriendsInRange, me.SeparationDistance,
				agent.unitBeliefs.EnemiesInRange) 
			* me.FlockingStrength;
		
		// velocity
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
