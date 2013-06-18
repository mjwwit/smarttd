using UnityEngine;
using System.Collections;

public class HexMap
{
	public int NodeCountX = 10;
	public int NodeCountY = 10;
	
	public float NodeWidth = 1;
	public float NodeHeight;
	
	// the root is positioned at the middle of the bottom left node
	Vector2 root;
	
	// Hexagonal grid!
	Node[][] nodes;
	
	public HexMap(int nodeCountX, int nodeCountY, float nodeWidth = 1, float rootX = 0, float rootY = 0)
	{
		nodes = new Node[NodeCountX][];
		for(int i = 0; i < NodeCountX; i++)
			nodes[i] = new Node[NodeCountY];
		
		root.x = rootX;
		root.y = rootY;
		
		// calculate hexagon height from given width
		// NodeHeight = 0.5 * NodeWidth * ( tan(PI/6) + tan(PI/3) ) = 4/6 * sqrt(3) * NodeWidth
		//    or NodeHeight / cos(30)
		NodeHeight = 4/6f * Mathf.Sqrt(3) * NodeWidth;
	}
	
	public Vector2 GetNodePosition(NodeIndex index) 
	{
		return GetNodePosition(index.X, index.Y);
	}
	public Vector2 GetNodePosition(int x, int y)
	{
		return new Vector2(
			root.x + NodeWidth * (x + y%2 * 0.5f),
			root.y + y * NodeHeight
		);
	}
	
	public NodeIndex GetNodeIndex(Vector2 pos)
	{
		return GetNodeIndex(pos.x, pos.y);
	}
	public NodeIndex GetNodeIndex(float x, float y)
	{
		x -= root.x;
		y -= root.y;
		
		NodeIndex index;
		index.Y = Mathf.RoundToInt(y / NodeHeight);
		index.X = Mathf.RoundToInt(x / NodeWidth - 0.5f * (index.Y%2));
		
		return index;
	}
}
