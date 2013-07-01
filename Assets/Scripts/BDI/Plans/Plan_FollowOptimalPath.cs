using UnityEngine;
using System.Collections.Generic;

public class Plan_FollowOptimalPath : UnitPlan 
{
	public Plan_FollowOptimalPath(UnitAgent agent) : base(agent)
	{
		
	}
	
	public override bool SatisfiesPreCondition ()
	{
		return true;
	}
	public override bool SatisfiesInvocationCondition ()
	{
		if(agent.Beliefs.FriendsInRange.Count <= 0)
			return true;
		return false;
	}
	public override bool SatisfiesTerminationCondition ()
	{
		return agent.Beliefs.FriendsInRange.Count > 0;
		//return false;
	}
	public override bool SatisfiesSuccessCondition ()
	{
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
		//return true;
	}
	
	public override void ExecuteStep ()
	{
		Vector3 nextPos = agent.Beliefs.Me.GetGridGoal();
		agent.SetGoal(nextPos);
		
		//stack.Push(this);
		//stack.Push(new Plan_Move(agent, nextPos, 0.5f));
	}
}
