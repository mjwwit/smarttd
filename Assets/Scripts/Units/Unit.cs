using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : BasicObject
{
	// temporary lists, cached every frame (we need it so often)
	List<BasicObject> attackers;
	List<BasicObject> defenders;
	
	List<BasicObject> knownDefenders;
	
	// grid-based pathfinding variables
	HexMap map;
	List<Node> path;
	int cPathNode;
	
	public float PositionReachedMargin = .3f;
	
	protected override void Start ()
	{
		base.Start ();
		
		map = GetComponent<HexMap>();
		cPathNode = 0;
		
		knownDefenders = new List<BasicObject>();
	}
	
	protected override void Update ()
	{
		base.Update ();
		
		// current position
		Vector3 cPos = transform.position;
		
		// store units that are in range
		attackers = FindAttackersInRange(this);
		defenders = FindDefendersInRange(this);
		
		
		// --------------------------------------------
		// handle newly aqcuired information
		
		bool resetPath = false;
		
		// for each defender in range
		foreach(BasicObject o in defenders)
		{
			// if it's already known, continue
			if(knownDefenders.Contains(o)) continue;
			
			// if it's not, handle the new information
			knownDefenders.Add(o);
			if(o.GetType() == typeof(Tower))
			{
				bool relevantData = handleNewData(o as Tower);
				if(relevantData) resetPath = true;
			}
		}
		
		// reset path if needed
		if(resetPath) path = null;
		
		
		// --------------------------------------------
		// basic path planning
		
		// get goal position
		Vector3 gridGoal = GetGridGoal(cPos);
		
		// get difference vector and distance
		Vector3 diff = gridGoal - cPos;
		float distance = diff.magnitude;
		
		// if we already reached this position
		if(distance < PositionReachedMargin)
		{
			// move towards ultimate goal
			// set new difference vector and distance
			diff = Attacker.VectorToGoal(cPos);
			distance = diff.magnitude;
		}
		
		// normalize to direction
		diff /= distance > 0 ? distance : 1;
		
		// move unit towards its current goal
		transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.deltaTime);
	}
	
	#region New Data Handling
	
	/* --- Handling of newly aqcuired information. ---
	 * These functions should return TRUE if the new data was relevant to this object (ie. for path planning).
	*/
	
	// handles new tower information
	bool handleNewData(Tower t)
	{
		map.ModifyCellsInRange<Tower>(t, GridMod_AddTower, t);
		return true;
	}
	
	// This function adds a tower's influence to the unit's hexmap data.
	// Here we can scale it by how much the unit is affected by this particular turret, etcetera.
	public int GridMod_AddTower(int cost, Tower t)
	{
		return cost += t.Damage * 10;
	}
	
	#endregion
	
	#region Grid-based pathfinding
	
	Vector3 GetGridGoal(Vector3 currentPosition)
	{
		// if we don´t have a path
		if(path == null || path.Count == 0)
		{
			// get new path
			path = GetPath();
			cPathNode = 0;
			
			// if still no path available, set goal to current position
			if(path == null || path.Count == 0)
				return currentPosition;
		}
		
		// if the next node doesn't exist
		// -- GOAL REACHED --
		// set goal to current position
		if(cPathNode >= path.Count)
		{
			return currentPosition;
		}
		
		Node n = path[cPathNode];
		
		// if we're close to the node
		if(DistanceTo(n.Position) < PositionReachedMargin)
		{
			// go to next node
			cPathNode++;
		
			// if the next node doesn't exist
			// -- GOAL REACHED --
			// set goal to current position
			if(cPathNode >= path.Count)
			{
				return currentPosition;
			}
		}
		
		// set goal to current target node's position
		return path[cPathNode].Position;
	}
	
	public List<Node> GetPath()
	{
		if(!map) return null;
		
		return map.GetOptimalPath(this, Attacker.ClosestPointOnGoal(this.transform.position));
	}
	
	#endregion
	
	public override void Die ()
	{
		base.Die ();
		Attacker.RemoveObject(this);
	}
	
	#region Visualization
	
	protected virtual void OnDrawGizmosSelected ()
	{
		base.OnDrawGizmos ();
		
		if(path == null || path.Count <= 0) return;
		
		Gizmos.color = Color.white;
		for(int i = 0; i < path.Count - 1; ++i)
		{
			Gizmos.DrawLine(map.GetNodePosition(path[i]), map.GetNodePosition(path[i+1]));
		}
	}
	
	#endregion
}