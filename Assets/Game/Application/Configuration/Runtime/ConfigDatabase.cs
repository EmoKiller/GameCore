using System.Collections.Generic;
using Game.Application.Configuration.Abstractions;

namespace Game.Application.Configuration.Runtime
{
    /// <summary>
    /// Cung cấp quyền truy cập Read-only vào toàn bộ cấu hình của trò chơi.
    /// Đây là lớp mà các Features sẽ sử dụng để lấy dữ liệu.
    /// </summary>
    public sealed class ConfigDatabase
    {
        private readonly IConfigRegistry _registry;

        public ConfigDatabase(IConfigRegistry registry)
        {
            _registry = registry;
        }

        // --- TRUY XUẤT ĐƠN LẺ ---

        /// <summary> Lấy Config theo ID (Dùng int trực tiếp để linh hoạt) </summary>
        public T Get<T>(int id) where T : IConfig
        {
            return _registry.Get<T>(id);
        }

        /// <summary> Thử lấy Config, trả về false nếu không tìm thấy </summary>
        public bool TryGet<T>(int id, out T config) where T : IConfig
        {
            return _registry.TryGet(id, out config);
        }

        // --- TRUY XUẤT DANH SÁCH ---

        /// <summary> Lấy toàn bộ danh sách Config của một loại (Ví dụ: Tất cả Item trong Shop) </summary>
        public IReadOnlyCollection<T> GetAll<T>() where T : IConfig
        {
            return _registry.GetAll<T>();
        }

        // --- KIỂM TRA TRẠNG THÁI ---

        /// <summary> Kiểm tra xem một ID cụ thể đã được nạp hay chưa </summary>
        public bool Contains<T>(int id) where T : IConfig
        {
            return _registry.Contains<T>(id);
        }

        /// <summary> Kiểm tra xem loại Config T này đã được đăng ký trong hệ thống chưa </summary>
        public bool IsLoaded<T>() where T : IConfig
        {
            return _registry.IsRegistered<T>();
        }

        /// <summary> Trả về tổng số lượng config hiện có của loại T </summary>
        public int Count<T>() where T : IConfig
        {
            return _registry.GetCount<T>();
        }
    }
}