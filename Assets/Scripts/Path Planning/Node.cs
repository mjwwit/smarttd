using UnityEngine;
using System.Collections;

public struct NodeIndex
{
	public int X, Y;
	public NodeIndex(int x, int y) { X=x;Y=y; }
}

// Not sure if this should be a class or a struct.
// Copying classes around could give us some garbage collection issues.
// 	-> However, this might not matter to us, even if we notice some of it
// Using structs may be a bit more difficult to program for (not sure?).
public class Node
{
	public int Cost;
	public NodeIndex Index;
	public Vector3 Position;
}
