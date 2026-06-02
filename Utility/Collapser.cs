namespace WorkoutTracker.Utility
{
    public class Collapser<TResult>
    {
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private long windowInTicks;
        private long nextRun;
        private TResult lastResult;

        public Collapser(TimeSpan window)
        {
            this.windowInTicks = window.Ticks;
        }

        public async Task<TResult> ExecuteAsync(Func<CancellationToken, Task<TResult>> innerAction, CancellationToken cancellationToken)
        {
            long requestStart = DateTime.UtcNow.Ticks;

            try
            {
                await semaphore.WaitAsync();

                if (requestStart <= nextRun)
                {
                    return this.lastResult;
                }

                this.lastResult = await innerAction(cancellationToken);

                this.nextRun = requestStart + windowInTicks;
                return lastResult;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
