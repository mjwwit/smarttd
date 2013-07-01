using System.Collections.Generic;

/*
We represent the information about the means of achieving certain future world states 
and the options available to the agent as plans, which can be viewed as a special form 
of beliefs (Rao and Georgeff 1992). 
Intuitively, plans are abstract specifications of both the means for achieving certain 
desires and the options available to the agent. 
Each plan has a body describing the primitive actions or subgoals that have to be 
achieved for plan execution to be successful.
The conditions under which a plan can be chosen as an option are specified by an 
invocation condition and a pre-condition; the invocation condition specifies the 
"triggering" event that is necessary for invocation of the plan, and the precondition 
specifies the situation that must hold for the plan to be executable.
*/
public abstract class Plan
{
	static int idCounter = 0;
	public readonly int ID;
	public Plan()
	{
		ID = idCounter++;
	}
	
	/*Plan parent;
	
	public void Initialize()
	{
		foreach(Plan p in SubPlans)
		{
			p.parent = this;
			p.Initialize();
		}
	}*/
	
	//BDI_Agent agent;
	
	// the subplans required for completing this plan's goals
	public List<Plan> SubPlans;
	
	// When both the precondition and the invocation condition are satisfied, we have a reason to commit to the plan.
	
	// return true if the current situation satisfies the pre-condition of this plan
	public abstract bool SatisfiesPreCondition();
	// return true if the current situation satisfies the invocation condition ("trigger event")
	public abstract bool SatisfiesInvocationCondition();
	
	// If this condition is satisfied, we stop committing to the plan.
	// Considered for every step in the plan.
	public abstract bool SatisfiesTerminationCondition();
	
	// If this condition is satisfied, we go to the next (sub)step in the plan.
	public abstract bool SatisfiesSuccessCondition();
	
	// execute the action that belongs to this plan
	public abstract void ExecuteStep();
	
	#region Plan Execution
	
	public Stack<Plan> GetExecutionStack()
	{
		Stack<Plan> stack = new Stack<Plan>();
		addToStack(stack, this);
		
		return stack;
	}
	void addToStack(Stack<Plan> s, Plan p)
	{
		s.Push(p);
		
		if(p.SubPlans == null) return;
		
		for(int i = 0; i < p.SubPlans.Count; i++)
		{
			addToStack(s, p.SubPlans[i]);
		}
	}
	
	protected Stack<Plan> stack;
	Plan p;
	bool planStarted = false;
	
	public bool ExecutePlan()	
	{
		if(!planStarted) startPlan();
		
		bool success = stepPlan();
		
		if(!success) endPlan();
		
		return success;
	}
	
	void startPlan()
	{
		stack = GetExecutionStack();
		p = stack.Pop();
		planStarted = true;
	}
	bool stepPlan()
	{
		// if a plan's success condition is satisfied from the beginning, it won't be executed at all
		if(!p.SatisfiesSuccessCondition())
		{
			if(p.SatisfiesTerminationCondition())
				return false;
			
			p.ExecuteStep();
		}
		
		// a check to see if we can go to the next step in the plan
		// while the condition of the current plan is satisfied, set next plan to current plan
		while (p.SatisfiesSuccessCondition())
		{
			if(stack.Count <= 0) 
				return false;
			
			p = stack.Pop();
			
			if(p.SatisfiesTerminationCondition())
				return false;
			
			p.ExecuteStep();
		}
		
		return true;
	}
	void endPlan()
	{
		planStarted = false;
		stack.Clear();
		stack = null;
		p = null;
	}
	
	#endregion
}