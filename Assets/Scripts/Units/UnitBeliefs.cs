using UnityEngine;
using System;
using System.Collections.Generic;

public class UnitBeliefs : BDI_Beliefs
{
	public BDI_Unit Me;
	public List<BDI_Unit> Friends;
	public List<BDI_Unit> FriendsInRange;
	public List<Tower> Enemies;
	public List<Tower> EnemiesInRange;
	public int TargetedBy = 0;
	
	public Vector3 OptimalVelocity;
	
	public UnitBeliefs(BDI_Unit me)
	{
		this.Me = me;
		
		Enemies = new List<Tower>();
		
		// at first the unit doesn't believe to have any friends at all.. aww :(
		Friends = new List<BDI_Unit>();
		
		// we're so lonely, we initially don't even care about moving
		OptimalVelocity = Vector3.zero;
	}
	
	public override void Update()
	{
		// all of type BDI_Unit in range
		FriendsInRange = BasicObject.FindAllInRange<BDI_Unit, BasicObject>( 
			Me.transform.position, Me.RangeOfView, Attacker.Objects );
		
		// remove itself
		FriendsInRange.Remove(Me);
		
		// add new friends to collection
		List<BDI_Unit> newFriends = new List<BDI_Unit>();
		foreach(BDI_Unit u in FriendsInRange)
		{
			if(!Friends.Contains(u))
			{
				Friends.Add(u);
				newFriends.Add(u);
			}
		}
		if(newFriends.Count > 0)
			OnNewFriends(newFriends);
		
		// all of type Tower in range
		EnemiesInRange = BasicObject.FindAllInRange<Tower, BasicObject>( 
			Me.transform.position, Me.RangeOfView, Defender.Objects );
		
		// add new enemies to collection
		// set by how many turrets we're targeted
		List<Tower> newEnemies = new List<Tower>();
		TargetedBy = 0;
		foreach(Tower t in EnemiesInRange)
		{
			if(!Enemies.Contains(t))
			{
				Enemies.Add(t);
				newEnemies.Add(t);
			}
			if(t.CurrentTarget == Me)
				TargetedBy++;
		}
		if(newEnemies.Count > 0)
			OnNewEnemies(newEnemies);
	}
	
	#region Events
	
	// -----------------------------------------------------------------------------
	// Events can be used to detect conditions without having to poll every time.
	
	// Here's one example dealing with firing events when new enemies are detected.
	public delegate void NewEnemiesEventHandler(object sender, List<Tower> newEnemies);
	public event NewEnemiesEventHandler NewEnemies;
	protected virtual void OnNewEnemies(List<Tower> newEnemies) 
	{
		if (NewEnemies != null)
			NewEnemies(this, newEnemies);
	}
	
	// new friend event (haha)
	public delegate void NewFriendsEventHandler(object sender, List<BDI_Unit> newFriends);
	public event NewFriendsEventHandler NewFriends;
	protected virtual void OnNewFriends(List<BDI_Unit> newFriends) 
	{
		if (NewFriends != null)
			NewFriends(this, newFriends);
	}
	
	#endregion
	
	public override string MyName ()
	{
		return Me.gameObject.name;
	}
}
