public enum UIReusePolicy
{
    /// <summary>
    /// giữ instance trong stack (hot)
    /// </summary>
    Retain,  

    /// <summary>
    /// giữ instance ngoài stack (warm)      
    /// </summary>
    Cache,  
    /// <summary>
    /// trả về pool (cold)       
    /// </summary>
    Release, 
    Destroy     
}

// | Policy  | Có giữ instance không  | Có reuse không        | Vị trí lưu |
// | ------- | ---------------------  | -------------------   | ---------- |
// | Retain  | ✅ luôn giữ            | ❌ không reuse logic | Stack      |
// | Cache   | ✅ giữ tạm             | ✅ reuse             | Cache      |
// | Release | ❌ không giữ trực tiếp | ✅ reuse qua pool    | Pool       |



// Retain — “giữ nguyên trạng”
// ✔ Bản chất
// View không bị destroy
// Không qua pool
// Không reset state
// Screen chính (Gameplay UI)
// UI cần giữ state:
// Scroll position
// Tab đang chọn
// Form input


// Cache — “giữ tạm để reuse nhanh”
// ✔ Bản chất
// View bị remove khỏi stack
// Nhưng không destroy
// Lưu vào _cache
// UI mở thường xuyên nhưng không cần giữ state:

// Inventory
// Settings
// Shop

// Release — “trả về pool”
// ✔ Bản chất
// View bị remove khỏi system
// Lifetime dispose
// View trả về pool
// UI ngắn hạn:

// Loading
// Toast
// Popup tạm

// | UI Type       | Policy  |
// | ------------- | ------- |
// | Main screen   | Retain  | Release
// | Gameplay HUD  | Retain  |
// | Settings      | Cache   |
// | Inventory     | Cache   |
// | Loading       | Release |
// | Toast         | Release |
// | Popup confirm | Release |
