using UnityEngine;
using System.Collections;

public class ScriptInitializer : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
	{
		// initialize attacker and defender lists
		Attacker.Awake();
		Defender.Awake();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}