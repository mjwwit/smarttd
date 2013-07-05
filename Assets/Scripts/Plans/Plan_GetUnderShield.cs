using UnityEngine;
using System.Collections.Generic;

public class Plan_GetUnderShield : UnitPlan 
{
	ShieldUnit shield;
	public Plan_GetUnderShield(UnitAgent agent) : base(agent)
	{
		Type |= PlanType.Movement;
		shield = null;
	}
	
	public override bool SatisfiesPreCondition ()//Not targeted currently
	{
		if(agent.unitBeliefs.TargetedBy > 0) return false;
		return true;
	}
	public override bool SatisfiesInvocationCondition ()//shield in range
	{
		for(int i=0;i<agent.unitBeliefs.FriendsInRange.Count;i++) 
		{
			shield = agent.unitBeliefs.FriendsInRange[i] as ShieldUnit;
			if (shield)
			{
				return shield.DistanceTo(agent.me)>3;
			}
		}		
		return false;
	}
	// Terminate when:
	// - targeted;
	// - shield dies;
	// - shield is out of view range.
	public override bool SatisfiesTerminationCondition ()
	{
		if(shield.DistanceTo(agent.me)<3) return true;
		return !shield || !(shield.IsAlive) 
			|| agent.unitBeliefs.Me.IsInRange(shield) < 0;
	}
	public override bool SatisfiesSuccessCondition ()
	{
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
	}
	
	// heuristic: how many units will survive when we use this plan, including ourselves?
	public override float ContributionHeuristic ()
	{
		return 100;//for testing
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
	 	//totalForce += me.getGoalVector(me.transform.position, me.GetGridGoal()) * me.GoalForceStrength;
		
		List<BDI_Unit> shieldcontainer= new List<BDI_Unit>();
		shieldcontainer.Add (shield);
		totalForce += Unit.getFlockingVector(
				agent.unitBeliefs.Me,
				shieldcontainer, me.RangeOfView,
				agent.unitBeliefs.FriendsInRange, me.SeparationDistance,
				agent.unitBeliefs.EnemiesInRange) 
			* me.FlockingStrength;		
		
		// velocity
		//Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		// we don't have to wait for the shield, since we're always chasing after the shield anyway
		// waiting could even get us killed, we better just move to him a.s.a.p.!
		
		//if(Attacker.DistanceToGoal(me.Agent.GetPosition())>Attacker.DistanceToGoal(shield.Agent.GetPosition()))
			//velocity=Vector3.zero;
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
