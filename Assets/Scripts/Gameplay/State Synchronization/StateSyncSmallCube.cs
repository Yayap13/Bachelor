using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class StateSyncSmallCube : StateSyncObject
    {
        public override void CalculatePriority()
        {
            if (Rigidbody.IsSleeping())
            {
                Priority += 1;
            }
            else
            {
                Priority = (Rigidbody.velocity == Vector3.zero && Rigidbody.angularVelocity == Vector3.zero)
                    ? Priority + 10
                    : Priority + 1000;
            }
        }
        
        protected override void Awake()
        {
            Prefab = SyncObject.LittleCube;
            base.Awake();
        }
    }
}