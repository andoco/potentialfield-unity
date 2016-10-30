namespace Andoco.Unity.Framework.Core
{
    using System;

    public interface IThreadingSystem
    {
        /// <summary>
        /// Schedules work to be performed on a background thread.
        /// </summary>
        /// <param name="work">Action to perform the work.</param>
        /// <param name="callback">Action to be performed on the main thread when the work has completed.</param>
        void Schedule(Action work, Action callback = null);

        /// <summary>
        /// Schedules work to be performed on the main thread.
        /// </summary>
        /// <param name="work">Action to perform the work.</param>
        void ScheduleMain(Action work);
    }
}
