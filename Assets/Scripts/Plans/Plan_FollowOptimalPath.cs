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
		return agent.unitBeliefs.FriendsInRange.Count <= 0;
	}
	public override bool SatisfiesTerminationCondition ()
	{
		return agent.unitBeliefs.FriendsInRange.Count > 0;
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
		Vector3 nextPos = agent.unitBeliefs.Me.GetGridGoal();
		agent.SetGoal(nextPos);
		
		//stack.Push(this);
		//stack.Push(new Plan_Move(agent, nextPos, 0.5f));
	}
}
