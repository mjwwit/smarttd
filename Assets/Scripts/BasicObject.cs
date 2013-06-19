using UnityEngine;
using System;
using System.Collections;
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
	// for simplicity, I think we should not
	public float Radius = 0.5f;
	
	public int MaxHP = 1;
	
	[HideInInspector]
	public int HP;
	
	[HideInInspector]
	public bool IsAlive = true;
	
	// world units per second
	public float MovementSpeed = 0;
	
	#endregion
	
	#region Private Properties
	
	// caching these is efficient, because the get and set under the hood is a bit slow when used extensively
	// don't worry about it until performance is poor though ;)
	new Transform transform;
	new Renderer renderer;
	Material material;
	
	#endregion
	
	
	// Use this for initialization
	protected virtual void Start ()
	{
		transform = this.GetComponent<Transform>();
		renderer = this.GetComponentInChildren<Renderer>();
		material = renderer.material;
		
		HP = MaxHP;
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if(!IsAlive) Destroy(this.gameObject);
	}
	
	public virtual void TakeDamage(int damage)
	{
		Blink(Color.red);
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
	
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public float IsInRange(BasicObject toObj)
	{
		return IsInRange(this, toObj);
	}
	
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public static float IsInRange(BasicObject fromObj, BasicObject toObj)
	{
		// determine distance between objects
		float distance = Distance(fromObj, toObj);
		
		// correct for size of target objects
		// distance -= toObj.Radius;
		
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
		return FindClosestInRange<BasicObject>(obj, Attacker.Objects);
	}
	public static BasicObject FindClosestDefenderInRange( BasicObject obj )
	{
		return FindClosestInRange<BasicObject>(obj, Defender.Objects);
	}
	public static T FindClosestInRange<T>( BasicObject obj, List<T> objects ) where T : BasicObject
	{
		T closestObject = null;
		float closestObjectDistance = float.MaxValue;
		
		float distance;
		
		foreach( T targetObj in objects )
		{
			distance = IsInRange(obj, targetObj);
			
			// store if within range and closer than the current closest
			if( distance >= 0 && distance < closestObjectDistance )
			{
				closestObject = targetObj;
				closestObjectDistance = distance;
			}
		}
		return closestObject;
	}
	
	
	/// <summary>
	/// For every object in objects, first checks if the object is in range of fromObj, 
	///  then checks if the criterion is satisfied better than the previously found best.
	/// </summary>
	public static T FindBestSuitedObjectInRange<T>( 
			BasicObject fromObj, 
			List<T> objects,
			Func<BasicObject, float> criterion
		) where T : BasicObject
	{
		T bestSuitedObject = null;
		float lowestCriterionValue = float.MaxValue; // the 'distance' in criterion space
		
		foreach( T obj in objects )
		{
			// don't check against ourselves
			if(obj == fromObj) continue;
			
			// if not within range, skip
			if(IsInRange(fromObj, obj) < 0) continue;
			
			// calculate criterion value
			float criterionValue = criterion(obj);
			
			// keep if a lower criterion value
			if( criterionValue < lowestCriterionValue )
			{
				bestSuitedObject = obj;
				lowestCriterionValue = criterionValue;
			}
		}
		return bestSuitedObject;
	}
	
	/// <summary>
	/// Criterion function delegate. The output float of this function should behave like a distance function.
	///  The closer it is to fulfilling the criterion, the smaller the value.
	/// </summary>
	public delegate bool Criterion( BasicObject obj );
	
	// A naïve and simple check to see how close the object is to the attacker's goal.
	public static float Criterion_DistanceToGoal(BasicObject obj)
	{
		// Project the object's position on the direction, bigger means it's closer to the goal.
		float projectedDistance = Vector3.Dot(obj.transform.position, Attacker.GoalDirection);
		
		// Times -1, to make sure the end result is smaller for objects closer to the goal.
		return -projectedDistance;
	}
	
	public static float Criterion_LowHP(BasicObject obj)
	{
		return obj.HP;
	}
	public static float Criterion_HighHP(BasicObject obj)
	{
		return -obj.HP;
	}
	
	//Visualisation
	public void Blink(Color col)
	{
		StartCoroutine(blink(col));
	}
	WaitForSeconds wait = new WaitForSeconds(0.1f);
	IEnumerator blink(Color col)
	{
		material.color=col;
		
		yield return wait;
		
		material.color=Color.white;
	}
	public void drawRange()
	{
		float theta_scale = 0.1f;             //Set lower to add more points
		int size = (int)Math.Round((2.0 * Math.PI) / theta_scale, 0); //Total number of points in circle.
		
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(Color.red, Color.red);
		lineRenderer.SetWidth(0.1f, 0.1f);
		lineRenderer.SetVertexCount(size);
		
		int i = 0;
		for(float theta = 0f; theta < 2 * Math.PI; theta += 0.1f) {
		    float x = transform.position.x + RangeOfView * (float)Math.Cos(theta);
		    float z = transform.position.z + RangeOfView * (float)Math.Sin(theta);
		
		    Vector3 pos = new Vector3(x, 0.6f, z);
		    lineRenderer.SetPosition(i, pos);
		    i+=1;
		}	
	}
	#endregion
}