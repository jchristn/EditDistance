namespace EditDistance.Test.Xunit
{
    using System.Threading;
    using System.Threading.Tasks;

    using EditDistance.Test.Shared;
    using Touchstone.Core;
    using Touchstone.XunitAdapter;
    using global::Xunit;

    /// <summary>
    /// xUnit host for the EditDistance Touchstone suite. Each shared
    /// <see cref="TestCaseDescriptor"/> is surfaced as its own xUnit theory row, so failures
    /// are reported one-per-test in IDE and CI output. The test logic itself lives entirely in
    /// <see cref="EditDistanceSuites"/> - this class is only the framework binding.
    /// </summary>
    public sealed class WagnerFischerTheoryTests
    {
        /// <summary>
        /// Every non-skipped shared test case, projected as xUnit theory data.
        /// </summary>
        public static TouchstoneTheoryData TestCases
        {
            get { return new TouchstoneTheoryData(EditDistanceSuites.All); }
        }

        /// <summary>
        /// Executes a single Touchstone test case under xUnit.
        /// </summary>
        /// <param name="testCase">The test case to execute.</param>
        /// <returns>Task representing the test run.</returns>
        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTouchstoneCase(TestCaseDescriptor testCase)
        {
            await testCase.ExecuteAsync(CancellationToken.None);
        }
    }
}
