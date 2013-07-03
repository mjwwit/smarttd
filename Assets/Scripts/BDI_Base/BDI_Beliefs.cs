using UnityEngine;
using System.Collections;

public abstract class BDI_Beliefs
{
	public abstract void Update();
	
	public virtual string MyName()
	{
		return string.Empty;
	}
}
