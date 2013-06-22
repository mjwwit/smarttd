using UnityEngine;
using System;
using System.Collections.Generic;

public class HexMap : MonoBehaviour
{
	#region Properties
	
	public HexMapDefinition d;
	
	// draw grid ? 
	public bool DrawGrid = true;
	// use pseudo-random data for drawing grid?
	public bool DummyData = false;
	
	public int goalX = 2;
	public int goalY = 3;
	
	// non-linear visualization, this is the cost value where the visualization is max!
	public float VisualizationCostMax = 100;
	
	// scaling for the gizmo spheres
	public float VisualizationScale = 0.5f;
	
	#endregion
	
	
	#region Variables
	
	public List<Node> OptimalPath = new List<Node>();
	
	// Hexagonal grid!
	Node[][] nodes;
	
	// stores NodeWidth value to check for changes
	// used for grid redefinition when width is changed in editor
	float varWidth = 0;
	
	#endregion
	
	
	#region Initialization
	
	public void Awake()
	{
		Initialize(false);
	}
	
	void Initialize(bool dummyValues)
	{
		if(!d) d = GameObject.FindObjectOfType(typeof(HexMapDefinition)) as HexMapDefinition;
		if(!d) Debug.LogError("Missing instance of HexMapDefinition in scene!");
		
		varWidth = d.NodeWidth;
		
		int currentID = 0;
		
		nodes = new Node[d.NodeCountX][];
		for(int i = 0; i < d.NodeCountX; i++)
		{
			nodes[i] = new Node[d.NodeCountY];
			for(int j = 0; j < d.NodeCountY; j++)
			{
				nodes[i][j] = new Node()
				{
					Index = new NodeIndex(i, j),
					Position = GetNodePosition(i, j),
					Cost = dummyValues ? Mathf.FloorToInt(UnityEngine.Random.value * 100) : 0,
					ID = currentID
				};
				
				currentID++;
			}
		}
		if(!dummyValues)
		{
			// Load values based on distance to goal
			initNodeCost(nodes[goalX][goalY]);
			
			//DEBUG: Load values based on tower positions
			foreach(Tower t in FindObjectsOfType(System.Type.GetType("Tower")))
			{
				ModifyCellsInRange<Tower>(this, t, HexMap.GridModifier_AddTowerDamage, t);
			}
		}
		
		//OptimalPath = GetOptimalPath<Unit>(this, gameObject.GetComponent<Unit>(), this.getGoalNode());
	}
	
	// Breadth-First-Search based grid cost initialization
	public void initNodeCost(Node n)
	{
		Queue<Node> queue = new Queue<Node>();
		queue.Enqueue(n);
		n.Cost = 1;
		
		while(queue.Count != 0)
		{
			n = queue.Dequeue();
			foreach(Node n2 in NeighboursOf(n))
			{
				if(n2.Cost == 0)
				{
					n2.Cost = n.Cost + 1;
					queue.Enqueue(n2);
				}
			}
		}
	}
	
	#endregion
	
	#region Indexing
	
	public Vector3 GetNodePosition(Node n) { return GetNodePosition(n.Index.X, n.Index.Y); }
	public Vector3 GetNodePosition(NodeIndex index) 
	{
		return GetNodePosition(index.X, index.Y);
	}
	public Vector3 GetNodePosition(int x, int y)
	{
		return new Vector3(
			d.Position.x + d.NodeWidth * (x + y%2 * 0.5f),
			d.Position.y,
			d.Position.z + y * d.NodeHeightDisplacement
		);
	}
	
	public NodeIndex GetNodeIndex(Vector3 pos)
	{
		return GetNodeIndex(pos.x, pos.z);
	}
	public NodeIndex GetNodeIndex(float x, float z)
	{
		x -= d.Position.x;
		z -= d.Position.z;
		
		NodeIndex index;
		index.Y = Mathf.RoundToInt(z / d.NodeHeightDisplacement);
		index.X = Mathf.RoundToInt(x / d.NodeWidth - 0.5f * (index.Y%2));
		
		return index;
	}
	
