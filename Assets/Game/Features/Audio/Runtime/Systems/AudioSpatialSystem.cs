// using Audio.Core.Data;
// using UnityEngine;
// namespace Audio.Runtime.Systems
// {
    

//     internal sealed class AudioSpatialSystem
//     {
//         public Handles.AudioHandle Play3D(AudioId id, Transform target)
//         {
//             var data = _registry.GetAsync(id).GetAwaiter().GetResult();

//             var handle = _channels[data.Channel].Play(data);

//             _spatial.Apply3D(GetSource(handle), target);

//             Track(id, handle);

//             return handle;
//         }

//         public void UpdateFollow(AudioSource source, Transform target)
//         {
//             if (source == null || target == null) return;

//             source.transform.position = target.position;
//         }
//     }
// }