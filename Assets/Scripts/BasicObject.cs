using UnityEngine;
using System.Collections.Generic;

// Our basic object, implents basic variables and functionality every object could use.
public class BasicObject : MonoBehaviour
{
	#region Object Properties
	
	// how far the object can see
	public float RangeOfView = 5;
	
	// how large the object is
	// -> should we take this into account when deciding whether targets are in range or not?
	// -> for both or only the target?
	public float Radius = 0.5f;
	
	public int HP = 1;
	
	public bool IsAlive = true;
	
	#endregion
	
	
	// Use this for initialization
	protected virtual void Start ()
	{
	
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if(!IsAlive) Destroy(this.gameObject);
	}
	
	public virtual void TakeDamage(int damage)
	{
		HP -= damage;
		if(HP <= 0)
			Die();
	}
	public virtual void Die()
	{
		IsAlive = false;
	}
	
	#region Helper Functions
	
	public float DistanceTo(BasicObject toObj)
	{
		return Distance(this, toObj);
	}
	
	// maybe a 2d distance function is better
	public static float Distance(BasicObject fromObj, BasicObject toObj)
	{
		return Vector3.Distance(fromObj.transform.position, toObj.transform.position);
	}
	
	// Returns the distance if in range, -1 otherwise.
	public float IsInRange(BasicObject toObj)
	{
		return IsInRange(this, toObj);
	}
	
	// Returns the distance if in range, -1 otherwise.
	public static float IsInRange(BasicObject fromObj, BasicObject toObj)
	{
		// determine distance between objects
		float distance = Distance(fromObj, toObj);
		
		// correct for size of target objects
		distance -= toObj.Radius;
		
		// check if the object is in range or not
		// distance can be negative with object size correction if objects are inside eachother 
		if( fromObj.RangeOfView >= distance )
			return Mathf.Max(0, distance);
		else return -1;
	}
	
	
	public static List<BasicObject> FindAttackersInRange( BasicObject obj )
	{
		List<BasicObject> objects = new List<BasicObject>();
		foreach( BasicObject attackerObject in Attacker.Objects )
		{
			// add to list if within range
			if( IsInRange(obj, attackerObject) >= 0 )
				objects.Add(attackerObject);
		}
		return objects;
	}
	public static List<BasicObject> FindDefendersInRange( BasicObject obj )
	{
		List<BasicObject> objects = new List<BasicObject>();
		foreach( BasicObject defenderObject in Defender.Objects )
		{
			// add to list if within range
			if( IsInRange(obj, defenderObject) >= 0 )
				objects.Add(defenderObject);
		}
		return objects;
	}
	
	public static BasicObject FindClosestAttackerInRange( BasicObject obj )
	{
		BasicObject closestAttacker = null;
		float closestAttackerDistance = float.MaxValue;
		
		foreach( BasicObject attackerObject in Attacker.Objects )
		{
			float distance = IsInRange(obj, attackerObject);
			
			// store if within range and closer than the current closest
			if( distance >= 0 && distance < closestAttackerDistance )
				closestAttacker = attackerObject;
		}
		return closestAttacker;
	}
	public static BasicObject FindClosestDefenderInRange( BasicObject obj )
	{
		BasicObject closestDefender = null;
		float closestDefenderDistance = float.MaxValue;
		
		foreach( BasicObject defenderObject in Defender.Objects )
		{
			float distance = IsInRange(obj, defenderObject);
			
			// store if within range and closer than the current closest
			if( distance >= 0 && distance < closestDefenderDistance )
				closestDefender = defenderObject;
		}
		return closestDefender;
	}
	
	#endregion
}