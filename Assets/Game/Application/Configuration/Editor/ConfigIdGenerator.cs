using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using Game.Application.Configuration.Runtime;
using Game.Application.Configuration.BaseScriptableObject;

namespace Game.Application.Configuration.Editor
{
    public static class ConfigIdGenerator
    {
        // Cấu hình dải ID tập trung
        private static readonly Dictionary<Type, int> TypeRanges = new()
        {
            // { typeof(HeroConfig), 1000 },
            // { typeof(ItemConfig), 2000 },
            // { typeof(EnemyConfig), 3000 }
        };

        private const int DefaultStartId = 10000;

        public static int GetNextAvailableId(Type type)
        {
            if (type == null) return 0;

            // 1. Xác định ID bắt đầu
            int startId = TypeRanges.TryGetValue(type, out int rangeStart) ? rangeStart : DefaultStartId;

            // 2. Thu thập tất cả ID đang được sử dụng
            // Dùng "t:" + type.Name là cách nhanh nhất để lọc trong AssetDatabase
            string filter = $"t:{type.Name}";
            string[] guids = AssetDatabase.FindAssets(filter);
            
            var usedIds = new HashSet<int>();

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Chỉ load asset nếu nó thực sự thuộc loại mình cần (tránh nhầm lẫn class trùng tên)
                var asset = AssetDatabase.LoadAssetAtPath(path, type) as BaseConfigSo;
                
                if (asset != null)
                {
                    usedIds.Add(asset.Id);
                }
            }

            // 3. Tìm lỗ hổng ID đầu tiên có sẵn trong dải
            int candidate = startId;
            while (usedIds.Contains(candidate))
            {
                candidate++;
            }

            return candidate;
        }
    }
}