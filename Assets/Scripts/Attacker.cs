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
		
		/*
		const float l2 = Vector3.Dot(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
		if (l2 == 0.0f) return Vector3.Distance(p, v);   // v == w case
		// Consider the line extending the segment, parameterized as v + t (w - v).
		// We find projection of point p onto the line. 
		// It falls where t = [(p-v) . (w-v)] / |w-v|^2
		const float t = dot(p - v, w - v) / l2;
		if (t < 0.0f) return Vector3.Distance(p, v);       // Beyond the 'v' end of the segment
		else if (t > 1.0f) return Vector3.Distance(p, w);  // Beyond the 'w' end of the segment
		const Vector3 projection = v + t * (w - v);  // Projection falls on the segment
		return Vector3.Distance(p, projection);
		*/
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

// Static, for ease of programming. Not going to bother with fancy structures! Practicality is our friend.
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
		if(obj.GetType() == typeof(Unit)) Units.Add( (Unit)obj );
	}
	
	public static void RemoveObject(BasicObject obj)
	{
		Objects.Remove(obj);
		if(obj.GetType() == typeof(Unit)) Units.Remove( (Unit)obj );
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
