using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Core;
using UnityEngine;

public class PlayAudio : ActionTask
{
    private readonly AudioSystem audioSystem;

    public PlayAudio(ITaskIdBuilder id, AudioSystem audioSystem)
        : base(id)
    {
        this.audioSystem = audioSystem;
    }

    public string Key { get; set; }

    public override TaskResult Run(ITaskNode node)
    {
        var audioBoard = node.Context.GetComponent<AudioBoard>();
        this.audioSystem.PlayClip(audioBoard, this.Key);

        return TaskResult.Success;
    }
}
