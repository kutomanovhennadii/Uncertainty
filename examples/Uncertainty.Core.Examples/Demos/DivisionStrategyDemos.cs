using System;
using Uncertainty.Core;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Examples.Demos
{
    public static class DivisionStrategyDemos
    {
        public static void RunAll()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            UncertaintyPolicies.SetDivisionTolerance(1e-307);

            Console.WriteLine("--- Demo: ThrowOnSmallDenominator ---");
            UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
            try
            {
                var r = UDouble.Divide(a, b);
                Console.WriteLine($"Result: mean={r.Mean}, variance={r.Variance}");
            }
            catch (DivideByZeroException ex)
            {
                // More specific exception handling (CA1031: avoid catching System.Exception)
                Console.WriteLine($"Thrown: {ex.Message}");
            }

            Console.WriteLine("--- Demo: SaturateVariance ---");
            UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;
            var r2 = UDouble.Divide(a, b);
            Console.WriteLine($"Result: mean={r2.Mean}, variance={r2.Variance}");

            Console.WriteLine("--- Demo: ReturnInfinityMean (fallback) ---");
            UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;
            var r3 = UDouble.Divide(a, b);
            Console.WriteLine($"Result: mean={r3.Mean}, variance={r3.Variance}");

            Console.WriteLine("--- Demo: Clamping behavior (illustration) ---");
            // Custom strategies are internal implementation details; clients should use DivisionBehavior.
            UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;
            var r4 = UDouble.Divide(a, b);
            Console.WriteLine($"Result: mean={r4.Mean}, variance={r4.Variance}");
        }
    }
}