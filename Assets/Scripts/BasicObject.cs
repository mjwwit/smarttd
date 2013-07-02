using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

// Our basic object, implents basic variables and functionality every object could use.
public class BasicObject : MonoBehaviour
{
	#region Object Properties
	
	// how far the object can see
	public float RangeOfView = 5;
	
	public bool DrawRangeOfView = false;
	public float DrawViewHeight = 0.1f;
	
	// how large the object is
	// -> should we take this into account when deciding whether targets are in range or not?
	// -> for both or only the target?
	// for simplicity, I think we should not
	public float Radius = 0.5f;
	
	public int MaxHP = 1;
	
	public int HP;
	
	public bool IsAlive = true;
	
	// world units per second
	public float MovementSpeed = 0;
	
	#endregion
	
	#region Private Properties
	
	// caching these is efficient, because the get and set under the hood is a bit slow when used extensively
	// don't worry about it until performance is poor though ;)
	//new Transform transform;
	new Renderer renderer;
	Material material;
	
	// used for drawing the range
	GameObject lineObject;
	LineRenderer lineRenderer;
	Material lineMaterial;
	
	#endregion
	
	// Use this for initialization
	protected virtual void Start ()
	{
		Buffs = new List<Buff>();
		
		//transform = this.GetComponent<Transform>();
		renderer = transform.FindChild("Model").GetComponent<Renderer>();
		material = renderer.material;
		
		HP = MaxHP;
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if(!IsAlive) Destroy(this.gameObject);
	}
	protected virtual void FixedUpdate ()
	{
		updateBuffs(Time.fixedDeltaTime);
	}
	
	#region Status Management
	
	#region Buff Management
	
	public List<Buff> Buffs;
	public void ApplyBuff(Buff b)
	{
		b.SetActive(this);
		Buffs.Add(b);
	}
	public void RemoveBuff(Buff b)
	{
		b.SetInactive();
		Buffs.Remove(b);
	}
	protected void updateBuffs(float deltaTime)
	{
		List<Buff> buffsToUpdate = new List<Buff>(Buffs);
		foreach( Buff b in buffsToUpdate )
		{
			if(b.IsActive)
				b.Update(deltaTime);
		}
	}
	
	#endregion
	
	#region Immunity
	
	// variable used by immunity buff to prevent immunity overrides
	public uint numImmunityBuffs = 0;
	
	bool immune = false;
	public void SetImmunity(bool setImmune)
	{
		this.immune = setImmune;
	}
	#endregion
	
	public virtual void TakeDamage(int damage)
	{
		if(immune)
			Blink(Color.white);
		else
		{
			Blink(Color.red);
			HP -= damage;
			if(HP <= 0)
				Die();
		}
	}
	public virtual void Die()
	{
		IsAlive = false;
	}
	
	#endregion
	
	#region Helper Functions
	
	#region Distance
	public float DistanceTo(BasicObject toObj)
	{
		return Distance(this.transform.position, toObj.transform.position);
	}
	public float DistanceTo(Vector3 toPos)
	{
		return Distance(this.transform.position, toPos);
	}
	public static float Distance(BasicObject fromObj, BasicObject toObj)
	{
		return Distance(fromObj.transform.position, toObj.transform.position);
	}
	public static float Distance(BasicObject fromObj, Vector3 toPos)
	{
		return Distance(fromObj.transform.position, toPos);
	}
	public static float Distance(Vector3 fromPos, Vector3 toPos)
	{
		return Vector3.Distance(fromPos, toPos);
	}
	#endregion
	
	#region IsInRange
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public float IsInRange(BasicObject toObj)
	{
		return IsInRange(this, toObj.transform.position);
	}
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public float IsInRange(Vector3 toPos)
	{
		return IsInRange(this, toPos);
	}
	
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public static float IsInRange(BasicObject fromObj, BasicObject toObj)
	{
		return IsInRange(fromObj, toObj.transform.position);
	}
	/// <summary>
	/// Returns the distance if in range, -1 otherwise.
	/// </summary>
	public static float IsInRange(BasicObject fromObj, Vector3 toPos)
	{
		// determine distance between objects
		float distance = Distance(fromObj, toPos);
		
		// correct for size of target objects
		// distance -= toObj.Radius;
		
		// check if the object is in range or not
		// distance can be negative with object size correction if objects are inside eachother 
		if( fromObj.RangeOfView >= distance )
			return Mathf.Max(0, distance);
		else return -1;
	}
	public static float IsInRange(Vector3 fromPos, float range, BasicObject toObj)
	{
		return IsInRange(fromPos, range, toObj.transform.position);
	}
	public static float IsInRange(Vector3 fromPos, float range, Vector3 toPos)
	{
		// determine distance between objects
		float distance = Distance(fromPos, toPos);
		
		// check if the object is in range or not
		if( range >= distance )
			return distance;
		else return -1;
	}
	#endregion
	
