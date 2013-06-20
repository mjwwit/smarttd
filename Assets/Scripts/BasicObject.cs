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
	
	#region Distance
	public float DistanceTo(BasicObject toObj)
	{
		return Distance(this, toObj.transform.position);
	}
	public float DistanceTo(Vector3 toPos)
	{
		return Distance(this, toPos);
	}
	public static float Distance(BasicObject fromObj, BasicObject toObj)
	{
		return Distance(fromObj, toObj.transform.position);
	}
	public static float Distance(BasicObject fromObj, Vector3 toPos)
	{
		return Vector3.Distance(fromObj.transform.position, toPos);
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
	#endregion
	
	#region FindInRange
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
	
	/* Criterion template: bool Criterion( BasicObject obj );
	 * The output float of this function should behave like a distance function.
	 * The closer it is to fulfilling the criterion, the smaller the value.
	*/
	
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
	
	void OnDrawGizmos()
	{
		drawRange();
	}
	
	#endregion
}