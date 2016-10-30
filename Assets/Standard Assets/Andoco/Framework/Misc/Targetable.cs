using System.Collections.Generic;
using Andoco.Core.Diagnostics.Logging;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Grouping;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Zenject;

namespace Andoco.Unity.Framework.Misc
{
    [RequireComponent(typeof(GameObjectGroup))]
    public class Targetable : MonoBehaviour
    {
        [Inject]
        private IStructuredLog log;

        private GameObjectGroup groups;
        private PeerGroup peerGroup;

        [Tooltip("Can this target currently be targeted")]
        public bool isTargetable = true;

        [ReadOnly]
        [Tooltip("True if the Targetable is currently being targeted")]
        public bool targeted;

        [Tooltip("This target acts as a proxy for another Targetable")]
        public bool isProxy;

        [Tooltip("The group key that the proxied targetables are grouped under")]
        public string proxiedGroupKey = "ProxiedTarget";

        [Tooltip("This target is proxied by another Targetable")]
        public bool isProxied;

        [Tooltip("This target is part of a targetable peer group")]
        public bool isPeer;

        public UnityEvent targetHighlightBeginEvent;
        public UnityEvent targetHighlightEndEvent;

        void Awake()
        {
            this.groups = this.GetComponent<GameObjectGroup>();
            this.peerGroup = this.GetComponent<PeerGroup>();
        }

        public void AddProxiedTarget(GameObject target)
        {
            Assert.IsTrue(this.isProxy);

            this.groups.Add(target, this.proxiedGroupKey);
        }

        public void Targeted(bool value)
        {
            if (value == this.targeted)
                return;

            this.targeted = value;
            
            if (this.targeted)
            {
                this.targetHighlightBeginEvent.Invoke();
            }
            else
            {
                this.targetHighlightEndEvent.Invoke();
            }
        }

        public void FillTargets(IList<GameObject> targets)
        {
            Assert.IsNotNull(targets);

            if (!this.isTargetable)
                return;

            if (this.isPeer && peerGroup != null && peerGroup.IsPeer)
            {
                var peers = peerGroup.Peers;

                for (int i = 0; i < peers.Count; i++)
                {
                    targets.Add(peers[i].gameObject);
                }
            }
            else if (this.isProxy)
            {
                this.log.Trace("Filling targets from proxied target group", x => x
                    .Field("proxiedGroupKey", this.proxiedGroupKey));
                
                this.groups.Fill(targets, this.proxiedGroupKey);
            }
            else if (!this.isProxied)
            {
                this.log.Trace("Adding self as target", x => {});
                targets.Add(this.gameObject);
            }
        }
    }
}
