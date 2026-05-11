# Object Pool Pattern

The Object Pool pattern allows you to reuse objects instead of constantly instantiating and destroying them. This eliminates Garbage Collection (GC) spikes and improves performance, especially when dealing with frequent creations (e.g., bullets, enemies, particles).

Our implementation acts as a robust wrapper around `UnityEngine.Pool` and is completely decoupled from any specific creation patterns (like Factory). It uses standard C# delegates (`Func<T>`) or direct prefab references to create new instances when needed.

## 1. Defining a Poolable Object

For objects that need to reset their state when reused, implement the `IPoolable` interface.

```csharp
using UnityEngine;
using TheLegends.Base.Pool;

public class Bullet : MonoBehaviour, IPoolable
{
    private float _speed = 10f;

    // Called automatically when the object is pulled from the pool
    public void OnSpawn()
    {
        // Reset state, set velocity, play spawn effects
        Debug.Log("Bullet Spawned");
    }

    // Called automatically when the object is returned to the pool
    public void OnDespawn()
    {
        // Clear references, reset physics
        Debug.Log("Bullet Despawned");
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
    }
}
```

## 2. Using `ComponentPool<T>` for Unity Objects

For Unity MonoBehaviours, use `ComponentPool<T>`. It automatically handles `gameObject.SetActive(true/false)` for you.

### Approach A: Using a Prefab (Simplest)
This is the fastest way to set up a pool for a Unity prefab.

```csharp
using UnityEngine;
using TheLegends.Base.Pool;

public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private ComponentPool<Bullet> _bulletPool;

    private void Awake()
    {
        // Pass the prefab directly. The pool handles instantiation automatically.
        _bulletPool = new ComponentPool<Bullet>(bulletPrefab, parent: transform);
    }

    public void Fire()
    {
        // ComponentPool automatically sets it active and calls OnSpawn()
        Bullet bullet = _bulletPool.Get();
        
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        
        // ... return to pool later via _bulletPool.Release(bullet);
    }
}
```

### Approach B: Integrating with a Factory
If you have complex creation logic encapsulated in a Factory, you can pass the Factory's `Create` method into the pool as a delegate. This keeps the Pool completely decoupled from the Factory interface.

```csharp
using UnityEngine;
using TheLegends.Base.Factory;
using TheLegends.Base.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private ComponentPool<Bullet> _bulletPool;

    private void Awake()
    {
        // 1. Create your complex factory
        IFactory<Bullet> factory = new PrefabFactory<Bullet>(bulletPrefab);

        // 2. Pass the factory.Create method (Func delegate) into the pool
        _bulletPool = new ComponentPool<Bullet>(factory.Create);
    }
}
```

## 3. Using `GenericPool<T>` for Pure C# Classes

If you are pooling standard C# classes (to avoid GC allocations), use `GenericPool<T>` and provide a `Func<T>` to instantiate the object.

```csharp
using TheLegends.Base.Pool;

// A pure C# class
public class DamageEvent : IPoolable
{
    public int Damage;
    public string Source;

    public void OnSpawn() { }
    
    public void OnDespawn() 
    {
        Damage = 0;
        Source = null;
    }
}

public class DamageManager
{
    private GenericPool<DamageEvent> _eventPool;

    public DamageManager()
    {
        // Provide a simple lambda to create new instances when the pool is empty
        _eventPool = new GenericPool<DamageEvent>(() => new DamageEvent());
    }

    public void ProcessDamage()
    {
        DamageEvent e = _eventPool.Get();
        e.Damage = 50;
        e.Source = "Fireball";
        
        // ... process ...

        _eventPool.Release(e);
    }
}
```
