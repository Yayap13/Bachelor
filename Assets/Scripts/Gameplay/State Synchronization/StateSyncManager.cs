using System.Collections.Generic;
using System.Linq;
using Network;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class StateSyncManager : MonoBehaviour, IStartClient, IStartServer
    {
        public Setup setup;
        public int SendRatePerSecond = 10;
        public int ObjectsSyncPerUpdate = 60;
        public int ElementsInJitterBuffer = 4;
        
        private float _nextSendTime = 0;
        
        private readonly List<Connection> _connectedPlayers = new List<Connection>();
        private readonly Dictionary<Connection, Rigidbody> _connectionToRigidbody = new Dictionary<Connection, Rigidbody>();
        private Transform entityHolder;
        
        private uint _sequenceId = 0;

        private JitterBuffer<StateSyncMessage> _jitterBuffer;
        
        private static readonly List<IStateSynchronize> _syncObjects = new List<IStateSynchronize>();

        private static readonly Dictionary<ushort, IStateSynchronize> _networkIdToIStateSynchronize = new Dictionary<ushort, IStateSynchronize>();

        private static ushort _nextFreeId = 1;
        private readonly StateSyncMessage _stateSyncMessage = new StateSyncMessage();
        
        public void OnStartServer()
        {
            SpawnLittleCubes();
            NetworkServer.NewConnectionEvent += connection =>
            {
                GameObject player = Instantiate(setup.playerCubePrefabSS);
                _connectedPlayers.Add(connection);
                _connectionToRigidbody.Add(connection, player.GetComponent<Rigidbody>());
            };
            NetworkServer.ConnectionDisconnectEvent += connection =>
            {
                GameObject player = _connectionToRigidbody[connection].gameObject;
                Destroy(player);
                _connectedPlayers.Remove(connection);
                _connectionToRigidbody.Remove(connection);
            };
        }

        public void OnStartClient()
        {
            entityHolder = new GameObject("Entities").transform;
            _jitterBuffer = new JitterBuffer<StateSyncMessage>(ElementsInJitterBuffer, SendRatePerSecond, (x => (int)x.SequenceNumber));
            NetworkClient.RegisterHandler<StateSyncMessage>(info =>
            {
                _jitterBuffer.Add(info.Message.Copy(), (int)info.Message.SequenceNumber);
            });
        }

        public static void Register(IStateSynchronize obj)
        {
            if(!_syncObjects.Contains(obj))
                _syncObjects.Add(obj);
            if(!_networkIdToIStateSynchronize.ContainsKey(obj.UniqueId))
                _networkIdToIStateSynchronize.Add(obj.UniqueId, obj);
        }

        public static void Unregister(IStateSynchronize obj)
        {
            if(_syncObjects.Contains(obj))
                _syncObjects.Remove(obj);
            if(_networkIdToIStateSynchronize.ContainsKey(obj.UniqueId))
                _networkIdToIStateSynchronize.Remove(obj.UniqueId);
        }

        public static ushort GetUniqueId()
        {
            return NetworkServer.Active ? _nextFreeId++ : ushort.MinValue; // Return first than increment it
        }

        protected void Update()
        {
            if (NetworkServer.Active && Time.time > _nextSendTime)
            {
                _nextSendTime = Time.time + 1f / SendRatePerSecond;
                
                int count = 0;
                if (_syncObjects.Count > 0)
                {
                    _sequenceId++;

                    foreach (IStateSynchronize obj in _syncObjects)
                    {
                        obj.CalculatePriority();
                    }
                    _syncObjects.Sort((p, q) =>
                        q.Priority.CompareTo(p.Priority)); // Sorting by descinding order of priority
                    //_syncObjects = _syncObjects.OrderBy(x => x.CalculatePriority()).ToList();
                    
                    _stateSyncMessage.SequenceNumber = _sequenceId;
                    
                    List<StateUpdateMessage> stateUpdateMessageList = new List<StateUpdateMessage>();
                    foreach (IStateSynchronize obj in _syncObjects)
                    {
                        if (count > ObjectsSyncPerUpdate || obj.Priority == 0)
                            break;
                        count++;

                        StateUpdateMessage stateUpdateMessage = new StateUpdateMessage();
                        stateUpdateMessage.SyncObject = obj.Prefab;
                        stateUpdateMessage.Id = obj.UniqueId;
                        stateUpdateMessage.Position = obj.Rigidbody.position;
                        stateUpdateMessage.Rotation = obj.Rigidbody.rotation;
                        stateUpdateMessage.IsMoving = obj.Rigidbody.IsSleeping();
                        stateUpdateMessage.LinearVelocity = obj.Rigidbody.velocity;
                        stateUpdateMessage.AngularVelocity = obj.Rigidbody.angularVelocity;
                        stateUpdateMessageList.Add(stateUpdateMessage);
                        
                        obj.Priority = 0;
                    }
                    _stateSyncMessage.NumberOfObjects = (ushort)stateUpdateMessageList.Count;
                    _stateSyncMessage.Objects = stateUpdateMessageList.ToArray();

                    foreach (var connection in _connectedPlayers)
                    {
                        if (!Input.GetKey(KeyCode.Space))
                        {
                            NetworkServer.SendToObservers(connection, _stateSyncMessage, 1); // 1 is unreliable
                        }
                    }
                }
            }
            if (NetworkClient.Active)
            {
                ProcessPacket();
            }
        }

        private void OnGUI()
        {
            if (NetworkServer.Active && Input.GetKey(KeyCode.Space))
            {
                GUI.contentColor = Color.red;
                GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 4 * 3, 400, 50), "The server is no longer sending any data to the clients");
            }
        }

        private void ProcessPacket()
        {
            StateSyncMessage message;
            
            if(!_jitterBuffer.TryGet(out message))
                return;
            
            for (int i = 0; i < message.NumberOfObjects; ++i)
            {
                IStateSynchronize obj;
                _networkIdToIStateSynchronize.TryGetValue(message.Objects[i].Id, out obj);
                if (obj == null)
                {
                    switch (message.Objects[i].SyncObject)
                    {
                           case SyncObject.PlayerCube:
                               obj = Instantiate(setup.playerCubePrefabSS).GetComponent<IStateSynchronize>();
                               break;
                           case SyncObject.LittleCube:
                               GameObject gm = Instantiate(setup.littleCubePrefabSS);
                               gm.transform.SetParent(entityHolder);
                               obj = gm.GetComponent<IStateSynchronize>();
                               break;
                           case SyncObject.None:
                               Debug.Assert(false);
                               return;
                    }
                    if (obj == null)
                    {
                        Debug.Assert(false);
                        return;
                    }
                    obj.UniqueId = message.Objects[i].Id;
                    Register(obj);
                }

//                obj.Rigidbody.isKinematic = true;
                obj.Rigidbody.position = message.Objects[i].Position;
                obj.Rigidbody.rotation = message.Objects[i].Rotation;
                obj.Rigidbody.velocity = message.Objects[i].LinearVelocity;
                obj.Rigidbody.angularVelocity = message.Objects[i].AngularVelocity;
//                obj.Rigidbody.isKinematic = false;
            }
        }
        
        private void SpawnLittleCubes()
        {
            Transform entityHolder = new GameObject("Entities").transform;
            for (int x = -setup.areaWidth / 2; x < setup.areaWidth / 2; x += setup.areaWidth / setup.cubePerWidth)
            {
                for (int z = -setup.areaWidth / 2; z < setup.areaWidth / 2; z += setup.areaWidth / setup.cubePerWidth)
                {
                    Instantiate(setup.littleCubePrefabSS, new Vector3(x, 1, z), Quaternion.identity).transform.SetParent(entityHolder);
                }
            }
        }
        
        private void FixedUpdate()
        {
            if (NetworkClient.Active)
                return;

            foreach (var connection in _connectedPlayers)
            {
                MovePlayer(connection);
            }
        }
        
        private void MovePlayer(Connection connection)
        {
            Rigidbody rb = _connectionToRigidbody[connection];
            float horizontalInput = 0;
            float verticalInput = 0;

            if (NetworkInput.GetKey(connection, Keys.MoveLeft))
            {
                horizontalInput = -1;
            }
            if (NetworkInput.GetKey(connection, Keys.MoveRight))
            {
                horizontalInput = 1;
            }
            if (NetworkInput.GetKey(connection, Keys.MoveForward))
            {
                verticalInput = 1;
            }
            if (NetworkInput.GetKey(connection, Keys.MoveBackward))
            {
                verticalInput = -1;
            }

            rb.AddForce(new Vector3(horizontalInput * setup.moveInputIntensity, 0, verticalInput * setup.moveInputIntensity));

            if (NetworkInput.GetKey(connection, Keys.Pull))
            {
                Vector3 pos = new Vector3(rb.transform.position.x, -1, rb.transform.position.z);
                Collider[] hitColliders = Physics.OverlapSphere(pos, 5);
                int i = 0;
                while (i < hitColliders.Length)
                {
                    Rigidbody colliderRb = hitColliders[i].GetComponent<Rigidbody>();
                    if (colliderRb != null && colliderRb != rb)
                        colliderRb.AddExplosionForce(50f, pos, 5f);
                    i++;
                }
            }
            if (NetworkInput.GetKey(connection, Keys.Push))
            {
                Vector3 pos = new Vector3(rb.transform.position.x, -1, rb.transform.position.z);
                Collider[] hitColliders = Physics.OverlapSphere(pos, 5);
                int i = 0;
                while (i < hitColliders.Length)
                {
                    Rigidbody colliderRb = hitColliders[i].GetComponent<Rigidbody>();
                    if (colliderRb != null && colliderRb != rb)
                        colliderRb.AddForce((rb.transform.position - hitColliders[i].transform.position) * 10f);
                    i++;
                }
            }
        }
    }
}