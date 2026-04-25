# Singleton - TheLegends Unity Patterns

Kiến trúc Singleton được cung cấp trong package này là một class C# thuần (Pure C#), không phụ thuộc vào `MonoBehaviour`. Hệ thống Singleton này cung cấp giải pháp an toàn luồng (Thread-safe) và cơ chế Lifecycle rõ ràng giúp việc khởi tạo và dọn dẹp bộ nhớ trở nên dễ kiểm soát.

## 1. Ưu điểm nổi bật

*   **Thread-Safe (An toàn đa luồng):** Sử dụng cơ chế khóa đúp (Double-Check Locking) thông qua `lock (typeof(T))`, đảm bảo rằng instance chỉ được tạo ra đúng một lần ngay cả khi có nhiều luồng cùng gọi tới `Instance` ở cùng một thời điểm.
*   **Không gắn với Unity Hierarchy:** Tránh được các vấn đề phức tạp liên quan tới scene load/unload. Thích hợp cho Data Manager, Service, hoặc Network Manager.
*   **Lifecycle Management (Quản lý vòng đời):** Cung cấp các hàm ảo (`virtual`) như `OnInitializing()`, `OnInitialized()`, `ClearSingleton()` giúp bạn có nơi an toàn để thiết lập hoặc dọn dẹp dữ liệu (đặc biệt hữu ích khi cần reset lại màn chơi hoặc Unit Test).

## 2. Hướng dẫn sử dụng

### Bước 1: Khởi tạo một class Singleton
Để biến một class thành Singleton, bạn chỉ cần cho nó kế thừa từ `Singleton<T>` (thuộc namespace `TheLegends.Base.UnitySingleton`). 

*Lưu ý: Do class này có constraint `new()`, bạn bắt buộc phải có một constructor không tham số (mặc định C# tự tạo nếu bạn không định nghĩa constructor nào).*

```csharp
using UnityEngine;
using TheLegends.Base.UnitySingleton;

public class GameDataManager : Singleton<GameDataManager>
{
    public int PlayerScore { get; private set; }

    // Gọi ngay trước khi trạng thái chuyển sang Initialized
    protected override void OnInitializing()
    {
        Debug.Log("GameDataManager đang được khởi tạo...");
        PlayerScore = 0;
    }

    // Gọi sau khi quá trình khởi tạo hoàn tất
    protected override void OnInitialized()
    {
        Debug.Log("GameDataManager khởi tạo thành công!");
    }

    public void AddScore(int amount)
    {
        PlayerScore += amount;
    }

    // Được gọi khi Singleton bị hủy bỏ (DestroyInstance)
    public override void ClearSingleton()
    {
        Debug.Log("Xóa bỏ dữ liệu của GameDataManager");
        PlayerScore = 0;
    }
}
```

### Bước 2: Truy xuất và sử dụng
Bất cứ nơi nào trong project, bạn chỉ cần gọi `ClassCuaBan.Instance` để lấy ra object.

```csharp
public class UIManager : MonoBehaviour
{
    private void Start()
    {
        // Nếu GameDataManager chưa được khởi tạo, nó sẽ tự động được tạo ra mới 
        // và gọi các hàm OnInitializing / OnInitialized.
        int score = GameDataManager.Instance.PlayerScore;
        Debug.Log("Score hiện tại: " + score);
    }

    public void OnEnemyKilled()
    {
        GameDataManager.Instance.AddScore(10);
    }
}
```

## 3. Các API nâng cao

Ngoài `Instance`, Singleton base class còn hỗ trợ các API giúp bạn chủ động quản lý vòng đời:

*   **`IsInitialized`**: Trả về `true` nếu Singleton này đã chạy qua bước khởi tạo.
*   **`InitializeSingleton()`**: Hàm này được tự động gọi khi lần đầu tiên gọi `.Instance`. Bạn không cần gọi tay trừ khi bạn muốn ép hệ thống khởi tạo object vào một thời điểm cụ thể (VD: lúc loading screen).
*   **`DestroyInstance()`**: Phá hủy Singleton hiện tại và đặt `Instance = null`. Trong quá trình phá hủy, hàm `ClearSingleton()` của bạn sẽ được gọi. Tính năng này vô cùng quan trọng khi bạn muốn **Reset hoàn toàn Game State** (ví dụ người chơi ấn nút "Play Again").
*   **`CreateInstance()`**: Phá hủy Instance hiện tại (nếu có) bằng `DestroyInstance()` và tự động tạo mới một Instance ngay lập tức.
