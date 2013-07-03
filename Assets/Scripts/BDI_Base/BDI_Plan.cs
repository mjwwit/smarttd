using System;
using System.Collections.Generic;
using UnityEngine;

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

public enum PlanType
{
	Movement = 1,
	//Type2 = 2,
	//Type3 = 4,
	//Type4 = 8,
	//Type5 = 16,
	//Type6 = 32,
	//Type7 = 64,
	//Type8 = 128,
	// etc.
}

public abstract class BDI_Plan
{
	static int idCounter = 0;
	public readonly int ID;
	public BDI_Plan()
	{
		ID = idCounter++;
		Type = 0;
	}
	
	#region Plan Type
	
	public static bool PlanTypeOverlaps(BDI_Plan a, BDI_Plan b)
	{
		return PlanTypeOverlaps(a.Type, b.Type);
	}
	public static bool PlanTypeOverlaps(PlanType a, PlanType b)
	{
		return (a&b)>0;
	}
	public static bool IsPlanType(PlanType source, PlanType compareTo)
	{
		return source == compareTo;
	}
	public static bool HasPlanType(PlanType source, PlanType compareTo)
	{
		return (source & compareTo) == compareTo;
	}
	public static List<PlanType> GetOverlappingPlanTypes(BDI_Plan a, BDI_Plan b)
	{
		return GetOverlappingPlanTypes(a.Type, b.Type);
	}
	public static List<PlanType> GetOverlappingPlanTypes(PlanType a, PlanType b)
	{
		List<PlanType> types = new List<PlanType>();
		PlanType overlap = a&b;
		
		foreach (PlanType pt in Enum.GetValues(typeof(PlanType)))
		{
			if((overlap & pt) == pt)
				types.Add(pt);
		}
		
		return types;
	}
	
	public PlanType Type;
	
	#endregion
	
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
	public List<BDI_Plan> SubPlans;
	
	#region Conditions
	
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
	
	#endregion
	
	// This function should define a heuristic that estimates how much influence this plan has on the end result.
	public abstract float ContributionHeuristic();
	
	// execute once when the plan is started
	protected virtual void StartPlan() { }
	
	// execute the action that belongs to this plan
	public abstract void ExecuteStep();
	
	// execute once when the plan is ended
	protected virtual void EndPlan() { }
	
	#region Plan Execution
	
	public Stack<BDI_Plan> GetExecutionStack()
	{
		Stack<BDI_Plan> stack = new Stack<BDI_Plan>();
		addToStack(stack, this);
		
		return stack;
	}
	void addToStack(Stack<BDI_Plan> s, BDI_Plan p)
	{
		s.Push(p);
		
		if(p.SubPlans == null) return;
		
		for(int i = 0; i < p.SubPlans.Count; i++)
		{
			addToStack(s, p.SubPlans[i]);
		}
	}
	
	protected Stack<BDI_Plan> stack;
	BDI_Plan p;
	bool planStarted = false;
	
	public bool ExecutePlan()	
	{
		if(!planStarted)
		{
			startPlan();
			planStarted = true;
			planEnded = false;
		}
		
		// stepPlan not executed when plan is ended ( && operator )
		bool success = !planEnded && stepPlan();
		
		if(!success)
		{
			endPlan();
			planStarted = false;
		}
		
		return success;
	}
	
	bool planEnded;
	public void Stop()
	{
		planEnded = true;
	}
	
	void startPlan()
	{
		stack = GetExecutionStack();
		p = stack.Pop();
		p.StartPlan();
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
		p.EndPlan();
		stack.Clear();
		stack = null;
		p = null;
	}
	
	#endregion
}