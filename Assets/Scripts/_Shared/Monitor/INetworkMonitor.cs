using System;
using BW.Services;

namespace BW.Networking.Monitors
{
    public interface INetworkMonitor : IService
    {
        bool ConnectedToInternet { get; }
        event Action<bool> OnConnectionStatusChange;
    }
}