using System;
using System.Runtime.ExceptionServices;

namespace SpecLight.Infrastructure
{
    internal class StepOutcome
    {
        public StepOutcome(Step step)
        {
            Step = step;
        }

        public Step Step { get; private set; }
        public Status Status { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public Exception Error { get; set; }
        public ExceptionDispatchInfo ExceptionDispatchInfo { get; set; }

        /// <summary>
        /// Was the passing method actually devoid of code?
        /// </summary>
        public bool Empty { get; set; }

        /// <summary>
        /// Did the step throw an exception, but Catch was used to expect it?
        /// </summary>
        public bool ExceptionCaught { get; set; }

        /// <summary>
        /// Should we start skipping steps now?
        /// </summary>
        public bool CausesSkip => Status == Status.Failed || Status == Status.Pending;
    }
}
