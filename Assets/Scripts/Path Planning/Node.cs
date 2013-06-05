using UnityEngine;
using System.Collections;

public struct NodeIndex
{
	public int X, Y;
}

// Not sure if this should be a class or a struct.
// Copying classes around could give us some garbage collection issues.
// 	-> However, this might not matter to us, even if we notice some of it
// Using structs may be a bit more difficult to program for (not sure?).
public struct Node
{
	// maybe store the world position as well
	// or maybe in a separate array?
	// public Vector2 Position;
	
	public NodeIndex Index;
	public int Cost;
}
