using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : IState
{
    protected HashSet<ITransition> _transitions { get; }
    public IReadOnlyCollection<ITransition> Transitions => _transitions;
    // Заменить ReadOnly на IEnumerable

    public BaseState() => _transitions = new HashSet<ITransition>();
    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void OnExit() { }

    public void AddTransition(IState to, IPredicate condition) => _transitions.Add(new Transition(to, condition));
}
