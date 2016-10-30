namespace Andoco.Unity.Framework.Core
{
    public interface ITickedReceipt
    {
        /// <summary>
        /// Cancel the ticked action for this receipt.
        /// </summary>
        /// <remarks
        /// Once cancelled a ticked action cannot be reused.
        /// </remarks>
        void Cancel();
    }
}