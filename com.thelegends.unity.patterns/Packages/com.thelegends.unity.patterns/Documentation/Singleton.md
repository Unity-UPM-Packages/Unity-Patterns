# Singleton - TheLegends Unity Patterns

The Singleton architecture provided in this package is a Pure C# class, independent of `MonoBehaviour`. This Singleton system provides a Thread-safe solution and a clear Lifecycle mechanism that makes initialization and memory cleanup easy to control.

## 1. Key Features

*   **Thread-Safe:** Uses a Double-Check Locking mechanism via `lock (typeof(T))`, ensuring that the instance is created exactly once even when multiple threads call `Instance` simultaneously.
*   **Not tied to Unity Hierarchy:** Avoids complex issues related to scene load/unload. Suitable for Data Managers, Services, or Network Managers.
*   **Lifecycle Management:** Provides `virtual` methods like `OnInitializing()`, `OnInitialized()`, `ClearSingleton()` to give you a safe place to setup or cleanup data (especially useful when you need to reset a level or do Unit Testing).

## 2. Usage Guide

### Step 1: Initialize a Singleton class
To turn a class into a Singleton, you simply need to inherit from `Singleton<T>` (under the `TheLegends.Base.UnitySingleton` namespace).

*Note: Since this class has a `new()` constraint, you must have a parameterless constructor (C# creates a default one if you don't define any constructor).*

```csharp
using UnityEngine;
using TheLegends.Base.UnitySingleton;

public class GameDataManager : Singleton<GameDataManager>
{
    public int PlayerScore { get; private set; }

    // Called just before the state changes to Initialized
    protected override void OnInitializing()
    {
        Debug.Log("GameDataManager is initializing...");
        PlayerScore = 0;
    }

    // Called after initialization is complete
    protected override void OnInitialized()
    {
        Debug.Log("GameDataManager initialized successfully!");
    }

    public void AddScore(int amount)
    {
        PlayerScore += amount;
    }

    // Called when the Singleton is destroyed (DestroyInstance)
    public override void ClearSingleton()
    {
        Debug.Log("Clearing GameDataManager data");
        PlayerScore = 0;
    }
}
```

### Step 2: Access and usage
Anywhere in the project, simply call `YourClass.Instance` to get the object.

```csharp
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // If GameDataManager hasn't been initialized, it will be automatically created
        // and call OnInitializing / OnInitialized methods.
        int score = GameDataManager.Instance.PlayerScore;
        Debug.Log("Current Score: " + score);
    }

    public void OnEnemyKilled()
    {
        GameDataManager.Instance.AddScore(10);
    }
}
```

## 3. Advanced APIs

Besides `Instance`, the Singleton base class also supports APIs to help you proactively manage the lifecycle:

*   **`IsInitialized`**: Returns `true` if this Singleton has passed the initialization step.
*   **`InitializeSingleton()`**: This method is called automatically the first time `.Instance` is called. You don't need to call it manually unless you want to force the system to initialize the object at a specific time (e.g., during a loading screen).
*   **`DestroyInstance()`**: Destroys the current Singleton and sets `Instance = null`. During destruction, your `ClearSingleton()` method will be called. This feature is extremely important when you want to **Completely Reset Game State** (e.g., player clicks "Play Again").
*   **`CreateInstance()`**: Destroys the current Instance (if any) via `DestroyInstance()` and automatically creates a new Instance immediately.
