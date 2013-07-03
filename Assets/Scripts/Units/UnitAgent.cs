using UnityEngine;
using System.Collections.Generic;

public class UnitAgent : BDI_Agent
{
	public UnitBeliefs unitBeliefs;
	public BDI_Unit me;
	public UnitAgent(BDI_Unit me) : base()
	{
		this.me = me;
	}
	
	public override void SetBeliefs ()
	{
		// different typed references to the same beliefs for convenience
		beliefs = new UnitBeliefs(me);
		unitBeliefs = beliefs as UnitBeliefs;
		
		unitBeliefs.NewEnemies += HandleBeliefsNewEnemies;
	}
	
	// todo: move hexmap to beliefs?
	// todo: move this to beliefs as well, since we update what we believe to be the best path?
	// on the other hand, it is the agent that updates his beliefs.. :P
	protected virtual void HandleBeliefsNewEnemies (object sender, List<Tower> newEnemies)
	{
		foreach(Tower t in newEnemies)
			unitBeliefs.Me.map.ModifyCellsInRange<Tower>(t, unitBeliefs.Me.GridMod_AddTower, t);
		
		unitBeliefs.Me.path = null;
	}
	
	protected override BDI_Plan[] GetAvailablePlans ()
	{
		return new BDI_Plan[]
		{
			new Plan_FlockAndFollowPath(this),
			new Plan_FollowOptimalPath(this),
		};
	}
	
	protected override bool GoalsCompleted ()
	{
		return Attacker.DistanceToGoal(GetPosition()) < unitBeliefs.Me.PositionReachedMargin;
	}
	
	public override void StopAgent ()
	{
		base.StopAgent ();
		
		unitBeliefs.OptimalVelocity = Vector3.zero;
	}
	
	public Vector3 GetPosition()
	{
		return unitBeliefs.Me.transform.position;
	}
}
