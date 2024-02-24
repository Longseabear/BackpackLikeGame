using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePhaseManager : MonoBehaviour
{

    public static GamePhaseManager Inst { get; private set; }

    void Awake() => Inst = this;

    public GamePhase[] phases;
    
    private GamePhase currentPhase = null;
    private int currentPhaseIndex = 0;
    void Start()
    {
        if(phases.Length > 0) {
            currentPhase = phases[currentPhaseIndex];
        }
    }

    void NextPhase() {
        if (phases.Length > 0) {
            currentPhaseIndex = (currentPhaseIndex + 1) % phases.Length;
            currentPhase = phases[currentPhaseIndex];
        }
    }

    void Update()
    {
        switch (currentPhase.state) {
            case PhaseState.Ready:
                currentPhase.Enter();
                break;
            case PhaseState.Running:
                currentPhase.Execute();
                break;
            case PhaseState.End:
                currentPhase.Exit();
                NextPhase();
                break;
        }   
    }
}
