using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HurlStudio.Utility
{
    /// <summary>
    /// see https://stackoverflow.com/questions/7612602/why-cant-i-use-the-await-operator-within-the-body-of-a-lock-statement
    /// </summary>
    public class SemaphoreLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task LockAsync(Func<Task> worker)
        {
            var isTaken = false;
            try
            {
                do
                {
                    try
                    {
                    }
                    finally
                    {
                        isTaken = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1));
                    }
                }
                while (!isTaken);
                await worker();
            }
            finally
            {
                if (isTaken)
                {
                    _semaphore.Release();
                }
            }
        }

        // overloading variant for non-void methods with return type (generic T)
        public async Task<T> LockAsync<T>(Func<Task<T>> worker)
        {
            var isTaken = false;
            try
            {
                do
                {
                    try
                    {
                    }
                    finally
                    {
                        isTaken = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1));
                    }
                }
                while (!isTaken);
                return await worker();
            }
            finally
            {
                if (isTaken)
                {
                    _semaphore.Release();
                }
            }
        }
    }
}
