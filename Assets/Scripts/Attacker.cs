using UnityEngine;
using System.Collections.Generic;

public struct Line
{
	public Vector3 v, w;
	public Line(Vector3 start, Vector3 end)
	{
		this.v = start;
		this.w = end;
	}
	
	// From http://www.gamedev.net/topic/444154-closest-point-on-a-line/
	// Returns the minimum distance between line segment vw and point p.
	public float Distance2D(Vector3 p)
	{
		Vector3 t = ClosestPointOnLine(p);
		t.y = 0;
		p.y = 0;
		return Vector3.Distance(t, p);
	}
	
	// Returns the closest point from a given point on a given line.
	public Vector3 ClosestPointOnLine(Vector3 p)
	{
	    Vector3 AP = p-v;
	    Vector3 AB = w-v;
	    float ab2 = AB.x*AB.x + AB.y*AB.y + AB.z*AB.z;
		if(ab2 == 0) return v;
	    float ap_ab = AP.x*AB.x + AP.y*AB.y + AP.z*AB.z;
	    float t = ap_ab / ab2;
		if (t <= 0.0f) return v;
		else if (t >= 1.0f) return w;
	    return v + AB * t;
	}
}

public static class Attacker
{
	//public static Vector3 GoalDirection = Vector3.Normalize(new Vector3(1, 0, 0));
	
	public static Line Goal = new Line(
		new Vector3(22.0f, 0.0f, -2),
		new Vector3(22.0f, 0.0f, 2)
	);
	
	public static List<BasicObject> Objects;
	public static List<Unit> Units;
	
	public static void Awake ()
	{
		Objects = new List<BasicObject>( (Unit[])GameObject.FindObjectsOfType(typeof(Unit)) );
		Units = new List<Unit>( (Unit[])GameObject.FindObjectsOfType(typeof(Unit)) );
	}
	
	public static void AddObject(BasicObject obj)
	{
		Objects.Add(obj);
		
		Unit u = obj as Unit;
		if(u) Units.Add( u );
	}
	
	public static void RemoveObject(BasicObject obj)
	{
		Objects.Remove(obj);
		
		Unit u = obj as Unit;
		if(u) Units.Remove( u );
	}
	
	// Check to see how close we are to the goal.
	public static float DistanceToGoal(Vector3 pos)
	{
		return Goal.Distance2D(pos);
	}
	public static Vector3 ClosestPointOnGoal(Vector3 pos)
	{
		return Goal.ClosestPointOnLine(pos);
	}
	public static Vector3 VectorToGoal(Vector3 pos)
	{
		return Goal.ClosestPointOnLine(pos) - pos;
	}
}
