using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class StateSyncPlayerCube : StateSyncObject
    {
        public override void CalculatePriority()
        {
            Priority = 1000000;
        }

        protected override void Awake()
        {
            Prefab = SyncObject.PlayerCube;
            base.Awake();
        }
    }
}