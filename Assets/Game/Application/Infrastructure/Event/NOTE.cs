// 1. Dùng MonoEventBinder cho:
// Presenter
// Gameplay handler
// System integration

// 👉 KHÔNG dùng cho View

// 2. Dùng Reactive + MVVM-lite cho:
// UI
// State binding
// Data flow trong UI

// 👉 KHÔNG dùng EventBus trực tiếp trong View






// ❌ Sai cách 1: View handle event trực tiếp
// → Coupling cao
// → Không test được
// → UI logic bị phân tán

// ❌ Sai cách 2: bỏ qua ViewModel
// → Không scale được
// → Không reuse
// → Debug cực khó

// ❌ Sai cách 3: ReactiveProperty nhưng vẫn dùng Event trong View

// → duplicate responsibility
// → hệ thống bị “2 nguồn sự thật”