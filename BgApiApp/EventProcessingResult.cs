using System;

namespace BgApiApp
{
    internal enum EventProcessingResult
    {
        /// <summary>
        /// Processing completed
        /// </summary>
        Complete,
        /// <summary>
        /// Skip the event
        /// </summary>
        Skip,
        /// <summary>
        /// The event was processed
        /// </summary>
        Processed
    }
}
