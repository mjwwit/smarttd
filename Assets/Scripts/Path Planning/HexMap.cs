using UnityEngine;
using System;
using System.Collections.Generic;

public class HexMap : MonoBehaviour
{
	// the root is positioned at the middle of the bottom left node
	public Vector3 Position;
	
	public int NodeCountX = 10;
	public int NodeCountY = 10;
	
	public float NodeWidth = 1;
	
	public bool dummyData = false;
	
	[HideInInspector]
	public float NodeHeightDisplacement;
	
	// non-linear visualization, this is the cost value where the visualization is max!
	public float VisualizationCostMax = 100;
	
	// scaling for the gizmo spheres
	public float VisualizationScale = 1;
	
	// Hexagonal grid!
	Node[][] nodes;
	
	// stores NodeWidth value to check for changes
	float varWidth = 0;
	
	#region Initialization
	public void Awake()
	{
		Initialize(false);
	}
	
	// NodeHeight = 1 / cos(30deg) * NodeWidth = 4/6 * sqrt(3) * NodeWidth
	const float heightDisplacementRatio = 0.86602540378443864676372317075294f;
	// calculate hexagon height displacement from given width
	public float GetNodeHeightDisplacement(float nodeWidth)
	{
		return heightDisplacementRatio * nodeWidth;	
	}
	
	void Initialize(bool dummyValues)
	{
		NodeHeightDisplacement = GetNodeHeightDisplacement(NodeWidth);
		varWidth = NodeWidth;
		
		nodes = new Node[NodeCountX][];
		for(int i = 0; i < NodeCountX; i++)
		{
			nodes[i] = new Node[NodeCountY];
			for(int j = 0; j < NodeCountY; j++)
			{
				nodes[i][j] = new Node()
				{
					Index = new NodeIndex(i, j),
					Position = GetNodePosition(i, j),
					Cost = dummyValues ? Mathf.FloorToInt(UnityEngine.Random.value * 100) : 0
				};
			}
		}
		
		//DEBUG: Load values based on tower position
		Tower t1 = (Tower)FindObjectsOfType(System.Type.GetType("Tower")).GetValue(0);
		ModifyCellsInRange<Tower>(this, t1, HexMap.Modifier_AddTowerDamage);
	}
	
	#endregion
	
	#region Indexing
	
	public Vector3 GetNodePosition(NodeIndex index) 
	{
		return GetNodePosition(index.X, index.Y);
	}
	public Vector3 GetNodePosition(int x, int y)
	{
		return new Vector3(
			Position.x + NodeWidth * (x + y%2 * 0.5f),
			Position.y,
			Position.z + y * NodeHeightDisplacement
		);
	}
	
	public NodeIndex GetNodeIndex(Vector3 pos)
	{
		return GetNodeIndex(pos.x, pos.z);
	}
	public NodeIndex GetNodeIndex(float x, float z)
	{
		x -= Position.x;
		z -= Position.z;
		
		NodeIndex index;
		index.Y = Mathf.RoundToInt(z / NodeHeightDisplacement);
		index.X = Mathf.RoundToInt(x / NodeWidth - 0.5f * (index.Y%2));
		
		return index;
	}
	
	public List<NodeIndex> GetCellsInRangeOf(BasicObject o)
	{
		return GetCellsInRangeOf(this, o);
	}
	public static List<NodeIndex> GetCellsInRangeOf(HexMap m, BasicObject o)
	{
		List<NodeIndex> indices = new List<NodeIndex>();
		
		for(int i = 0; i < m.NodeCountX; i++)
		{
			for(int j = 0; j < m.NodeCountY; j++)
			{
				NodeIndex n = new NodeIndex(i, j);
				if(o.IsInRange(m.GetNodePosition(n)) >= 0)
				{
					indices.Add(n);
				}
			}
		}
		
		return indices;
	}
	
	#endregion
	
	#region Cell Modification
	
	/// <summary>
	/// Modifies the cells in range with a given Modifier function. 
	/// All cells are checked for whether they're in range, that can probably use some optimization.
	/// </summary>
	public static void ModifyCellsInRange<T>(HexMap m, T o, Func<T, int, int> Modifier) where T : BasicObject
	{
		for(int i = 0; i < m.NodeCountX; i++)
		{
			for(int j = 0; j < m.NodeCountY; j++)
			{
				NodeIndex n = new NodeIndex(i, j);
				if(o.IsInRange(m.GetNodePosition(n)) >= 0)
				{
					m.nodes[i][j].Cost = Modifier(o, m.nodes[i][j].Cost);
				}
			}
		}
	}

	/* Modifier template: int Modifier( int cost );
	 * The output float of this function should behave like a distance function.
	 * The closer it is to fulfilling the criterion, the smaller the value.
	*/
	
	// Just an arbitary example function, adding a tower's damage times ten to all cells in range.
	public static int Modifier_AddTowerDamage(Tower t, int cost)
	{
		//Tower t = o as Tower;
		//if(t)
			return cost += t.Damage * 10;
				
		//return cost;
	}
	
	#endregion
	
	#region Visualization
	
	void OnDrawGizmosSelected()
	{
		if(nodes == null 
			|| NodeCountX > nodes.Length 
			|| NodeCountY > nodes[0].Length
			|| varWidth != NodeWidth)
		{
			Initialize(dummyData);
		}
		
		for(int i = 0; i < NodeCountX; i++)
		{
			for(int j = 0; j < NodeCountY; j++)
			{
				float cost = nodes[i][j].Cost;
				float costRatio = cost > 1 ? (Mathf.Log10(cost/(VisualizationCostMax*.1f))*.5f+.5f) : 0;
				Gizmos.color = Visualizer.HSVtoRGB(360 - costRatio*360, 1, 1);
				Gizmos.DrawSphere(GetNodePosition(i, j), .01f + 0.5f * costRatio * VisualizationScale);
			}
		}
	}
	
	#endregion
}
