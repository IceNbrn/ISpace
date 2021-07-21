using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class LocalPlayerAnnouncer : NetworkBehaviour
    {
        public static Action<NetworkIdentity> OnLocalPlayerUpdated;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            OnLocalPlayerUpdated?.Invoke(base.netIdentity);
        }

        private void OnEnable()
        {
            if(base.isLocalPlayer)
                OnLocalPlayerUpdated?.Invoke(base.netIdentity);
        }

        private void OnDisable()
        {
            if(base.isLocalPlayer)
                OnLocalPlayerUpdated?.Invoke(null);
        }
    }
}