using System.Collections.Generic;
using System.Linq;
using Network;
using SmartNet;
using UnityEngine;

namespace Gameplay
{
    public class SnapshotInterpolationManager : MonoBehaviour, IStartClient, IStartServer
    {
        public Setup setup;
        public int sendRatePerSecond = 20;
        public int interpolationBufferInMs = 100;

        private float _nextSendTime = 0;
        private readonly List<Connection> _connectedPlayers = new List<Connection>();

        private readonly Dictionary<Connection, Rigidbody> _connectionToRigidbody = new Dictionary<Connection, Rigidbody>();

        private readonly List<GameObject> _playerCubes = new List<GameObject>();
        private GameObject[] _littleCubes;
        private Transform entityHolder;

        private SnapshotMessage _snapshotMessage;


        private int _oldestInterpolationDataIndex = 0;
        private InterpolationData[] _interpolationDatas;

        private InterpolationData _newSnapsnot = new InterpolationData(0, new SnapshotMessage());
        private InterpolationData _oldSnapshot = new InterpolationData(0, new SnapshotMessage());


        public void OnStartServer()
        {
            NetworkServer.NewConnectionEvent += connection =>
            {
                GameObject player = Instantiate(setup.playerCubePrefabSI);
                _connectedPlayers.Add(connection);
                _connectionToRigidbody.Add(connection, player.GetComponent<Rigidbody>());
                _playerCubes.Add(player);
            };
            NetworkServer.ConnectionDisconnectEvent += connection =>
            {
                GameObject player = _connectionToRigidbody[connection].gameObject;
                Destroy(player);
                _connectedPlayers.Remove(connection);
                _connectionToRigidbody.Remove(connection);
                _playerCubes.Remove(player);
            };
        }

        public void OnStartClient()
        {
            Debug.Log("OnStartClient!");
            // Make sure to round up here
            _interpolationDatas = new InterpolationData[(int) ((1f / sendRatePerSecond * interpolationBufferInMs * 2) + 0.5f)];
            for (int i = 0; i < _interpolationDatas.Length; ++i)
            {
                _interpolationDatas[i] = new InterpolationData(0, new SnapshotMessage());
            }

            NetworkClient.RegisterHandler<SnapshotMessage>(info =>
            {
                if (_newSnapsnot.Snapshot.SequenceNumber >= info.Message.SequenceNumber)
                    return;

                _interpolationDatas[_oldestInterpolationDataIndex] = new InterpolationData(Time.time, info.Message.Copy());
                _oldestInterpolationDataIndex = (_oldestInterpolationDataIndex + 1) % _interpolationDatas.Length;
            });
        }

        private void Awake()
        {
            entityHolder = new GameObject("Entities").transform;
            SpawnLittleCubes();
        }

        private void Update()
        {

            if (NetworkClient.Active)
            {
                float renderTime = Time.time - (interpolationBufferInMs / 1000f);

                // We need to find the two best snapshot to interpolate from
                if (renderTime > _newSnapsnot.Time)
                {
                    int j = _oldestInterpolationDataIndex;
                    for (int i = 0; i < _interpolationDatas.Length; ++i)
                    {
                        if (_interpolationDatas[j].Time > Time.time - (interpolationBufferInMs / 1000f))
                        {
                            _oldSnapshot = _newSnapsnot;
                            _newSnapsnot = _interpolationDatas[j];

                            Debug.Assert(_oldSnapshot.Snapshot.SequenceNumber <= _newSnapsnot.Snapshot.SequenceNumber);
                            break;
                        }
                        j = (j + 1) % _interpolationDatas.Length;
                    }
                }

                // Spawn/Despawn players
                while (_newSnapsnot.Snapshot.NumberOfPlayers != _playerCubes.Count)
                {
                    if (_playerCubes.Count < _newSnapsnot.Snapshot.NumberOfPlayers)
                    {
                        _playerCubes.Add(Instantiate(setup.playerCubePrefabSI));
                    }
                    else
                    {
                        Destroy(_playerCubes.Last());
                        _playerCubes.Remove(_playerCubes.Last());
                    }
                }

                if (_oldSnapshot.Time > 0 && _newSnapsnot.Time > 0)
                {
                    float lerp = (renderTime - _oldSnapshot.Time) / (_newSnapsnot.Time - _oldSnapshot.Time);

                    // Now update nicely with a linear interpolation all positions and rotations
                    for (int i = 0; i < _newSnapsnot.Snapshot.NumberOfPlayers; ++i)
                    {
                        _playerCubes[i].transform.position = Vector3.Lerp(_oldSnapshot.Snapshot.Positions[i],
                            _newSnapsnot.Snapshot.Positions[i], lerp);
                        _playerCubes[i].transform.rotation = Quaternion.Lerp(_oldSnapshot.Snapshot.Rotations[i],
                            _newSnapsnot.Snapshot.Rotations[i], lerp);
                    }
                    for (int i = 0; i < _newSnapsnot.Snapshot.NumberOfLittleCubes; ++i)
                    {
                        _littleCubes[i].transform.position = Vector3.Lerp(_oldSnapshot.Snapshot.Positions[i + _newSnapsnot.Snapshot.NumberOfPlayers],
                            _newSnapsnot.Snapshot.Positions[i + _newSnapsnot.Snapshot.NumberOfPlayers], lerp);
                        _littleCubes[i].transform.rotation = Quaternion.Lerp(_oldSnapshot.Snapshot.Rotations[i + _newSnapsnot.Snapshot.NumberOfPlayers],
                            _newSnapsnot.Snapshot.Rotations[i + _newSnapsnot.Snapshot.NumberOfPlayers], lerp);
                    }
                }
            }

            if (NetworkServer.Active && Time.time > _nextSendTime)
            {
                SendSnapshot();
                _nextSendTime = Time.time + 1f / sendRatePerSecond;
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
        
        private void OnGUI()
        {
            if (NetworkServer.Active && Input.GetKey(KeyCode.Space))
            {
                GUI.contentColor = Color.red;
                GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 4 * 3, 400, 50), "The server is no longer sending any data to the clients");
            }
        }

        private void SpawnLittleCubes()
        {
            _littleCubes = new GameObject[setup.cubePerWidth * setup.cubePerWidth];
            int pos = 0;
            for (int x = -setup.areaWidth / 2; x < setup.areaWidth / 2; x += setup.areaWidth / setup.cubePerWidth)
            {
                for (int z = -setup.areaWidth / 2; z < setup.areaWidth / 2; z += setup.areaWidth / setup.cubePerWidth)
                {
                    GameObject gm = Instantiate(setup.littleCubePrefabSI, new Vector3(x, 1, z), Quaternion.identity);
                    gm.transform.SetParent(entityHolder);
                    if (NetworkClient.Active)
                        gm.GetComponent<Rigidbody>().isKinematic = true;
                    _littleCubes[pos++] = gm;
                }
            }
        }

        private void SendSnapshot()
        {
            _snapshotMessage = new SnapshotMessage(_playerCubes, _littleCubes);
            foreach (var connection in _connectedPlayers)
            {
                if (!Input.GetKey(KeyCode.Space))
                {
                    NetworkServer.SendToObservers(connection, _snapshotMessage, 1); // 1 is unreliable
                }
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