namespace Andoco.Unity.Framework.Core
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    [Serializable]
    public class TransformEvent : UnityEvent<Transform> { }

    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    [Serializable]
    public class CollisionEvent : UnityEvent<Collision> { }
}

