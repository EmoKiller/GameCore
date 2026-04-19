using System;
using System.Collections.Generic;
namespace Game.Application.Configuration.Abstractions
{
    public interface IConfigRegistry
    {
        // --- QUẢN LÝ DỮ LIỆU ---
        
        /// <summary> Đăng ký mới (Ghi đè nếu đã tồn tại loại T) </summary>
        void Register<T>(IEnumerable<T> configs, Func<T, int> idSelector) where T : IConfig;

        /// <summary> Nạp thêm vào danh sách T đang có sẵn </summary>
        void Append<T>(IEnumerable<T> configs, Func<T, int> idSelector) where T : IConfig;

        /// <summary> Gỡ bỏ hoàn toàn một loại Config khỏi bộ nhớ </summary>
        void Unregister<T>() where T : IConfig;

        /// <summary> Xóa sạch toàn bộ Registry (Dùng khi Reset Game) </summary>
        void ClearAll();

        // --- TRUY XUẤT DỮ LIỆU ---

        /// <summary> Lấy 1 config theo ID (Throw exception nếu không thấy) </summary>
        T Get<T>(int id) where T : IConfig;

        /// <summary> Thử lấy config (Trả về false nếu không thấy, không crash) </summary>
        bool TryGet<T>(int id, out T config) where T : IConfig;

        /// <summary> Lấy toàn bộ danh sách config của loại T </summary>
        IReadOnlyCollection<T> GetAll<T>() where T : IConfig;

        // --- KIỂM TRA TRẠNG THÁI ---

        /// <summary> Kiểm tra loại T đã được nạp vào Registry chưa </summary>
        bool IsRegistered<T>() where T : IConfig;

        /// <summary> Kiểm tra một ID cụ thể có tồn tại trong loại T không </summary>
        bool Contains<T>(int id) where T : IConfig;

        /// <summary> Trả về số lượng item của loại T </summary>
        int GetCount<T>() where T : IConfig;



        // T Get<T>(ConfigId<T> id) where T : IConfig;

        // bool TryGet<T>(ConfigId<T> id, out T config) where T : IConfig;

        // IReadOnlyCollection<T> GetAll<T>() where T : IConfig;
    }
}
