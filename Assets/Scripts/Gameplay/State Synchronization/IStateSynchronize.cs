using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public interface IStateSynchronize
    {
        SyncObject Prefab { get; }
        ushort UniqueId { get; set; }
        Rigidbody Rigidbody { get; }
        float Priority { get; set; }

        void CalculatePriority();
    }
}