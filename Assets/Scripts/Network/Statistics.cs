using System;
using System.Collections.Generic;
using System.Linq;
using SmartNet.Profiler;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class Statistics : MonoBehaviour
    {
        public int RefreshPerSeconds = 4;
        public int CapturePerSeconds = 30;
        private float _nextRefreshTime = 0;
        private float _nextCaptureTime = 0;

        private int _allBytesOut = 0;
        
        private readonly Queue<int> _lastBytesOut = new Queue<int>();
        
        private int _bytesOutLastSec = 0;
        
        private void OnGUI()
        {
            String value;
            if (_bytesOutLastSec > 1000000f)
            {
                value = (_bytesOutLastSec/1000000)+"MB/s";
            } else if (_bytesOutLastSec > 1000f)
            {
                value = (_bytesOutLastSec/1000)+"KB/s";
            }
            else
            {
                value = _bytesOutLastSec+"B/s";
            }
            GUILayout.Label($"Out {value}");
        }

        private void Update()
        {
            if (Time.time > _nextCaptureTime)
            {
                _nextCaptureTime = Time.time + (1 / (float)CapturePerSeconds);
                int newBytesOut = NetworkTransport.GetOutgoingFullBytesCount();
                _lastBytesOut.Enqueue(newBytesOut-_allBytesOut);
                _allBytesOut = newBytesOut;
            }
            while (_lastBytesOut.Count>CapturePerSeconds)
            {
                _lastBytesOut.Dequeue();
            }
            if (Time.time > _nextRefreshTime)
            {
                _nextRefreshTime = Time.time + (1 / (float)RefreshPerSeconds);
                _bytesOutLastSec = (int)_lastBytesOut.Average()*CapturePerSeconds;
            }
        }
    }
}