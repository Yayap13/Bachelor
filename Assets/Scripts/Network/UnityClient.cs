using SmartNet;
using SmartNet.Messages;
using UnityEngine;

namespace Network
{
    public class UnityClient : MonoBehaviour
    {
        [SerializeField] private string ip = "127.0.0.1";
        [SerializeField] private int port = 15337;
        [SerializeField] private ServerSettings settings;
        
        public GameObject setup;

        private void Start()
        {
            Application.runInBackground = true;
            
            GeneratedMessageTypes.Initialize();

            NetworkClient.Configure(settings);

            NetworkClient.ConnectEvent += () =>
            {
                Debug.Log("Connected to server");
                Instantiate(setup);
                NetworkClient.Send(new AddPlayer(), 0);
            };

            NetworkClient.DisconnectEvent += () => { Debug.Log("Disconnected from server"); };

            NetworkClient.Connect(ip, port);

            //WorldUpdater.ConfigureClient(200);

            //StatisticsUI.Show();
        }

        private void Update()
        {
            if(NetworkInput.GetKeyDown(Keys.Use))
                Debug.Log("Use key down!");
            if(NetworkInput.GetKeyUp(Keys.Use))
                Debug.Log("Use key up!");
        }

        private void OnDestroy()
        {
            NetworkClient.Disconnect();
        }
    }
}