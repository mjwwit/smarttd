using UnityEngine;
using System.Collections.Generic;

public class UnitAgent : BDI_Agent
{
	public UnitBeliefs Beliefs;
	
	public UnitAgent(BDI_Unit me) : base()
	{
		Beliefs = new UnitBeliefs(me);
		Beliefs.NewEnemies += HandleBeliefsNewEnemies;
	}
	
	// ? todo: move this to beliefs, since we update what we believe to be the best path?
	void HandleBeliefsNewEnemies (object sender, List<Tower> newEnemies)
	{
		// todo: move hexmap to beliefs?
		foreach(Tower t in newEnemies)
			Beliefs.Me.map.ModifyCellsInRange<Tower>(t, Beliefs.Me.GridMod_AddTower, t);
		
		Beliefs.Me.path = null;
	}
	
	protected override Plan[] availablePlans ()
	{
		return new Plan[]
		{
			new Plan_FollowOptimalPath(this),
		};
	}
	
	public override void Update ()
	{
		Beliefs.Update();
		base.Update ();
	}
	
	public Vector3 GetPosition()
	{
		return Beliefs.Me.transform.position;
	}
	
	public void SetGoal(Vector3 position)
	{
		Beliefs.Me.GoalPosition = position;
	}
}
