using UnityEngine;
using Game.Application.Configuration.Abstractions;
#if UNITY_EDITOR
using Game.Application.Configuration.Editor; // Namespace chứa ConfigIdGenerator
#endif

namespace Game.Application.Configuration.BaseScriptableObject
{
    public abstract class BaseConfigSo : ScriptableObject, IConfig
    {
        [SerializeField] private int _id;
        public int Id => _id;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // Chỉ tự động cấp ID nếu ID hiện tại đang bằng 0 (file mới)
            // Không tự sinh ID khi đang chơi game hoặc đang trong quá trình build
            if (UnityEngine.Application.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer) return;

            if (_id == 0)
            {
                _id = ConfigIdGenerator.GetNextAvailableId(this.GetType());
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}