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
	//Beliefs beliefs;
	
	// Desires / goals:
	/*
	- Reach the end
		o Move towards goal
		o Stay alive
			* Take as little damage as possible
			* Become more reckless as time progresses
		o Help others stay alive (others can help you stay alive)
	- Help others stay alive (more units at the end is good!)
	 */
	
	// Intentions
	// Plans
	// - Move to position (x, y)
	// - ?? special actions ??
	// Intentions are represented by an hierarchical structure of plans. 
	// The SubPlans list in the Plan class represents this structure!
	List<Plan> PossiblePlans;
	List<Plan> ConsideredPlans;
	List<Plan> CommittedPlans;
	
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
		PossiblePlans = new List<Plan>(availablePlans());
		ConsideredPlans = new List<Plan>();
		CommittedPlans = new List<Plan>();
	}
	
	protected abstract Plan[] availablePlans();
	
	public virtual void Update()
	{
		// option generator
		// read event queue ( or current status ) and return a list of options
		
		ConsideredPlans.Clear();
		
		// select a subset of options to be adopted
		foreach(Plan p in PossiblePlans)
		{
			if(p.SatisfiesPreCondition()
				&& !CommittedPlans.Contains(p))
			{
				ConsideredPlans.Add(p);
			}
		}
		
		foreach(Plan p in ConsideredPlans)
		{
			if(p.SatisfiesInvocationCondition())
			{
				// enforce consistency
				// -> do some decision making on which plan(s) to commit to
				
				CommittedPlans.Add(p);
			}
		}
		
		for(int i = CommittedPlans.Count-1; i >= 0; --i)
		{
			Plan p = CommittedPlans[i];
			
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
}
