
using UnityEngine;
using System;
public enum PhaseState
{
    Ready, Running, End
}
public abstract class GamePhase
{
    public PhaseState state;

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}

