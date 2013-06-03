using UnityEngine;
using System.Collections;

public class Unit : BasicObject
{
	protected override void Start ()
	{
		base.Start ();
	}
	protected override void Update ()
	{
		base.Update ();
	}
	
	public override void Die ()
	{
		base.Die ();
		Attacker.RemoveObject(this);
	}
}