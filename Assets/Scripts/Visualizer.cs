using UnityEngine;
using System.Collections.Generic;

// Created it and ended up using Debug.DrawLine, because this didn't show up in the scene view.
// Does the same thing as Debug.DrawLine, but in release mode.
// 
public class Visualizer : MonoBehaviour
{
	struct Line
	{
		public Vector3 Start, End;
		public Color Color;
		
		public Line(Vector3 start, Vector3 end, Color color)
		{
			Start = start;
			End = end;
			Color = color;
		}
	}
	
	static Visualizer me;
	
	List<Line> lines;
    Material lineMaterial;
	
	void Start()
	{
		me = this;
		lines = new List<Line>();
		
		createLineMaterial();
	}
	
    void createLineMaterial()
    {
        if( !lineMaterial )
		{
            lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }" );
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }
     
	public static void DrawLine(Vector3 start, Vector3 end, Color c)
	{
		me.lines.Add(new Line(start, end, c));
	}
	
    void OnPostRender()
    {
		if(lines.Count == 0) return;
		
        lineMaterial.SetPass( 0 );
        GL.Begin( GL.LINES );
		foreach( Line line in lines )
		{
        	GL.Color( line.Color );
	        GL.Vertex3( line.Start.x, line.Start.y, line.Start.z );
	        GL.Vertex3( line.End.x, line.End.y, line.End.z );
		}
        GL.End();
		
		lines.Clear();
    }
}
