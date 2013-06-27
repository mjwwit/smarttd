using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// BDI implementation based on wikipedia:
// https://en.wikipedia.org/wiki/Belief%E2%80%93desire%E2%80%93intention_software_model#BDI_agent_implementations

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
public abstract class Plan//<T> where T : Unit // not sure which type (or interface?) we should set here, want to keep it as generic as possible
{
	// the subplans required for completing this plan's goals
	public List<Plan> SubPlans;
	
	// return true if the plan has an executable action defined
	public bool HasAction;
	
	// return true if the current situation satisfies the pre-condition of this plan
	public abstract bool SatisfiesPreCondition();
	
	// return true if the current situation satisfies the invocation condition ("trigger event")
	public abstract bool SatisfiesInvocationCondition();
	
	// execute the action that belongs to this plan
	public abstract void ExecuteAction();
}

public class BDI_Agent
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
	Unit me;
	List<Unit> friends;
	List<Tower> enemies;
	
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
	
	public void Update()
	{
		// option generator
		// read event queue ( or current status ) and return a list of options
		// options generated will be a series of possible positions to move to, based on information about the world
		// ie up, down, left, right
		
		// select a subset of options to be adopted
		
		// add options to intention structure
		
		// perform any atomic intentions in the structure (an action, ie. move in direction x,y)
		
		// add external events (internal events should be added as they occur)
		
		// drop successful desires and satisfied intentions
		// drop impossible desires and unrealizable intentions
	}
}
