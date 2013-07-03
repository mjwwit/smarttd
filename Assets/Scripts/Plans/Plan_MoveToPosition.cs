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

	
	public override void ExecuteStep ()
	{
		agent.SetGoal(GoalPosition);
	}
}
