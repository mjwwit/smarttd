using UnityEngine;
using System.Collections.Generic;

public static class Defender
{
	public static List<BasicObject> Objects;
	public static List<Tower> Towers;
	
	public static void Awake ()
	{
		Objects = new List<BasicObject>( (Tower[])GameObject.FindObjectsOfType(typeof(Tower)) );
		Towers = new List<Tower>( (Tower[])GameObject.FindObjectsOfType(typeof(Tower)) );
	}
	
	public static void AddObject(BasicObject obj)
	{
		Objects.Add(obj);
		
		Tower t = obj as Tower;
		if(t) Towers.Add( t );
	}
	
	public static void RemoveObject(BasicObject obj)
	{
		Objects.Remove(obj);
		
		Tower t = obj as Tower;
		if(t) Towers.Remove( t );
	}
}
