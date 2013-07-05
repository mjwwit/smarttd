using UnityEngine;
using System.Collections;

public class Plan_MoveToGoalPosition : UnitPlan 
{
	public Vector3 GoalPosition;
	
	public Plan_MoveToGoalPosition(UnitAgent agent) : base(agent)
	{
		
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
		
		return distanceToTarget < agent.me.PositionReachedMargin;
	}
	
	public override float ContributionHeuristic ()
	{
		return 0;
	}
	
	protected override void StartPlan ()
	{
		this.GoalPosition = Attacker.ClosestPointOnGoal(agent.GetPosition());
	}
	
	public override void ExecuteStep ()
	{
		Vector3 totalForce = Vector3.zero;
		
		BDI_Unit me = this.agent.unitBeliefs.Me;
		
		Vector3 myPos = me.transform.position;
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		
		Vector3 diff = Attacker.VectorToGoal(myPos);
		float distance = diff.magnitude;
		
		// normalize to direction
		diff /= distance > 0 ? distance : 1;
		
		Vector3 goalDesiredVelocity = diff * me.MovementSpeed;
		Vector3 goalForce = goalDesiredVelocity - velocity;
		
		// goal vector component
	 	totalForce += goalForce * me.GoalForceStrength;
		
		// velocity
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
