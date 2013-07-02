using UnityEngine;
using System;
using System.Collections.Generic;

public class UnitBeliefs : BDI_Beliefs
{
	public BDI_Unit Me;
	//public List<BDI_Unit> Friends;
	public List<BDI_Unit> FriendsInRange;
	public List<Tower> Enemies;
	public List<Tower> EnemiesInRange;
	
	public UnitBeliefs(BDI_Unit me)
	{
		this.Me = me;
		
		Enemies = new List<Tower>();
	}
	
	public override void Update()
	{
		// all of type BDI_Unit in range
		FriendsInRange = BasicObject.FindAllInRange<BDI_Unit, BasicObject>( 
			Me.transform.position, Me.RangeOfView, Attacker.Objects );
		
		// remove itself
		FriendsInRange.Remove(Me);
		
		// all of type Tower in range
		EnemiesInRange = BasicObject.FindAllInRange<Tower, BasicObject>( 
			Me.transform.position, Me.RangeOfView, Defender.Objects );
		
		// add new enemies to collection
		List<Tower> newEnemies = new List<Tower>();
		foreach(Tower t in EnemiesInRange)
		{
			if(!Enemies.Contains(t))
			{
				Enemies.Add(t);
				newEnemies.Add(t);
			}
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
	
	#endregion
}
