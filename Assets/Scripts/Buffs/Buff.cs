using UnityEngine;
using System.Collections;

public abstract class Buff
{
	protected BasicObject subject;
	public float Duration;
	public bool IsActive;
	
	protected float timeLeft;
	
	public Buff(float duration)
	{
		Duration = duration;
	}
	
	protected virtual void applyEffect()
	{
		
	}
	protected virtual void executeEffect()
	{
		
	}
	protected virtual void removeEffect()
	{
		
	}
	
	public void Update(float deltaTime)
	{
		if(!subject) return;
		
		executeEffect();
		timeLeft -= deltaTime;
		
		if(timeLeft <= 0)
			subject.RemoveBuff(this);
	}
	
	public void SetDuration(float duration)
	{
		this.Duration = duration;
		this.timeLeft = duration;
	}
	public void SetActive(BasicObject subject)
	{
		this.IsActive = true;
		this.subject = subject;
		this.timeLeft = Duration;
		this.applyEffect();
	}
	public void SetInactive()
	{
		this.IsActive = false;
		this.removeEffect();
		this.subject = null;
	}
}