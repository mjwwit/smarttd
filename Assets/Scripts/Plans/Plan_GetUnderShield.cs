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
				return true;
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
		return !shield || !(shield.IsAlive) 
			|| agent.unitBeliefs.Me.IsInRange(shield) < 0;
	}
	public override bool SatisfiesSuccessCondition ()
	{
		return Attacker.DistanceToGoal(agent.GetPosition()) < 0.2f;
	}
	
	// heuristic: how many units will survive when we use this plan, including ourselves?
	// it's a pretty bad heuristic, we need something better 
	// something like an estimation of:
	// 		the total cost of all nodes we pass through divided by the chance we'll be damaged
	public override float ContributionHeuristic ()
	{
		return 101;//for testing
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
		
		//if we are far, we move towards shield, if we are close we follow optimal path.
		if(me.DistanceTo(shield)>shield.ShieldRange)
		{
			List<BDI_Unit> shieldcontainer= new List<BDI_Unit>();
			shieldcontainer.Add (shield);
			totalForce += Unit.getFlockingVector(
				agent.unitBeliefs.Me,
				shieldcontainer, me.RangeOfView,
				agent.unitBeliefs.FriendsInRange, me.SeparationDistance,
				agent.unitBeliefs.EnemiesInRange) 
			* me.FlockingStrength;		
		}
		else
		{
			totalForce += me.getGoalVector(me.transform.position, me.GetGridGoal()) * me.GoalForceStrength;
		}
		
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