	#region FindInRange
	public static List<BasicObject> FindAttackersInRange( BasicObject obj )
	{
		return FindAllInRange<BasicObject>( obj.transform.position, obj.RangeOfView, Attacker.Objects );
	}
	public static List<BasicObject> FindDefendersInRange( BasicObject obj )
	{
		return FindAllInRange<BasicObject>( obj.transform.position, obj.RangeOfView, Defender.Objects );
	}
	public static List<T> FindAllInRange<T>( Vector3 position, float range, List<T> objects ) where T : BasicObject
	{
		List<T> objectsInRange = new List<T>();
		foreach( T obj in objects )
		{
			// add to list if within range
			if( IsInRange(position, range, obj) >= 0 )
				objectsInRange.Add(obj);
		}
		return objectsInRange;
	}
	
	public static List<T> FindAllInRange<T, U>( Vector3 position, float range, List<U> objects ) where T : BasicObject
	{
		List<T> objectsInRange = new List<T>();
		foreach( U obj in objects )
		{
			T o = obj as T;
			
			if( o == null ) continue;
			
			// add to list if within range
			if( IsInRange(position, range, o) >= 0 )
				objectsInRange.Add(o);
		}
		return objectsInRange;
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
		return FindClosestInRange<T>( obj.transform.position, obj.RangeOfView, objects );
	}
	public static T FindClosestInRange<T>( Vector3 position, float range, List<T> objects ) where T : BasicObject
	{
		T closestObject = null;
		float closestObjectDistance = float.MaxValue;
		
		float distance;
		
		foreach( T targetObj in objects )
		{
			distance = IsInRange(position, range, targetObj.transform.position);
			
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
	
	/* Criterion template: bool Criterion( BasicObject obj );
	 * The output float of this function should behave like a distance function.
	 * The closer it is to fulfilling the criterion, the smaller the value.
	*/
	
	public static float Criterion_DistanceToGoal(BasicObject obj)
	{
		return Attacker.DistanceToGoal(obj.transform.position);
		
		/* -- old method
		// Project the object's position on the direction, bigger means it's closer to the goal.
		float projectedDistance = Vector3.Dot(obj.transform.position, Attacker.GoalDirection);
		
		// Times -1, to make sure the end result is smaller for objects closer to the goal.
		return -projectedDistance;
		*/
	}
	
	public static float Criterion_LowHP(BasicObject obj)
	{
		return obj.HP;
	}
	public static float Criterion_HighHP(BasicObject obj)
	{
		return -obj.HP;
	}
	#endregion
	
	#endregion
	
	#region Visualization
	
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
	
	const int numLineSegments = 20;
	public void drawRange()
	{
		// if we don't have it yet, get the line object
		if(!lineObject)
		{
			Transform t = transform.FindChild("LineObject");
			if(t)
			{
				lineObject = t.gameObject;
				lineRenderer = lineObject.GetComponent<LineRenderer>();
			}
		}
		
		// if none exists, create one
		/*if(!lineObject)
		{
			lineObject = new GameObject("LineObject");
			lineObject.transform.parent = transform;

			lineRenderer = lineObject.AddComponent<LineRenderer>();
			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			lineRenderer.SetColors(Color.red, Color.red);
			lineRenderer.SetWidth(0.1f, 0.1f);
			lineRenderer.SetVertexCount(numLineSegments+1);
			lineRenderer.useWorldSpace = false;
			
			SceneView.RepaintAll();
		}*/
		
		// set inactive when no drawing is wanted and return
		if(!DrawRangeOfView)
		{
			if(lineObject.activeSelf)
			{
				lineObject.SetActive(false);
				//SceneView.RepaintAll();
				HandleUtility.Repaint();
			}
			return;
		}
		
		// set active if required
		if(!lineObject.activeSelf)
		{
			lineObject.SetActive(true);
			//SceneView.RepaintAll();
			HandleUtility.Repaint();
		}
		
		// set parameters
		lineObject.transform.localPosition = new Vector3(0, DrawViewHeight, 0);
		for(int i=0; i < numLineSegments+1; i++)
		{
			float theta = i*Mathf.PI*2/(numLineSegments);
		    float x = RangeOfView * (float)Math.Cos(theta);
			float y = 0;
		    float z = RangeOfView * (float)Math.Sin(theta);
			
		    lineRenderer.SetPosition(i, new Vector3(x, y, z));
		}
	}
	
	protected virtual void OnDrawGizmos()
	{
		drawRange();
	}
	
	#endregion
}