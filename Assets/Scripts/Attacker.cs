using UnityEngine;
using System.Collections.Generic;

// Static, for ease of programming. Not going to bother with fancy structures! Practicality is our friend.
public static class Attacker
{
	public static Vector3 GoalDirection = (new Vector3(1, 0, 0)).normalized;
	
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
}
