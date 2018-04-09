using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Gameplay
{
    public struct InterpolationData
    {
        public float Time;
        public SnapshotMessage Snapshot;

        public InterpolationData(float time, SnapshotMessage snapshot)
        {
            Time = time;
            Snapshot = snapshot;
        }
    }
}