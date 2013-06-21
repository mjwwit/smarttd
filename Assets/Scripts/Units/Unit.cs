using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : BasicObject
{
	// temporary lists, cached every frame (we need it so often)
	List<BasicObject> attackers;
	List<BasicObject> defenders;
	
	// grid-based pathfinding variables
	List<Node> path;
	int cPathNode;
	
	public float PositionReachedMargin = .3f;
	
	protected override void Start ()
	{
		base.Start ();
		cPathNode = 0;
	}
	protected override void Update ()
	{
		base.Update ();
		
		// current position
		Vector3 cPos = transform.position;
		
		// store units that are in range
		attackers = FindAttackersInRange(this);
		defenders = FindDefendersInRange(this);
		
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
			diff = Attacker.ShortestVectorToGoal(cPos) - cPos;
			distance = diff.magnitude;
		}
		
		// normalize to direction
		diff /= distance > 0 ? distance : 1;
		
		// move unit towards its current goal
		transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.deltaTime);
	}
	
	Vector3 GetGridGoal(Vector3 currentPosition)
	{
		if(path == null || path.Count == 0) 
		{
			// get new path
			path = GetPath();
			cPathNode = 0;
			
			// if no path available
			if(path.Count == 0)
				return currentPosition;
		}
		if(cPathNode >= path.Count)
		{
			// -- GOAL REACHED --
			return currentPosition;
		}
		
		Node n = path[cPathNode];
		
		// if we're close to the node
		if(DistanceTo(n.Position) < PositionReachedMargin)
		{
			// go to next node
			cPathNode++;
			
			if(cPathNode >= path.Count)
			{
				// -- GOAL REACHED --
				return currentPosition;
			}
		}
		
		return path[cPathNode].Position;
	}
	
	public List<Node> GetPath()
	{
		return new List<Node>();
	}
	
	public override void Die ()
	{
		base.Die ();
		Attacker.RemoveObject(this);
	}
}