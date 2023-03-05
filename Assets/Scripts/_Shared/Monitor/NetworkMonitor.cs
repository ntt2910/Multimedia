using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BW.Networking.Monitors
{
    public class NetworkMonitor : MonoBehaviour, INetworkMonitor
    {
        [SerializeField] private float checkInterval = 60;

        private bool connected;
        private UnityEngine.Coroutine monitorCor;

        public bool ConnectedToInternet
        {
            get => this.connected;
            private set
            {
                if (this.connected == value)
                {
                    return;
                }

                this.connected = value;
                Debug.Log("Connection Status Changed: " + value);
                OnConnectionStatusChange?.Invoke(value);
            }
        }

        public event Action<bool> OnConnectionStatusChange;

        public void StartMonitorInternet()
        {
            if (this.monitorCor != null) StopMonitorInternet();
            this.monitorCor = StartCoroutine(_MonitorInternetConnection());
        }

        public void StopMonitorInternet()
        {
            StopCoroutine(this.monitorCor);
            this.monitorCor = null;
        }

        private IEnumerator _MonitorInternetConnection()
        {
            while (true)
            {
                yield return StartCoroutine(_CheckInternetConnection());
                yield return new WaitForSeconds(this.checkInterval);
            }
        }

        private IEnumerator _CheckInternetConnection()
        {
            yield return new WaitForEndOfFrame();
            var request = UnityWebRequest.Get("http://google.com");
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                if (ConnectedToInternet)
                {
                    ConnectedToInternet = false;
                }
            }
            else
            {
                if (ConnectedToInternet == false)
                {
                    ConnectedToInternet = true;
                }
            }
        }
    }
}