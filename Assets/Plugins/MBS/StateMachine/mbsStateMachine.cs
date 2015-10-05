using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MBS
{
public class mbsStateMachine<T> {
	T currentState;
	Action currentAction;

	public Dictionary<T, Action>			StateFunctions;
	
	public mbsStateMachine() {
		StateFunctions = new Dictionary<T, Action>();
	}
		
	public T CurrentState	{ get {	return currentState;	} }
		
	public bool PerformAction() {
		return PerformAction(currentState);
	}
	
	public bool PerformAction(T action) {
		int a = (int)Convert.ChangeType(action, typeof(int));
		int c = (int)Convert.ChangeType(currentState, typeof(int));

		if (a != c)
			SetState(action);

		if (null != currentAction)
			currentAction();
			
		return true;
	}
	
	public bool CompareState(T state) {
		int s = (int)Convert.ChangeType(state, typeof(int));
		int c = (int)Convert.ChangeType(currentState, typeof(int));
		return s == c;
	}
	
	virtual public bool SetState(T to) {
		if (StateFunctions.ContainsKey(to)) {
			currentState = to;
			currentAction = StateFunctions[currentState];
			return true;
		}
		Debug.Log("Cannot find state " + to);
		return false;
	}
	
	public void AddState(T statename, Action function = null) {
		if (null == statename) {
			return;
		}
			
		if (StateFunctions.ContainsKey(statename)) {
			StateFunctions[ statename ] = function;
		} else {
			StateFunctions.Add(statename, function);

			if (null == currentAction) 
				SetState(statename);
		}
	}
	
	public bool RemoveState(T statename) {
		if (StateFunctions.ContainsKey(statename)) {
			StateFunctions.Remove( statename );
			return true;
		}
		return false;
	}
}
}
