using System.Collections.Generic;
public interface IState
{    
    public IReadOnlyCollection<ITransition> Transitions { get; } 
    void OnEnter();
    void Update();
    void FixedUpdate();
    void LateUpdate();
    void OnExit();

    void AddTransition(IState to, IPredicate condition);
}

