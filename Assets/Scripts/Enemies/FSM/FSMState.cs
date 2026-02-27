using System.Xml.Serialization;
using UnityEngine;

public abstract class FSMState
{
    protected NPCController npc;

    public FSMState(NPCController npc)
    {
        this.npc = npc;
    }

    public abstract void Enter();
    public abstract void Update();

    public abstract void FixedUpdate();
    public abstract void Exit();
}