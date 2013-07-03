using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// BDI implementation based on wikipedia:
// https://en.wikipedia.org/wiki/Belief%E2%80%93desire%E2%80%93intention_software_model#BDI_agent_implementations

public abstract class BDI_Agent
{
	// Beliefs / belief base:
	/*
	- Me
		o Unit properties, grid / potential field, optimal path, etc.
	- Other agents (in range)
		o Same as me, but theirs.
	- Turrets (if scouted)
		o Turret properties, updated grid
	- World
		o End goal position (
		o Terrain (implicit with grid)
	*/
	public BDI_Beliefs beliefs;
	
	// Desires / goals:
	/*
	- Reach the end
		o Move towards goal
		o Stay alive
			* Take as little damage as possible
			* Become more reckless as time progresses
		o Help others stay alive (others can help you stay alive)
	- Help others stay alive (more units at the end is good!)
	
	Expressed in beliefs:
	- I'm at the goal.
	- My friends are at the goal.
	
	 */
	
	// Intentions
	// Plans
	// - Move to position (x, y)
	// - ?? special actions ??
	// Intentions are represented by an hierarchical structure of plans. 
	// The SubPlans list in the Plan class represents this structure!
	List<BDI_Plan> PossiblePlans;
	List<BDI_Plan> ConsideredPlans;
	List<BDI_Plan> CommittedPlans;
	
	// Events
	// New information
	// - Tower discovered
	// - Tower changed target to friendly
	// - Tower changed target to me
	// - Friendly unit takes damage
	// - ???
	// Constantly changing: friendly unit position. Not sure how to model/quantify this.
	// We might not have to explicitly model these events, but simply check all the conditions all the time. 
	// It's probably fast enough in our system.
	
	/*
	Algorithm:
	
	Rao, A. S., & Georgeff, M. P. (1995, June). BDI agents: From theory to practice. 
	In Proceedings of the first international conference on multi-agent systems (ICMAS-95) (pp. 312-319).
	
    initialize-state
    repeat
        options: option-generator(event-queue)
        selected-options: deliberate(options)
        update-intentions(selected-options)
        execute()
        get-new-external-events()
        drop-unsuccessful-attitudes()
        drop-impossible-attitudes()
    end repeat

	*/
	
	public BDI_Agent()
	{
		PossiblePlans = new List<BDI_Plan>(GetAvailablePlans());
		ConsideredPlans = new List<BDI_Plan>();
		CommittedPlans = new List<BDI_Plan>();
	}
	
	//public virtual void Start()
	//{
	//	Beliefs = GetBeliefs();
	//}
	
	public abstract void SetBeliefs();
	protected abstract BDI_Plan[] GetAvailablePlans();
	
	protected abstract bool GoalsCompleted();
	
	// currently every frame, but using a coroutine this can easily be changed to some time interval
	public virtual void Update()
	{
		// option generator
		// read event queue ( or current status ) and return a list of options
		// ----->
		//   we update the beliefs and fire events
		//   pre- and triggerconditions of plans might be satisfied after that
		beliefs.Update();
		
		// if our goals are completed, stop the agent
		if(GoalsCompleted())
		{
			StopAgent();
			
			return;
		}
		
		
		ConsideredPlans.Clear();
		
		// select a subset of options to be adopted
		foreach(BDI_Plan p in PossiblePlans)
		{
			if(p.SatisfiesPreCondition()
				&& !CommittedPlans.Contains(p)
				&& !p.SatisfiesSuccessCondition())
			{
				ConsideredPlans.Add(p);
			}
		}
		
		// our considered plans are now all plans that satisfy the precondition
		// additionally, they exclude:
		// - plans we're currently committing to 
		// - plans that have already succeeded before we even start them
		
		foreach(BDI_Plan p in ConsideredPlans)
		{
			if(p.SatisfiesInvocationCondition())
			{
				// enforce consistency
				// -> do some decision making on which plan(s) to commit to
				
				bool addPlan = true;
				List<BDI_Plan> currentlyCommittedPlans = new List<BDI_Plan>(CommittedPlans);
				foreach(BDI_Plan cPlan in currentlyCommittedPlans)
				{
					// check for overlap (but not which types overlap!)
					// todo: take multiple plan types into account! difficulty: high
					// 		    multiple overlapping plans with different possible combinations are possible
					if(BDI_Plan.PlanTypeOverlaps(p, cPlan))
					{
						// select the best plan
						BDI_Plan bestPlan = SolvePlanConflict(p, cPlan);
						
						// if the best plan is the same as our considered plan
						if(bestPlan == p)
						{
							// stop current plan
							cPlan.Stop();
							// remove from committed plans
							CommittedPlans.Remove(cPlan);
						}
						// if the best plan is a current plan
						else
						{
							// don't add the plan
							addPlan = false;
							// quit
							break;
						}
					}
				}
				
				// if we've decided to commit to this plan, add it
				if(addPlan)
				{
					CommittedPlans.Add(p);
					Debug.Log("Agent " + beliefs.MyName() + " committed to a new plan: " + p.GetType());
				}
			}
		}
		
		// the plans we're committing to represent the current goal state of the agent
		
		for(int i = CommittedPlans.Count-1; i >= 0; --i)
		{
			BDI_Plan p = CommittedPlans[i];
			
			// success is a bad variable name
			// the return value indicates if we should continue executing this plan
			bool success = p.ExecutePlan();
			
			if(!success)
				CommittedPlans.RemoveAt(i);
		}
		
		// we need some kind of way to enforce consistency in our actions:
		// "... desires and intentions have to be closed under implication and have to be consistent. "
		
		// add options to intention structure
		
		// perform any atomic intentions in the structure (an action, ie. move in direction x,y)
		
		// add external events (internal events should be added as they occur)
		
		// drop successful desires and satisfied intentions
		// drop impossible desires and unrealizable intentions
	}
	
	// returns the best of the two plans, based on their heuristic functions
	BDI_Plan SolvePlanConflict(BDI_Plan a, BDI_Plan b)
	{
		float aValue = a.ContributionHeuristic();
		float bValue = b.ContributionHeuristic();
		
		if(aValue > bValue) return a;
		else return b;
	}
	
	public virtual void StopAgent()
	{
		foreach(BDI_Plan p in CommittedPlans)
		{
			p.Stop();
		}
		CommittedPlans.Clear();
	}
}
