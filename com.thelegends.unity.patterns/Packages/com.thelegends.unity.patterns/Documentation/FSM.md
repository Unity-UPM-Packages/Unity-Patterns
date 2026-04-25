# Kiến trúc FSM (Finite State Machine) - TheLegends Unity Patterns

Hệ thống FSM (Finite State Machine) này là một Design Pattern cực kỳ nhẹ và độc lập (Pure C#) được cung cấp trong gói `Unity-Patterns`. Nó được thiết kế với tiêu chí **Zero GC** (Không tạo rác bộ nhớ) và hiệu năng cao, thích hợp cho việc quản lý trạng thái của AI, Player, Flow của một màn chơi, hoặc luồng giao diện UI.

## 1. Các thành phần chính

Hệ thống bao gồm hai thành phần cốt lõi nằm trong namespace `TheLegends.Base.FSM`:

### `IState<T>`
Là một interface đại diện cho một Trạng thái. Bất kỳ Trạng thái nào cũng cần phải implement các phương thức vòng đời sau:
*   `OnEnter(T context)`: Gọi một lần duy nhất khi máy trạng thái chuyển sang trạng thái này.
*   `OnUpdate(T context)`: Gọi mỗi frame (thường từ hàm Update của MonoBehaviour).
*   `OnFixedUpdate(T context)`: Gọi mỗi frame vật lý (thường từ hàm FixedUpdate của MonoBehaviour).
*   `OnLateUpdate(T context)`: Gọi cuối mỗi frame (thường từ hàm LateUpdate của MonoBehaviour).
*   `OnExit(T context)`: Gọi một lần duy nhất khi máy trạng thái chuyển sang trạng thái khác (hoặc bị kết thúc).

Tham số `T context` là chủ thể (Owner) của FSM (ví dụ: một component `PlayerController`), qua đó State có thể thao tác với chủ thể mà không cần ép kiểu hay truyền dữ liệu qua lại, giúp loại bỏ Boxing/Unboxing.

### `StateMachine<T>`
Là bộ điều khiển trung tâm lưu giữ trạng thái hiện tại (`CurrentState`) và Context.
*   `ChangeState(IState<T> newState)`: Dừng trạng thái hiện tại (gọi `OnExit`) và chuyển sang trạng thái mới (gọi `OnEnter`).
*   `Update()`, `FixedUpdate()`, `LateUpdate()`: Các phương thức dùng để truyền lời gọi vòng lặp từ Unity tới trạng thái hiện tại.

## 2. Hướng dẫn sử dụng

### Bước 1: Định nghĩa Context và các States
Context có thể là bất kỳ class nào (thường là một `MonoBehaviour` đại diện cho đối tượng chính).

```csharp
using UnityEngine;
using TheLegends.Base.FSM;

// 1. Lớp Context
public class EnemyController : MonoBehaviour
{
    public StateMachine<EnemyController> StateMachine { get; private set; }
    
    // Các State instances có thể khởi tạo sẵn để tránh tạo rác (Zero GC)
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }

    public Transform Target;
    public float Speed = 5f;

    private void Awake()
    {
        // Khởi tạo StateMachine, truyền this (EnemyController) làm context
        StateMachine = new StateMachine<EnemyController>(this);

        // Khởi tạo các State
        IdleState = new EnemyIdleState();
        ChaseState = new EnemyChaseState();
    }

    private void Start()
    {
        // Bắt đầu với trạng thái Idle
        StateMachine.ChangeState(IdleState);
    }

    // Pass các vòng lặp Unity cho StateMachine
    private void Update() => StateMachine.Update();
    private void FixedUpdate() => StateMachine.FixedUpdate();
    private void LateUpdate() => StateMachine.LateUpdate();
}
```

### Bước 2: Triển khai IState<T>
Tạo các class implement interface `IState<EnemyController>`.

```csharp
using TheLegends.Base.FSM;
using UnityEngine;

// Trạng thái đứng yên
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
            // Chuyển sang trạng thái Chase
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

// Trạng thái truy đuổi
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

        // Di chuyển tới mục tiêu
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

## 3. Tại sao chọn kiến trúc này?

*   **Không phụ thuộc MonoBehaviour**: FSM có thể được dùng cho C# objects thông thường, không nhất thiết phải là Unity Objects.
*   **Zero GC (Garbage Collection) khi chạy**: Context được truyền dưới dạng Generic `<T>`, các State được khởi tạo một lần duy nhất lúc Awake. Nhờ vậy, quá trình ChangeState diễn ra hoàn toàn không tạo thêm bộ nhớ rác, đảm bảo FPS mượt mà cho Mobile Game.
*   **Dễ mở rộng và Test**: Logic từng State được cô lập rõ ràng giúp dễ dàng Unit Test các logic này mà không cần Unity Engine (sử dụng NUnit với EditMode tests).
