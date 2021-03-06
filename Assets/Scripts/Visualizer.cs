using UnityEngine;
using System.Collections.Generic;

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
	
	void Awake()
	{
		me = this;
		lines = new List<Line>();
	}
     
	public static void DrawLine(Vector3 start, Vector3 end, Color c)
	{
		me.lines.Add(new Line(start, end, c));
	}
	
	// Gizmos drawing method
	void OnDrawGizmos()
	{
		if(lines == null || lines.Count == 0) return;
		
		foreach( Line line in lines )
		{
			Gizmos.color = line.Color;
			Gizmos.DrawLine(line.Start, line.End);
		}
		lines.Clear();
	}
	
	// GL drawing method, only shows in game view
	/*
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
    */
	
	/// <summary>
    /// Method to convert HSV to RGB, based on information from Wikipedia
	/// </summary>
	/// <returns>
	/// Color in RGB.
	/// </returns>
	/// <param name='hue'>
	/// Hue. [0, 360>
	/// </param>
	/// <param name='saturation'>
	/// Saturation. [0, 1]
	/// </param>
	/// <param name='value'>
	/// Value. [0, 1]
	/// </param>
    public static Color HSVtoRGB(float hue, float saturation, float value)
    {
		hue %= 360;
		
        Color res;
        float c = value * saturation;
        float x = c * (1 - Mathf.Abs((hue / 60) % 2 - 1));
        float m = value - c;

        c += m;
        x += m;

        if (hue < 60)
            res = new Color(c, x, m);
        else if (hue < 120)
            res = new Color(x, c, m);
        else if (hue < 180)
            res = new Color(m, c, x);
        else if (hue < 240)
            res = new Color(m, x, c);
        else if (hue < 300)
            res = new Color(x, m, c);
        else
            res = new Color(c, m, x);

        return res;
    }
}
