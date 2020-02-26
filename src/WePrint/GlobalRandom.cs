using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WePrint
{
    /// <summary>
    /// Helper (non-cryptographic) random number generator available globally.
    /// </summary>
    public static class GlobalRandom
    {
        private static ThreadLocal<Random> ThreadLocalRng { get; } = new ThreadLocal<Random>(() => new Random());
        public static Random Rng => ThreadLocalRng.Value;

        public static int Next() => Rng.Next();
        public static int Next(int exclusiveMax) => Rng.Next(exclusiveMax);
        public static int Next(int inclusiveMin, int exclusiveMax) => Rng.Next(inclusiveMin, exclusiveMax);

    }
}
