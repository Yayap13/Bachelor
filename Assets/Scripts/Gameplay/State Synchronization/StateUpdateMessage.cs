using System.Collections;
using System.Collections.Generic;
using Gameplay;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class StateUpdateMessage
    {
        public SyncObject SyncObject;
        public ushort Id;

        public Vector3 Position;
        public Quaternion Rotation;

        public bool IsMoving;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;

        public StateUpdateMessage Copy()
        {
            StateUpdateMessage message = new StateUpdateMessage();
            message.SyncObject = SyncObject;
            message.Id = Id;
            message.Position = Position;
            message.Rotation = Rotation;
            message.IsMoving = IsMoving;
            message.LinearVelocity = LinearVelocity;
            message.AngularVelocity = AngularVelocity;
            return message;
        }
    }
}