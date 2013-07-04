using UnityEngine;
using System.Collections.Generic;

public class Plan_FollowTankUnit : UnitPlan 
{
	TankUnit tank;
	public Plan_FollowTankUnit(UnitAgent agent) : base(agent)
	{
		Type |= PlanType.Movement;
		tank = null;
	}
	
	public override bool SatisfiesPreCondition ()//Not targeted currently
	{
		if(agent.unitBeliefs.TargetedBy > 0) return false;
		return true;
	}
	public override bool SatisfiesInvocationCondition ()//Tank in range
	{
		for(int i=0;i<agent.unitBeliefs.FriendsInRange.Count;i++) 
		{
			tank = agent.unitBeliefs.FriendsInRange[i] as TankUnit;
			if (tank)
			{
				return true;
			}
		}		
		return false;
	}
	// Terminate when:
	// - targeted;
	// - tank dies;
	// - tank is out of view range.
	public override bool SatisfiesTerminationCondition ()
	{
		if(agent.unitBeliefs.TargetedBy > 0) return true;
		return !tank || !(tank.IsAlive) 
			|| agent.unitBeliefs.Me.IsInRange(tank) < 0;
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
		
		List<BDI_Unit> tankcontainer= new List<BDI_Unit>();
		tankcontainer.Add (tank);
		totalForce += Unit.getFlockingVector(
				agent.unitBeliefs.Me,
				tankcontainer, me.RangeOfView,
				agent.unitBeliefs.FriendsInRange, me.SeparationDistance,
				agent.unitBeliefs.EnemiesInRange) 
			* me.FlockingStrength;		
		
		// velocity
		//Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		Vector3 velocity = agent.unitBeliefs.OptimalVelocity;
		velocity += totalForce / me.Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, me.MovementSpeed);
		
		// we don't have to wait for the tank, since we're always chasing after the tank anyway
		// waiting could even get us killed, we better just move to him a.s.a.p.!
		
		//if(Attacker.DistanceToGoal(me.Agent.GetPosition())>Attacker.DistanceToGoal(tank.Agent.GetPosition()))
			//velocity=Vector3.zero;
		
		agent.unitBeliefs.OptimalVelocity = velocity;
	}
}
