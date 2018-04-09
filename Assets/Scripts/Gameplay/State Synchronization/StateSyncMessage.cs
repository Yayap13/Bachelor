using System.Collections;
using System.Collections.Generic;
using Gameplay;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class StateSyncMessage : INetMessage
    {
        public uint SequenceNumber;
        public ushort NumberOfObjects;

        public StateUpdateMessage[] Objects;

        public void OnSerialize(Writer writer)
        {
            writer.WriteCompressed(SequenceNumber);
            writer.Write(NumberOfObjects);
            for (int i = 0; i < NumberOfObjects; ++i)
            {
                writer.Write((byte) Objects[i].SyncObject);
                writer.Write(Objects[i].Id);
                writer.WriteCompressed(Objects[i].Position);
                writer.WriteCompressed(Objects[i].Rotation);
                writer.Write(Objects[i].IsMoving);
//                if (Objects[i].IsMoving)
//                {
                    writer.WriteCompressed(Objects[i].LinearVelocity);
                    writer.WriteCompressed(Objects[i].AngularVelocity);
//                }
            }
        }

        public void OnDeserialize(Reader reader)
        {
            SequenceNumber = reader.ReadUIntCompressed();
            NumberOfObjects = reader.ReadUShort();
            Objects = new StateUpdateMessage[NumberOfObjects];
            
            for (int i = 0; i < NumberOfObjects; ++i)
            {
                Objects[i] = new StateUpdateMessage();
                Objects[i].SyncObject = (SyncObject) reader.ReadByte();
                Objects[i].Id = reader.ReadUShort();
                Objects[i].Position = reader.ReadVector3Compressed();
                Objects[i].Rotation = reader.ReadQuaternionCompressed();
                Objects[i].IsMoving = reader.ReadBoolean();
//                if (Objects[i].IsMoving)
//                {
                    Objects[i].LinearVelocity = reader.ReadVector3Compressed();
                    Objects[i].AngularVelocity = reader.ReadVector3Compressed();
//                }
            }
        }

        public StateSyncMessage Copy()
        {
            StateSyncMessage message = new StateSyncMessage();
            message.SequenceNumber = SequenceNumber;
            message.NumberOfObjects = NumberOfObjects;
            message.Objects = new StateUpdateMessage[NumberOfObjects];
            for (int i = 0; i < NumberOfObjects; ++i)
            {
                message.Objects[i] = Objects[i].Copy();
            }
            return message;
        }
    }
}