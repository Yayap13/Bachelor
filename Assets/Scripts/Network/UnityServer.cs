using System.Collections.Generic;
using SmartNet;
using UnityEngine;

namespace Network
{
    public class UnityServer : MonoBehaviour
    {
        [SerializeField] private string ip = "127.0.0.1";
        [SerializeField] private int port = 15337;
        [SerializeField] private ServerSettings settings;

        public GameObject setup;

        private void Start()
        {
            Application.runInBackground = true;
            
            GeneratedMessageTypes.Initialize();

            NetworkServer.Configure(settings);

            NetworkServer.StartEvent += () =>
            {
                Debug.Log($"Start server id {NetworkServer.HostId}");
                Instantiate(setup);
            };

            NetworkServer.StopEvent += () => { Debug.Log($"Stop server id {NetworkServer.HostId}"); };
            NetworkServer.NewConnectionEvent += connection =>
            {
                Debug.Log($"New connection from {connection.Ip}");
                //NetworkScene.Spawn(TestPrefab);
            };
            NetworkServer.ConnectionDisconnectEvent += connection =>
            {
                Debug.Log($"{connection.Ip} disconnected");
            };

            NetworkServer.Start(ip, port);

            //WorldUpdater.ConfigureServer(33, 200);

            //StatisticsUI.Show();
        }

        private void OnDestroy()
        {
            NetworkServer.Stop();
        }
    }
}