	// Get node by ID.
	public Node GetNode(int id)
	{
		int y = id % d.NodeCountY;
		int x = (id-y) / d.NodeCountY;
		return GetNode(x, y);
		//return GetNode((int)Math.Floor((float)id / (float)NodeCountY), id - ((int)Math.Floor((float)id / (float)NodeCountY) * NodeCountY));
	}
	public Node GetNode(Vector3 pos) { return GetNode(pos.x, pos.z); }
	public Node GetNode(float x, float z)
	{
		NodeIndex i = GetNodeIndex(x, z);
		return nodes[i.X][i.Y];
	}
	public Node GetNode(int X, int Y) { return nodes[X][Y]; }
	
	public Node getGoalNode(){ return GetNode(goalX, goalY); }
	
	public List<Node> GetCellsInRangeOf(BasicObject o)
	{
		return GetCellsInRangeOf(this, o);
	}
	public static List<Node> GetCellsInRangeOf(HexMap m, BasicObject o)
	{
		List<Node> nodes = new List<Node>();
		
		for(int x = 0; x < m.d.NodeCountX; x++)
		{
			for(int y = 0; y < m.d.NodeCountY; y++)
			{
				Node n = m.nodes[x][y];
				if(o.IsInRange(m.GetNodePosition(n.Index)) >= 0)
				{
					nodes.Add(n);
				}
			}
		}
		
		return nodes;
	}
	
	public IEnumerable<Node> NeighboursOf(int id)
	{
		int y = id % d.NodeCountY;
		int x = (id-y) / d.NodeCountY;
		return NeighboursOf(x, y);
		//return NeighboursOf((int)Math.Floor((float)id / (float)NodeCountY), id - ((int)Math.Floor((float)id / (float)NodeCountY) * NodeCountY));
	}
	public IEnumerable<Node> NeighboursOf(Node n) { return NeighboursOf(n.Index.X, n.Index.Y); }
	public IEnumerable<Node> NeighboursOf(NodeIndex i) { return NeighboursOf(i.X, i.Y); }
	public IEnumerable<Node> NeighboursOf(int x, int y)
	{
		int xMin = x-1;
		int xPlus = x+1;
		int yMin = y-1;
		int yPlus = y+1;
		
		// these series of if-statements could use some optimization by branching
		if(y%2==0)
		{
			// neighbours clockwise, starting left
			// x-1, y
			// y+1, x-1
			// y+1, x
			// x+1, y
			// y-1, x
			// y-1, x-1
			if(xMin >= 0) 						yield return nodes[x-1][y];
			if(xMin >= 0 && yPlus < d.NodeCountY) yield return nodes[x-1][y+1];
			if(yPlus < d.NodeCountY) 				yield return nodes[x]  [y+1];
			if(xPlus < d.NodeCountX) 				yield return nodes[x+1][y];
			if(yMin >= 0) 						yield return nodes[x]  [y-1];
			if(xMin >= 0 && yMin >= 0)		 	yield return nodes[x-1][y-1];
		}
		else
		{
			// neighbours clockwise, starting left
			// x-1, y
			// y+1, x
			// y+1, x+1
			// x+1, y
			// y-1, x+1
			// y-1, x
			if(xMin >= 0) 								 yield return nodes[x-1][y];
			if(yPlus < d.NodeCountY) 						 yield return nodes[x][y+1];
			if(xPlus < d.NodeCountX && yPlus < d.NodeCountY) yield return nodes[x+1]  [y+1];
			if(xPlus < d.NodeCountX) 						 yield return nodes[x+1][y];
			if(xPlus < d.NodeCountX && yMin >= 0) 		 yield return nodes[x+1]  [y-1];
			if(yMin >= 0) 								 yield return nodes[x][y-1];
		}
	}
	
	#endregion
	
	#region Cell Modification
	
	/// <summary>
	/// Modifies the cells in range with a given Modifier function. 
	/// All cells are checked for whether they're in range, that can probably use some optimization.
	/// </summary>
	public static void ModifyCellsInRange<T>(HexMap m, BasicObject o, Func<int, T, int> Modifier, T metaData)
	{
		for(int i = 0; i < m.d.NodeCountX; i++)
		{
			for(int j = 0; j < m.d.NodeCountY; j++)
			{
				NodeIndex n = new NodeIndex(i, j);
				if(o.IsInRange(m.GetNodePosition(n)) >= 0)
				{
					m.nodes[i][j].Cost = Modifier(m.nodes[i][j].Cost, metaData);
				}
			}
		}
	}
	
