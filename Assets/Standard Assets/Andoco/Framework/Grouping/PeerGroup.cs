using System;
using System.Collections.Generic;
using Andoco.Core.Diagnostics.Logging;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Andoco.Unity.Framework.Grouping
{
    public class PeerGroup : MonoBehaviour
    {
        [Inject]
        private IStructuredLog log;

        public Directory directory;

        public JoinPeerKind joinMode;

        [Tooltip("Allows joining with other peers that are members of a different group to the current peer")]
        public bool joinForeignPeers;

        public float peerGizmoSphereRadius = 1f;
        public Vector3 peerGizmoLineOffset;

        #region Properties

        public bool IsPeer { get { return this.directory != null; } }

        public IList<PeerGroup> Peers { get { return this.directory == null ? null : this.directory.Peers; } }

        #endregion

        #region Lifecycle

        void OnDrawGizmos()
        {
            if (this.IsPeer && this == this.Peers[0])
            {
                var tx = this.transform;

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(tx.position, this.peerGizmoSphereRadius);

                var lineStartPos = tx.TransformPoint(this.peerGizmoLineOffset);

                for (int i = 1; i < this.Peers.Count; i++)
                {
                    if (this.Peers[i] == null)
                        continue;
                    
                    var peerTx = this.Peers[i].transform;

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(peerTx.position, this.peerGizmoSphereRadius);

                    var lineEndPos = peerTx.TransformPoint(this.peerGizmoLineOffset);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(lineStartPos, lineEndPos);
                }
            }
        }

        void Recycled()
        {
            this.LeaveCurrent();
        }

        void OnDestroy()
        {
            this.LeaveCurrent();
        }

        #endregion

        #region Public methods

        public void JoinWith(PeerGroup peer)
        {
            if (!this.CanJoin(peer))
                return;

            if (this.log.Status.IsTraceEnabled)
                this.log.Trace("Peer is joining with another peer").Field("currentPeer", this).Field("otherPeer", peer).Write();

            switch (this.joinMode)
            {
                case JoinPeerKind.None:
                    break;
                case JoinPeerKind.Leave:
                    this.LeaveCurrent();
                    peer.AddPeer(this);
                    break;
                case JoinPeerKind.Merge:
                    this.MergeWith(peer);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown join peer mode {0}", this.joinMode));
            }
        }

        public void MergeWith(PeerGroup peer)
        {
            if (this.directory == null)
            {
                peer.AddPeer(this);
            }
            else
            {
                Assert.IsFalse(this.HasSameGroup(peer));

                var tmpPeers = this.directory.Peers;

                while (tmpPeers.Count > 0)
                {
                    var p = tmpPeers[tmpPeers.Count - 1];
                    p.LeaveCurrent(false);
                    peer.AddPeer(p);
                }
            }
        }

        public bool CanJoin(PeerGroup otherPeer)
        {
            if (!ObjectValidator.Validate(otherPeer))
                return false;

            // Check joining self.
            if (otherPeer == this)
                return false;

            // Check joining peers in a different group to our own.
            if (!this.joinForeignPeers && this.IsPeer)
                return false;

            // Check rejoining existing group.
            if (this.HasSameGroup(otherPeer))
                return false;

            return true;
        }

        public void AddPeer(PeerGroup peer)
        {
            Assert.IsNotNull(peer);
            Assert.IsFalse(peer.IsPeer);

            if (this.directory == null)
            {
                // Creating a new group.
                this.directory = new Directory();
                this.directory.Peers.Add(this);
            }

            this.directory.Peers.Add(peer);
            peer.directory = this.directory;
        }

        public void LeaveCurrent(bool cleanupGroup = true)
        {
            // Not in group so nothing to do.
            if (this.directory == null)
                return;
            
            if (this.log.Status.IsTraceEnabled)
                this.log.Trace("Peer is leaving its current group")
                    .Field("peer", this)
                    .Field("numPeers", this.Peers.Count)
                    .Write();

            Assert.IsNotNull(this.directory);

            this.directory.Peers.Remove(this);

            // If only 1 peer remains in the directory, also tell it leave so that the group no longer exists.
            if (cleanupGroup && this.directory.Peers.Count == 1)
                this.directory.Peers[0].LeaveCurrent();

            this.directory = null;
        }

        public bool HasSameGroup(PeerGroup other)
        {
            Assert.IsFalse(this == other);

            // Check if either is not in a peer group.
            if (!(this.IsPeer || other.IsPeer))
                return false;
            
            return this.directory == other.directory;
        }

        #endregion

        #region Private methods

        #endregion

        #region Types

        public class Directory
        {
            public Directory()
            {
                this.Peers = new List<PeerGroup>();
            }

            public IList<PeerGroup> Peers { get; private set; }
        }

        #endregion
    }
}

