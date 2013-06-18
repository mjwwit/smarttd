using UnityEngine;
using System.Collections;

public class Unit : BasicObject
{
	Vector3 goal;
	
	protected override void Start ()
	{
		base.Start ();
	}
	protected override void Update ()
	{
		base.Update ();
		
		// move towards the goal
		Vector3 diff = goal - transform.position;
		float distance = diff.magnitude;
		if(distance > 0) diff /= distance;
		transform.position += diff * Mathf.Min(distance, MovementSpeed) * Time.deltaTime;
	}
	
	public void SetGoal(Vector3 goal)
	{
		this.goal = goal;
	}
	
	public override void Die ()
	{
		base.Die ();
		Attacker.RemoveObject(this);
	}
}