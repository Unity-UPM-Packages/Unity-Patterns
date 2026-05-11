# Factory - TheLegends Unity Patterns

The Factory architecture provides convenient tools to separate object creation logic from object usage logic, adhering to the **Dependency Inversion** and **Open/Closed** principles. Our system provides both the standard Factory Method (Gang of Four) for pure C# code, and a specialized, convenient `PrefabFactory` library for the Unity environment.

## 1. Key Features

*   **Decoupling:** The consuming classes (Player, Manager, Enemy) do not need to know the details of the `Instantiate` or `new` process. They only communicate through a common interface (`IFactory`).
*   **Perfect Unity Compatibility:** `PrefabFactory<T>` allows you to leverage the power of Prefabs via the Inspector without generating unnecessary C# classes for each object type.
*   **Easy Maintenance & Object Pool Upgrades:** Changing the object generation mechanism (from `Instantiate` to taking from a reusable `Object Pool`) can be done entirely hidden beneath the Factory without breaking thousands of lines of game logic.
*   **Centralized Setup:** All initial setup data (health, speed, color changes) is encapsulated in one place within the Create function, cleaning up copy-pasted code scattered throughout the project.

## 2. Usage Guide for Unity GameObjects

### Scenario A: Simple Prefab Initialization (Fast & Lightweight)
If you only need to spawn an object from a Prefab without complex input data logic, you don't need to create a new script file. Use the built-in `PrefabFactory<T>`:

```csharp
using UnityEngine;
using TheLegends.Base.Factory;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private ArrowView _arrowPrefab;
    [SerializeField] private Transform _spawnPoint;
    
    // Use IFactory Interface to easily switch to Object Pool later
    private IFactory<ArrowView> _factory;

    private void Start()
    {
        // Initialize the factory
        _factory = new PrefabFactory<ArrowView>(_arrowPrefab, _spawnPoint);
    }

    public void Fire()
    {
        // Clean method call, ArrowManager doesn't know how Unity Instantiate works
        ArrowView arrow = _factory.Create();
        arrow.Shoot();
    }
}
```

### Scenario B: Initialization with Parameters (Custom Factory)
Use this when you need to configure the object (pass Damage, Color, Level parameters, etc.) to inject data into the object as soon as it is spawned.

**Step 1: Create Data Structure**
```csharp
public struct ArrowSetupData
{
    public float Speed;
    public int Damage;
    public Transform SpawnPosition;
}
```

**Step 2: Write Custom Factory inheriting from parameterized IFactory**
```csharp
using UnityEngine;
using TheLegends.Base.Factory;

public class ArrowViewFactory : IFactory<ArrowSetupData, ArrowView>
{
    private ArrowView _prefab;

    public ArrowViewFactory(ArrowView prefab)
    {
        _prefab = prefab;
    }

    // Spawn logic and data injection encapsulated here
    public ArrowView Create(ArrowSetupData param)
    {
        ArrowView instance = Object.Instantiate(_prefab, param.SpawnPosition.position, param.SpawnPosition.rotation);
        
        // Call initialization methods of ArrowView
        instance.SetSpeed(param.Speed);
        instance.SetDamage(param.Damage);
        
        return instance;
    }
}
```

## 3. Applying Factory Method Pattern (Gang of Four)

For **Pure C#** logic or core data management systems, you can use the abstract base `Creator<TProduct>` to build standard object-oriented structures (Subclassing to decide which object to create).

```csharp
using UnityEngine;
using TheLegends.Base.Factory;

// 1. Product Interface
public interface IWeapon { void Attack(); }

// 2. Concrete Products
public class Sword : IWeapon { public void Attack() { Debug.Log("Slash with sword!"); } }
public class Bow : IWeapon { public void Attack() { Debug.Log("Shoot with bow!"); } }

// 3. Creators
public class SwordCreator : Creator<IWeapon>
{
    // Subclass decides exactly which class is created
    protected override IWeapon FactoryMethod() => new Sword();
}

public class BowCreator : Creator<IWeapon>
{
    protected override IWeapon FactoryMethod() => new Bow();
}
```

The advantage of this architecture is that you can call the `creator.GetProduct()` function to get the Product and allow the `Creator` to insert additional core business logic before and after the Product is created, a highly useful technique for complex software design projects.
