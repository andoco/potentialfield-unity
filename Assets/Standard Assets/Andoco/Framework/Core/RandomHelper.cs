namespace Andoco.Unity.Framework.Core
{
    using System.Collections.Generic;
    using Andoco.Core;
    using UnityEngine;

    public static class RandomHelper
    {
        static readonly List<Random.State> states = new List<Random.State>();

        /// <summary>
        /// Pushes the current <see cref="Random.State"/> onto the state stack.
        /// </summary>
        public static void PushState()
        {
            states.Push(Random.state);
        }

        /// <summary>
        /// Pushes the current <see cref="Random.State"/> onto the state stack and sets the supplied seed.
        /// </summary>
        /// <param name="seed">Seed.</param>
        public static void PushSeed(int seed)
        {
            PushState();
            Random.InitState(seed);
        }

        /// <summary>
        /// Pushes the current <see cref="Random.State"/> onto the state stack and sets a new seed.
        /// </summary>
        public static void PushReseed()
        {
            PushState();
            Reseed();
        }

        /// <summary>
        /// Pops a <see cref="Random.State"/> off the stack and sets as the current state.
        /// </summary>
        public static void PopState()
        {
            Random.state = states.Pop();
        }

        /// <summary>
        /// Reseeds the RNG based on the current system tick count.
        /// </summary>
        public static void Reseed()
        {
            Random.InitState(System.Environment.TickCount);
        }
    }
}

