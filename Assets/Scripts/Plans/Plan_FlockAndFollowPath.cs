using UnityEngine;
using System.Collections;

// legacy plan
// this is the plan executing the behaviour of the units pre-BDI
public class Plan_FlockAndFollowPath : UnitPlan 
{	
	public Plan_FlockAndFollowPath(UnitAgent agent) : base(agent)
	{
		
	}
	
	public override bool SatisfiesPreCondition ()
	{
		return true;
	}
	public override bool SatisfiesInvocationCondition ()
	{
		return agent.unitBeliefs.FriendsInRange.Count > 0;
	}
	public override bool SatisfiesTerminationCondition ()
	{
		return agent.unitBeliefs.FriendsInRange.Count <= 0;
	}
	
	public override bool SatisfiesSuccessCondition ()
	{
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
	}
	
	public override void StartPlan ()
	{
		// reset any existing optimal path
		agent.me.path = null;
	}

	public override void ExecuteStep ()
	{
		Vector3 totalForce = Vector3.zero;
		
		BDI_Unit me = this.agent.unitBeliefs.Me;
		Vector3 cPos = me.transform.position;
		
		// goal vector component
	 	totalForce += me.getGoalVector(cPos) * me.GoalForceStrength;
		
		// apply flocking force
		totalForce += Unit.getFlockingVector(
				agent.unitBeliefs.Me, 
				agent.unitBeliefs.FriendsInRange, 
				agent.unitBeliefs.EnemiesInRange) 
			* me.FlockingStrength;
		
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		// velocity
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		//velocity /= velocity.magnitude * MovementSpeed;
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
