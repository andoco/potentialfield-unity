using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Grouping
{
    public class TriggerPeerJoiner : MonoBehaviour
    {
        public PeerGroup peerGroup;

        void OnTriggerEnter(Collider other)
        {
            Assert.IsNotNull(this.peerGroup);

            var otherPeer = other.GetComponentInParent<PeerGroup>();

            if (otherPeer != null)
            {
                this.peerGroup.JoinWith(otherPeer);
            }
        }
    }
}
