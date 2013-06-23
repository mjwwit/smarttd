using UnityEngine;
using System.Collections;

public class HexMapDefinition : MonoBehaviour
{
	// the root is positioned at the middle of the bottom left node
	public Vector3 Position = new Vector3(15.0f, 0.0f, -2.15f);
	
	public int NodeCountX = 30;
	public int NodeCountY = 6;
	
	public int DefaultNodeCost = 1;
	public float NodeWidth = 1;
	
	// NodeHeight = 1 / cos(30deg) * NodeWidth = 4/6 * sqrt(3) * NodeWidth
	const float heightDisplacementRatio = 0.86602540378443864676372317075294f;
	
	public float NodeHeightDisplacement
	{
		get { return heightDisplacementRatio * NodeWidth; }
	}
}