	/* Modifier template: int Modifier( int cost );
	 * Input is a BasicObject as metadata and the grid cell's current cost.
	 * Output is the new cell value.
	 * 
	 * Example usage:
	 * ModifyCellsInRange<MetaDataType>(hexMap, basicObject, GridModifierFunc, metaDataObject);
	 * 
	 * For example, using the grid modifier that adds tower damage:
	 * ModifyCellsInRange<Tower>(hexMap, tower, GridModifier_AddTowerDamage, tower);
	 * 
	 */
	
	// Just an arbitary example function, adding a tower's damage times ten to the cell.
	public static int GridModifier_AddTowerDamage(int cost, Tower t)
	{
		return cost += t.Damage * 10;
	}
	
	#endregion
	
	#region Analyzation
	
	// Find the optimal path through the given HexMap m for given unit c to goal Node g, using Dijkstra's.
	public static List<Node> GetOptimalPath<T>(HexMap m, T c, Node g) where T : BasicObject
	{
		int[] distances = new int[m.d.NodeCountX * m.d.NodeCountY];
		int[] previous = new int[m.d.NodeCountX * m.d.NodeCountY];
		
		for(int i = 0; i < distances.Length; ++i)
		{
			distances[i] = int.MaxValue;
			previous[i] = -1;
		}
		
		int source = m.GetNode(new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z)).ID;
		distances[source] = 0;
		
		List<int> q = new List<int>();
		
		foreach(Node[] ns in m.nodes)
			foreach(Node n in ns)
				q.Add(n.ID);
		
		while(q.Count != 0)
		{
			// Find index with smallest distance
			int minVal = int.MaxValue;
			int minIndex = 0;
			for(int i = 0; i < q.Count; ++i)
			{
				if(distances[q[i]] < minVal)
				{
					minIndex = q[i];
					minVal = distances[q[i]];
				}
			}
			
			// Remove from list of unoptimized nodes
			q.Remove(minIndex);
			
			// Check if goal is reached
			if(minIndex == m.getGoalNode().ID) break;
			
			// Check if node is unreachable
			if(minVal == int.MaxValue) break;
			
			foreach(Node n in m.NeighboursOf(minIndex))
			{
				// Check if more optimal path has been found
				int alt = distances[minIndex] + n.Cost;
				if(alt < distances[n.ID])
				{
					distances[n.ID] = alt;
					previous[n.ID] = minIndex;
				}
			}
		}
		
		// Reconstruct optimal path
		List<Node> optimalPath = new List<Node>();
		Node cn = m.getGoalNode();
		while(previous[cn.ID] != -1)
		{
			optimalPath.Insert(0, cn);
			cn = m.GetNode(previous[cn.ID]);
		}
		
		return optimalPath;
	}
	
	#endregion
	
	#region Visualization
	
	void OnDrawGizmosSelected()
	{
		if(DrawGrid)
		{
			if(nodes == null 
				|| !d
				|| d.NodeCountX > nodes.Length 
				|| d.NodeCountY > nodes[0].Length
				|| varWidth != d.NodeWidth)
			{
				Initialize(DummyData);
			}
			
			for(int i = 0; i < d.NodeCountX; i++)
			{
				for(int j = 0; j < d.NodeCountY; j++)
				{
					float cost = nodes[i][j].Cost;
					float costRatio = cost > 1 ? (Mathf.Log10(cost/(VisualizationCostMax*.1f))*.5f+.5f) : 0;
					Gizmos.color = Visualizer.HSVtoRGB(360 - costRatio*360, 1, 1);
					Gizmos.DrawSphere(GetNodePosition(i, j), .01f + 0.5f * costRatio * VisualizationScale);
				}
			}
		}
		
		Gizmos.color = Color.white;
		if(OptimalPath.Count != 0)
		{
			for(int i = 0; i < OptimalPath.Count - 1; ++i)
			{
				Gizmos.DrawLine(GetNodePosition(OptimalPath[i]), GetNodePosition(OptimalPath[i+1]));
			}
		}
	}
	
	#endregion
}
