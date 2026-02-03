using System;
using System.Threading;
using Uncertainty.Core.Policies.DivisionPolicies;
using DivisionStrategies = Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Policies
{
    /// <summary>
    /// Policies for numeric behavior in the Uncertainty.Core arithmetic operations.
    /// 
    /// Thread-safe singleton facade for managing global numeric policies. Safe to call from both
    /// synchronous and asynchronous contexts without causing deadlocks or thread pool starvation.
    /// Uses SemaphoreSlim for thread-safe synchronization.
    /// </summary>
    public static class UncertaintyPolicies
    {
        #region Private Fields
        private static DivisionBehavior _divisionBehavior;
        private static IDivisionStrategy? _divisionStrategy;
        private static VarianceSaturationOptions _varianceSaturation = VarianceSaturationOptions.Default;
        private static readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);
        #endregion

        #region Constructors
        static UncertaintyPolicies()
        {
            // Explicitly set defaults for clarity and to avoid relying on implicit language defaults.
            DivisionTolerance = 0.0;
            DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
        }
        #endregion

        #region Division Politics
        /// <summary>
        /// Tolerance used to treat small denominators as zero in division operations.
        /// Default is 0.0 to preserve exact behavior (<c>b.Mean == 0.0</c>).
        /// Use <see cref="SetDivisionTolerance(double)"/> to modify this value.
        /// 
        /// Safe to read from async contexts.
        /// </summary>
        public static double DivisionTolerance { get; private set; }

        /// <summary>
        /// Sets the division tolerance. Must be finite and &gt;= 0.
        /// 
        /// Thread-safe and safe to call from both sync and async contexts without deadlock risk.
        /// If called from an async context, uses SemaphoreSlim internally for synchronization.
        /// </summary>
        public static void SetDivisionTolerance(double tolerance)
        {
            if (double.IsNaN(tolerance) || double.IsInfinity(tolerance))
                throw new ArgumentException("Division tolerance must be finite.", nameof(tolerance));
            if (tolerance < 0.0)
                throw new ArgumentOutOfRangeException(nameof(tolerance), "Division tolerance must be ≥ 0.");

            _sync.Wait();
            try
            {
                DivisionTolerance = tolerance;
            }
            finally
            {
                _sync.Release();
            }
        }

        /// <summary>
        /// Specifies how division by a very small denominator should behave.
        /// Setting this will also update the <see cref="DivisionStrategy"/> to a default implementation.
        /// 
        /// Thread-safe and safe to call from both sync and async contexts without deadlock risk.
        /// </summary>
        public static DivisionBehavior DivisionBehavior
        {
            get => _divisionBehavior;
            set
            {
                if (!Enum.IsDefined(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid DivisionBehavior value.");

                _sync.Wait();
                try
                {
                    _divisionBehavior = value;

                    // Map enum to default strategy implementations.
                    _divisionStrategy = value switch
                    {
                        DivisionBehavior.ThrowOnSmallDenominator => new DivisionStrategies.ThrowingDivisionStrategy(),
                        DivisionBehavior.SaturateVariance => new DivisionStrategies.SaturatingDivisionStrategy(),
                        DivisionBehavior.ReturnInfinityMean => new DivisionStrategies.ReturnInfinityDivisionStrategy(),
                        _ => new DivisionStrategies.ThrowingDivisionStrategy()
                    };
                }
                finally
                {
                    _sync.Release();
                }
            }
        }

        /// <summary>
        /// Division strategy used when a denominator is considered "small" according to <see cref="DivisionTolerance"/>.
        /// Internal property; users should configure division behavior via <see cref="DivisionBehavior"/> enum.
        /// </summary>
        internal static IDivisionStrategy DivisionStrategy
        {
            get => _divisionStrategy ??= new DivisionStrategies.ThrowingDivisionStrategy();
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _sync.Wait();
                try
                {
                    _divisionStrategy = value;

                    // Update enum for backward compatibility when possible using internal mapper.
                    if (value is DivisionStrategies.IMappedDivisionBehavior mapper)
                    {
                        _divisionBehavior = mapper.MappedBehavior;
                    }
                }
                finally
                {
                    _sync.Release();
                }
            }
        }
        #endregion

        #region Variance Saturation Politics
        /// <summary>
        /// Current variance saturation options used by the library.
        /// 
        /// Safe to read from async contexts.
        /// </summary>
        public static VarianceSaturationOptions VarianceSaturation
        {
            get
            {
                _sync.Wait();
                try
                {
                    return _varianceSaturation;
                }
                finally
                {
                    _sync.Release();
                }
            }
        }

        /// <summary>
        /// Configure variance saturation runtime options. Validates input before applying.
        /// 
        /// Thread-safe and safe to call from both sync and async contexts without deadlock risk.
        /// Uses SemaphoreSlim for synchronization, compatible with async/await patterns.
        /// </summary>
        public static void ConfigureVarianceSaturation(VarianceSaturationOptions options)
        {
            if (options.Equals(default(VarianceSaturationOptions)))
                throw new ArgumentException("Options must be provided.", nameof(options));

            options.EnsureValid();

            _sync.Wait();
            try
            {
                _varianceSaturation = options;
            }
            finally
            {
                _sync.Release();
            }
        }
        #endregion
    }
}