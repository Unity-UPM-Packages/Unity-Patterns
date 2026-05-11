# FSM Architecture (Finite State Machine) - TheLegends Unity Patterns

This FSM (Finite State Machine) system is an extremely lightweight and independent (Pure C#) Design Pattern provided in the `Unity-Patterns` package. It is designed with a **Zero GC** (Zero Garbage Collection) and high-performance criteria, suitable for managing AI states, Player states, Level Flow, or UI flow.

## 1. Core Components

The system includes two core components located in the `TheLegends.Base.FSM` namespace:

### `IState<T>`
An interface representing a State. Any State needs to implement the following lifecycle methods:
*   `OnEnter(T context)`: Called exactly once when the state machine transitions to this state.
*   `OnUpdate(T context)`: Called every frame (usually from a MonoBehaviour's Update).
*   `OnFixedUpdate(T context)`: Called every physics frame (usually from a MonoBehaviour's FixedUpdate).
*   `OnLateUpdate(T context)`: Called at the end of every frame (usually from a MonoBehaviour's LateUpdate).
*   `OnExit(T context)`: Called exactly once when the state machine transitions to another state (or is terminated).

The parameter `T context` is the owner of the FSM (e.g., a `PlayerController` component). Through this, the State can interact with the owner without casting or passing data back and forth, eliminating Boxing/Unboxing.

### `StateMachine<T>`
The central controller that holds the current state (`CurrentState`) and the Context.
*   `ChangeState(IState<T> newState)`: Stops the current state (calls `OnExit`) and transitions to the new state (calls `OnEnter`).
*   `Update()`, `FixedUpdate()`, `LateUpdate()`: Methods used to pass Unity loop calls to the current state.

## 2. Usage Guide

### Step 1: Define Context and States
The Context can be any class (usually a `MonoBehaviour` representing the main object).

```csharp
using UnityEngine;
using TheLegends.Base.FSM;

// 1. Context Class
public class EnemyController : MonoBehaviour
{
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    // State instances can be pre-initialized to avoid memory allocation (Zero GC)
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }

    public Transform Target;
    public float Speed = 5f;

    private void Awake()
    {
        // Initialize StateMachine, passing 'this' (EnemyController) as context
        StateMachine = new StateMachine<EnemyController>(this);

        // Initialize States
        IdleState = new EnemyIdleState();
        ChaseState = new EnemyChaseState();
    }

    private void Start()
    {
        // Start with Idle state
        StateMachine.ChangeState(IdleState);
    }

    // Pass Unity loops to StateMachine
    private void Update() => StateMachine.Update();
    private void FixedUpdate() => StateMachine.FixedUpdate();
    private void LateUpdate() => StateMachine.LateUpdate();
}
```

### Step 2: Implement IState<T>
Create classes implementing the `IState<EnemyController>` interface.

```csharp
using TheLegends.Base.FSM;
using UnityEngine;

// Idle State
public class EnemyIdleState : IState<EnemyController>
{
    private float waitTime;

    public void OnEnter(EnemyController context)
    {
        waitTime = 2f;
        Debug.Log("Enter Idle State");
    }

    public void OnUpdate(EnemyController context)
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0 && context.Target != null)
        {
            // Transition to Chase state
            context.StateMachine.ChangeState(context.ChaseState);
        }
    }

    public void OnFixedUpdate(EnemyController context) {}
    public void OnLateUpdate(EnemyController context) {}
    public void OnExit(EnemyController context) 
    {
        Debug.Log("Exit Idle State");
    }
}

// Chase State
public class EnemyChaseState : IState<EnemyController>
{
    public void OnEnter(EnemyController context)
    {
        Debug.Log("Enter Chase State");
    }

    public void OnUpdate(EnemyController context)
    {
        if (context.Target == null)
        {
            context.StateMachine.ChangeState(context.IdleState);
            return;
        }

        // Move towards target
        context.transform.position = Vector3.MoveTowards(
            context.transform.position, 
            context.Target.position, 
            context.Speed * Time.deltaTime
        );
    }

    public void OnFixedUpdate(EnemyController context) {}
    public void OnLateUpdate(EnemyController context) {}
    public void OnExit(EnemyController context) {}
}
```

## 3. Why choose this architecture?

*   **No MonoBehaviour Dependency**: The FSM can be used for normal C# objects, not necessarily Unity Objects.
*   **Zero GC (Garbage Collection) at Runtime**: The Context is passed as a Generic `<T>`, and States are instantiated only once during Awake. Thanks to this, the `ChangeState` process creates absolutely no garbage memory, ensuring smooth FPS for Mobile Games.
*   **Easy to Extend and Test**: The logic of each State is clearly isolated, making it easy to Unit Test these logic blocks without the Unity Engine (using NUnit with EditMode tests).
