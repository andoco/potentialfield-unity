namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Core.Pooling;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class GameObjectGroup : MonoBehaviour
    {
        [Inject]
        private readonly IStructuredLog log;

        [Inject]
        private GameObjectGroupSignal groupSignal;

        private List<GroupMember> groupMembers = new List<GroupMember>();

        private List<int> indicesToRemove = new List<int>();

        public int Count { get { return this.groupMembers.Count; } }

        #region Lifecycle

        void Recycled()
        {
            this.Clear();
        }

        #endregion

        public void Add(GameObject go, string group)
        {
            this.groupMembers.Add(this.MakeMember(go, group));

            if (this.log.Status.IsInfoEnabled)
                this.log.Info("Added GameObject to group", x => x
                    .Field("addedGameObject", go)
                    .Field("group", group));
        }

        public void Clear()
        {
            this.groupMembers.Clear();
        }

        public bool IsEmpty(string group = null)
        {
            if (this.groupMembers.Count == 0)
                return true;

            for (int i = 0; i < this.groupMembers.Count; i++)
            {
                var member = this.groupMembers[i];

                if (member.IsValid() && member.HasGroup(group))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsInGroup(GameObject go, string group)
        {
            for (int i = 0; i < this.groupMembers.Count; i++)
            {
                var member = this.groupMembers[i];

                if (member.IsValid() && member.Matches(go, group))
                {
                    return true;
                }
            }

            return false;
        }

        public void Fill(IList<GameObject> members, string group)
        {
            for (int i = 0; i < this.groupMembers.Count; i++)
            {
                var member = this.groupMembers[i];

                if (member.IsValid() && member.HasGroup(group))
                {
                    members.Add(member.gameobject);
                }
            }
        }

        public IList<GameObject> GetInGroup(string group)
        {
            var results = ListPool<GameObject>.Take();

            this.Fill(results, group);

            return results;
        }

        public void Remove(GameObject go, string group = null)
        {
            Assert.AreEqual(0, this.indicesToRemove.Count);

            if (this.log.Status.IsInfoEnabled)
                this.log.Info("Removing GameObject from group", x => x
                    .Field("removedGameObject", go)
                    .Field("group", group));

            for (int i = 0; i < this.groupMembers.Count; i++)
            {
                var member = this.groupMembers[i];

                if (member.gameobject == go && (group == null || member.HasGroup(group)))
                {
                    this.indicesToRemove.Add(i);
                }
            }

            this.Remove(this.indicesToRemove);

            this.indicesToRemove.Clear();

            this.groupSignal.Dispatch(new GameObjectGroupSignal.Data { name = group, group = this });
        }

        public void Remove(string group)
        {
            Assert.IsFalse(string.IsNullOrEmpty(group));

            if (this.log.Status.IsInfoEnabled)
                this.log.Info("Removing all GameObjects in group", x => x.Field("group", group));

            for (int i = 0; i < this.groupMembers.Count; i++)
            {
                var member = this.groupMembers[i];

                if (member.HasGroup(group))
                {
                    this.indicesToRemove.Add(i);
                }
            }

            this.Remove(this.indicesToRemove);

            this.indicesToRemove.Clear();
        }

        public void RemoveDisposed()
        {
            for (var i = this.groupMembers.Count - 1; i >= 0; i--)
            {
                if (!this.groupMembers[i].IsValid())
                {
                    this.indicesToRemove.Add(i);
                }
            }

            this.Remove(this.indicesToRemove);
            this.indicesToRemove.Clear();
        }

        private GroupMember MakeMember(GameObject go, string group)
        {
            return new GroupMember {
                groupName = group,
                gameobject = go
            };
        }

        private void Remove(IList<int> indicesToRemove)
        {
            if (this.indicesToRemove.Count == 0)
                return;

            this.indicesToRemove.Sort();

            for (int i = this.indicesToRemove.Count -1; i >= 0; i--)
            {
                // Get the group name of the member being removed.
                var name = this.groupMembers[this.indicesToRemove[i]].groupName;

                this.groupMembers.RemoveAt(this.indicesToRemove[i]);

                // TODO: Only signal once per group.
                this.groupSignal.Dispatch(new GameObjectGroupSignal.Data { name = name, group = this });
            }
        }

        #region Types

        [Serializable]
        public class GroupMember
        {
            public string groupName;
            public GameObject gameobject;

            public bool Matches(GameObject go, string group)
            {
                return this.gameobject == go && this.groupName.Equals(group, StringComparison.OrdinalIgnoreCase);
            }

            public bool HasGroup(string group)
            {
                return groupName.Equals(group, StringComparison.OrdinalIgnoreCase);
            }

            public bool IsValid()
            {
                return ObjectValidator.Validate(this.gameobject);
            }
        }

        #endregion
    }
}
