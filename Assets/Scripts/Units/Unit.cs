using UnityEngine;
using System;
using System.Collections.Generic;

public class Unit : BasicObject
{
	// temporary lists, cached every frame (we need it so often)
	List<BasicObject> attackers;
	List<BasicObject> defenders;
	
	List<BasicObject> knownDefenders;
	
	// todo: move hexmap to beliefs
	// grid-based pathfinding variables
	public HexMap map;
	public List<Node> path;
	int cPathNode;
	
	public float PositionReachedMargin = .5f;
	
	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 totalForce;
	
	// influences how easily the object changes to new directions
	// lower mass makes for an easier change in velocity
	public float Mass = 1;
	
	// force parameters
	public float GoalForceStrength = 3.0f;
	public float FlockingStrength = 2.0f;
	public float SeparationDistance = 1.5f;
	public float SeparationStrength = 3.0f;
	
	public float CohesionDistance = 5.0f;
	public float CohesionStrength = 1.0f;
	
	public float AlignmentStrength;
	
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

	}
	
	protected override void FixedUpdate ()
	{
		base.FixedUpdate();
		
		Vector3 cPos = transform.position;
		
		// store units that are in range
		attackers = FindAttackersInRange(this);
		defenders = FindDefendersInRange(this);
		
		
		// --------------------------------------------
		// handle newly aqcuired information
		
		handleNewData();
		
		
		// --------------------------------------------
		// basic path planning
		totalForce = Vector3.zero;
		
		// goal vector component
	 	totalForce += getGoalVector(cPos) * GoalForceStrength;
		
		// apply flocking force
		//totalForce += getFlockingVector(attackers, defenders, cPos) * FlockingStrength;
		
		// velocity
		velocity += totalForce / Mass * Time.fixedDeltaTime;
		
		// clamp to maximum speed
		//velocity /= velocity.magnitude * MovementSpeed;
		velocity = Vector3.ClampMagnitude(velocity, MovementSpeed);
		
		// move unit
		transform.position += velocity * Time.fixedDeltaTime;
		
		//transform.position += diff * Mathf.Min(distance, MovementSpeed * Time.fixedDeltaTime);
	}
	
	#region Flocking
	
	public static Vector3 getFlockingVector(BDI_Unit me, List<BDI_Unit> friends, List<Tower> enemies)
	{
		Vector3 cPos = me.transform.position;
		Vector3 v = Vector3.zero;
		foreach(BasicObject a in friends)
		{				
			//if(this == a) { continue; }
			
			Vector3 vAdd = Vector3.zero;
						
			float dist = BasicObject.Distance(me, a);
			Vector3 dir = (cPos - a.transform.position) / dist;
			
			// if we're not being targeted by an area effect, enable cohesion
			bool targetedByAoE = false;
			foreach(BasicObject b in enemies)
			{
				Tower_AoE t = b as Tower_AoE;
				if(t && t.CurrentTarget == me) 
					targetedByAoE = true;
			}
			if(!targetedByAoE && dist < me.CohesionDistance)
			{
				vAdd += -me.CohesionStrength * dir;
			}
			
			// for each object in range that's being targeted, add separation force
			foreach(BasicObject b in enemies)
			{
				Tower_AoE t = b as Tower_AoE;
				if(!t) continue;
				
				if(a == t.CurrentTarget && dist < t.AreaOfEffectRadius + 0.2f)
					vAdd += me.SeparationStrength * dir;
			}
			
			// standard separation force
			if(dist < me.SeparationDistance)
			{
				vAdd += me.SeparationStrength * dir;
			}
			
			//float baseInfluence = 0.5f;
			//float directionInfluence = 0.5f;
			//float influence = baseInfluence + directionInfluence * (1 + -Vector3.Dot(velocity.normalized, dir))*.5f;
			//vAdd *= influence;
			
			v+= vAdd;
		}
		
		return v;
	}
	
	#endregion
	
	// todo: move this to belief updating
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
			if(o as Tower)
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
		return cost += Mathf.RoundToInt(t.Damage / t.AttackCooldown * 10);
	}
	
	#endregion
	
	// todo: move this part to beliefs
	#region Grid-based path planning
	
	public Vector3 getGoalVector(Vector3 cPos)
	{
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
		
		// maybe GoalForceStrength should be multiplied with desired velocity instead of goalForce ??
		Vector3 goalDesiredVelocity = diff * MovementSpeed;
		Vector3 goalForce = goalDesiredVelocity - velocity;
		
		return goalForce;
	}
	
	public Vector3 GetGridGoal()
	{
		return GetGridGoal(this.transform.position);
	}
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
		
		// if the next node is closer
		if(path.Count > cPathNode+1 
			&& Distance(this, path[cPathNode].Position) > Distance(this, path[cPathNode+1].Position))
		{
			cPathNode++;
			n = path[cPathNode];
		}
		
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