using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class StateMachine
{
    public IState currentState;

    Dictionary<Type, IState> states;

    HashSet<ITransition> anyTransitions;

    public StateMachine(IState initialState)
    {
        currentState = initialState;

        anyTransitions = new HashSet<ITransition>();
        states = new Dictionary<Type, IState>
        {
            { initialState.GetType(), initialState }
        };


        currentState.OnEnter();
    }

    public void Update()
    {
        ITransition transition = GetTransition();

        if (transition != null)
            ChangeState(transition.To);

        currentState?.Update();

    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void LateUpdate()
    {
        currentState?.LateUpdate();
    }
    public void ChangeState(IState state)
    {
        if (state == currentState) return;

        currentState?.OnExit();
        state?.OnEnter();
        currentState = state;
    }

    private ITransition GetTransition()
    {
        foreach (var transition in anyTransitions)
            if (transition.Condition.Evaluate())
                return transition;

        foreach (var transition in currentState.Transitions)
            if (transition.Condition.Evaluate())
                return transition;

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddState(from).AddTransition(GetOrAddState(to), condition);

    }
    public void AddAnyTransition(IState to, IPredicate condition)
    {
        anyTransitions.Add(new Transition(GetOrAddState(to), condition));
    }


    private IState GetOrAddState(IState state)
    {
        IState temp = states.GetValueOrDefault(state.GetType());

        if (temp == null)
        {
            states.Add(state.GetType(), state);
            temp = state;
        }

        return temp;
    }

}
