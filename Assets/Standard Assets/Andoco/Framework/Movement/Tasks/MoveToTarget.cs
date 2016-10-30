using System;
using System.Collections.Generic;
using Andoco.BehaviorTree;
using Andoco.Unity.Framework.Misc;
using Andoco.Unity.Framework.Movement.Objectives;
using UnityEngine;

namespace Andoco.Unity.Framework.Movement.Tasks
{
    public class MoveToTarget : Move
    {
        public MoveToTarget(ITaskIdBuilder id)
            : base(id)
        {
        }

        /// <summary>
        /// The key of the GameObjectGroup group to get the target from.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The name of the move driver to use to perform the movement.
        /// </summary>
        public string Mover { get; set; }

        public float Speed { get; set; }

        protected override void OnPrepareMovement(MoveState state)
        {
            state.Mover.SetDriver(this.Mover);

            var groups = state.Mover.GetComponent<GameObjectGroup>();

            var members = new List<GameObject>();
            groups.Fill(members, this.Group);

            if (members.Count > 0)
            {
                var targetGo = members[0];

                if (this.Speed > 0f)
                {
                    state.Mover.Driver.Speed = this.Speed;
                }

                state.Mover.Driver.MoveTo(new Objective(targetGo.transform));

                state.willMove = true;
                state.readyToMove = true;
            }
            else
            {
                state.willMove = false;
            }
        }
    }
}

