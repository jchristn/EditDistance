namespace EditDistance.Test.Nunit
{
    using System.Collections;
    using System.Threading;
    using System.Threading.Tasks;

    using EditDistance.Test.Shared;
    using Touchstone.Core;
    using Touchstone.NunitAdapter;
    using global::NUnit.Framework;

    /// <summary>
    /// NUnit host for the EditDistance Touchstone suite. Each shared
    /// <see cref="TestCaseDescriptor"/> is enumerated as a distinct NUnit test case. The test
    /// logic itself lives entirely in <see cref="EditDistanceSuites"/> - this class is only the
    /// framework binding.
    /// </summary>
    [TestFixture]
    public sealed class WagnerFischerNunitTests
    {
        /// <summary>
        /// Enumerates every non-skipped shared Touchstone test case.
        /// </summary>
        /// <returns>Enumerator of <see cref="TestCaseDescriptor"/> values.</returns>
        public static IEnumerable TestCases()
        {
            return new TouchstoneTestCaseSource(EditDistanceSuites.All);
        }

        /// <summary>
        /// Executes a single Touchstone test case under NUnit.
        /// </summary>
        /// <param name="testCase">The test case to execute.</param>
        /// <returns>Task representing the test run.</returns>
        [Test]
        [TestCaseSource(nameof(TestCases))]
        public async Task RunTouchstoneCase(TestCaseDescriptor testCase)
        {
            await testCase.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
