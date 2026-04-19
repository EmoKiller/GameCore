public enum UIReusePolicy
{
    Retain,        // giữ instance trong stack (hot)
    Cache,         // giữ instance ngoài stack (warm)
    Release,       // trả về pool (cold)
    Persistent     // never unload (HUD, overlay)
}