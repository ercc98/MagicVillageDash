using UnityEngine;
using ErccDev.Foundation.Pause;
using System;

namespace MagicVillageDash.Pause
{

    public sealed class PauseServiceBehaviour : MonoBehaviour, IPauseService
    {
        private readonly PauseService service = new PauseService();

        public bool IsPaused => service.IsPaused;

        public event Action<bool, string> Changed
        {
            add    { service.Changed += value; }
            remove { service.Changed -= value; }
        }

        public void Pause(string reason = "Pause")  => service.Pause(reason);
        public void Resume(string reason = "Resume") => service.Resume(reason);
        public void Toggle(string reason = "Toggle") => service.Toggle(reason);
    }
}
