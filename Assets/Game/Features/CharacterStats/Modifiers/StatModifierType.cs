namespace Game.Character.Core.Stats
{
    /// <summary>
    /// Defines the type of modification applied to a stat value.
    /// Xác định loại sửa đổi được áp dụng cho giá trị thống kê.
    /// </summary>
    public enum StatModifierType
    {
        /// <summary>
        /// Adds a flat value to the base stat.
        /// Thêm một giá trị cố định vào thống kê cơ bản.
        /// </summary>
        Flat = 0,

        /// <summary>
        /// Adds a percentage value to the base stat (additive with other PercentAdd modifiers).
        /// Thêm một giá trị phần trăm vào thống kê cơ bản (cộng dồn với các bộ điều chỉnh PercentAdd khác).
        /// </summary>
        PercentAdd = 1,

        /// <summary>
        /// Multiplies the current stat value by a percentage (multiplicative - applied after PercentAdd).
        /// Nhân giá trị thống kê hiện tại với một phần trăm (nhân - áp dụng sau PercentAdd).
        /// </summary>
        PercentMultiply = 2,

        /// <summary>
        /// Overrides the final stat value, ignoring all other modifiers (use with caution).
        /// Ghi đè giá trị thống kê cuối cùng, bỏ qua tất cả các bộ điều chỉnh khác (sử dụng cẩn thận).
        /// </summary>
        //Override = 3
    }
}

