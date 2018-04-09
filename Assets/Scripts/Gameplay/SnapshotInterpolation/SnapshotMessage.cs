using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class SnapshotMessage : INetMessage
    {
        private static uint _lastSequenceNumber = 0;
        
        public uint SequenceNumber;
        
        // Maximum 255 players
        public byte NumberOfPlayers;

        // Maximum 65'535 cubes
        public ushort NumberOfLittleCubes;

        public Vector3[] Positions;
        public Quaternion[] Rotations;

        public SnapshotMessage()
        {
            SequenceNumber = ++_lastSequenceNumber;
        }

        public SnapshotMessage(List<GameObject> playerCubes, GameObject[] littleCubes)
        {
            SequenceNumber = ++_lastSequenceNumber;
            NumberOfPlayers = (byte) playerCubes.Count;
            NumberOfLittleCubes = (ushort) littleCubes.Length;
            
            Positions = new Vector3[NumberOfPlayers+NumberOfLittleCubes];
            Rotations = new Quaternion[NumberOfPlayers+NumberOfLittleCubes];

            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                Positions[i] = playerCubes[i].transform.position;
                Rotations[i] = playerCubes[i].transform.rotation;
            }
            for (int i = 0; i < NumberOfLittleCubes; ++i)
            {
                Positions[i + NumberOfPlayers] = littleCubes[i].transform.position;
                Rotations[i + NumberOfPlayers] = littleCubes[i].transform.rotation;
            }
        }

        public void OnSerialize(Writer writer)
        {
            writer.WriteCompressed(SequenceNumber);
            writer.Write(NumberOfPlayers);
            writer.Write(NumberOfLittleCubes);
            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                writer.WriteCompressed(Positions[i]);
                writer.WriteCompressed(Rotations[i]);
            }
            for (int i = 0; i < NumberOfLittleCubes; ++i)
            {
                writer.WriteCompressed(Positions[i + NumberOfPlayers]);
                writer.WriteCompressed(Rotations[i + NumberOfPlayers]);
            }
        }

        public void OnDeserialize(Reader reader)
        {
            SequenceNumber = reader.ReadUIntCompressed();
            NumberOfPlayers = reader.ReadByte();
            NumberOfLittleCubes = reader.ReadUShort();

            Positions = new Vector3[NumberOfPlayers + NumberOfLittleCubes];
            Rotations = new Quaternion[NumberOfPlayers + NumberOfLittleCubes];
            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                Positions[i] = reader.ReadVector3Compressed();
                Rotations[i] = reader.ReadQuaternionCompressed();
            }
            for (int i = 0; i < NumberOfLittleCubes; ++i)
            {
                Positions[i+NumberOfPlayers] = reader.ReadVector3Compressed();
                Rotations[i+NumberOfPlayers] = reader.ReadQuaternionCompressed();
            }
        }

        public SnapshotMessage Copy()
        {
            SnapshotMessage message = new SnapshotMessage();
            message.SequenceNumber = SequenceNumber;
            message.NumberOfPlayers = NumberOfPlayers;
            message.NumberOfLittleCubes = NumberOfLittleCubes;
            message.Positions = Positions;
            message.Rotations = Rotations;
            return message;
        }
    }
}