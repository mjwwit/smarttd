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
	
	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 totalForce;
	
	// influences how easily the object changes to new directions
	// lower mass makes for an easier change in velocity
	public float Mass = 1;
	
	protected override void Start ()
	{
		base.Start ();
		
		map = GetComponent<HexMap>();
		cPathNode = 0;
		
		knownDefenders = new List<BasicObject>();
		
		velocity = Vector3.zero;
		totalForce = Vector3.zero;
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
		
		handleNewData();
		
		
		// --------------------------------------------
		// basic path planning
		
		// get goal position
		// can be used as a hint for a forces based method
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
		
		Vector3 goalDesiredVelocity = diff * Mathf.Min(distance, MovementSpeed * Time.deltaTime);
		Vector3 goalForce = goalDesiredVelocity - velocity;
		
		
		totalForce = Vector3.zero;
		
		// goal vector component
	 	totalForce += goalForce * 0.05f;
		
		// apply flocking force
		totalForce += getFlockingVector();
		
		// velocity
		velocity += totalForce / Mass;
		
		// clamp to maximum speed
		velocity = Vector3.ClampMagnitude(velocity, MovementSpeed);
		
		// move unit
		transform.position += velocity;
		
		//transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.deltaTime);
	}
	
	#region Flocking
	
	public float SeparationDistance = 1.5f;
	public float SeparationStrength = 0.0003f;
	
	public float CohesionDistance = 5.0f;
	public float CohesionStrength = 0.0001f;
	
	public float AlignmentStrength;
	
	Vector3 getFlockingVector()
	{
		Vector3 v = Vector3.zero;
		foreach(BasicObject a in attackers)
		{				
			if(this == a) { continue; }
						
			float dist = DistanceTo(a);
			if(dist < CohesionDistance)
			{
				v += -CohesionStrength * (this.transform.position - a.transform.position).normalized;
				if(dist < SeparationDistance)
				{
					v += SeparationStrength * (this.transform.position - a.transform.position).normalized;
				}
			}
		}

		return v;
	}
	
	#endregion
	
	#region New Data Handling
	
	void handleNewData()
	{
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
	}
	
	/* --- Functions handling newly aqcuired information. ---
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
	
	#region Grid-based path planning
	
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
		
		Vector3 pos = this.transform.position;
		Vector3 goal = Attacker.ClosestPointOnGoal(pos);
		
		return map.GetOptimalPath(pos, goal);
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
		
		Gizmos.color = Color.green;
		for(int i = 0; i < path.Count - 1; ++i)
		{
			Gizmos.DrawLine(map.GetNodePosition(path[i]) + new Vector3(0, 0.01f, 0), map.GetNodePosition(path[i+1]) + new Vector3(0, 0.01f, 0));
		}
	}
	
	#endregion
}