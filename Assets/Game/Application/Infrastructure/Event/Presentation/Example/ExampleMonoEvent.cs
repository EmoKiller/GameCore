// using UnityEngine;
// using Game.Application.Events;
// using Game.Presentation.Events;

// namespace Game.UI
// {
//     public sealed class LoadingView                         ❌ Sai cách 1: View handle event trực tiếp
//         : MonoEventHandler<ShowLoadingEvent>                         → Coupling cao
//     {                                                                → Không test được
//         [SerializeField] private GameObject loadingPanel;            → UI logic bị phân tán

//         public override int Priority => EventPriority.Normal;
//         public override EventChannel Channel => EventChannel.UI;

//         public override void Handle(ShowLoadingEvent evt)
//         {
//             loadingPanel.SetActive(evt.IsVisible);
//         }
//     }
// }
// namespace Game.Gameplay
// {
//     public sealed class LoadLevelHandler 
//         : MonoAsyncEventHandler<LoadLevelEvent>
//     {
//         public override int Priority => EventPriority.High;
//         public override EventChannel Channel => EventChannel.Gameplay;

//         public override async UniTask HandleAsync(LoadLevelEvent evt)
//         {
//             await LoadSceneAsync(evt.SceneName);
//         }
//     }
// }

// public sealed class HUDController : MultiEventBinder                     ❌ Sai cách 2: bỏ qua ViewModel
// {                                                                            → Không scale được
//     private HpHandler _hp;                                                   → Không reuse        
//     private ManaHandler _mana;                                               → Debug cực khó

//     protected override void Awake()                                      ❌ Sai cách 3: ReactiveProperty nhưng vẫn dùng Event trong View
//     {                                                                    → duplicate responsibility
//         base.Awake();                                                    → hệ thống bị “2 nguồn sự thật”

//         _hp = new HpHandler(this);
//         _mana = new ManaHandler(this);
//     }

//     protected override void RegisterHandlers(bool bind)
//     {
//         if (bind)
//         {
//             EventBus.Subscribe(_hp);
//             EventBus.Subscribe(_mana);
//         }
//         else
//         {
//             EventBus.Unsubscribe(_hp);
//             EventBus.Unsubscribe(_mana);
//         }
//     }
// }