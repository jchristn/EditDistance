namespace EditDistance.Test.Automated
{
    using System;
    using System.Threading.Tasks;

    using EditDistance.Test.Shared;
    using Touchstone.Cli;

    /// <summary>
    /// Console runner for the EditDistance Touchstone test suite.
    ///
    /// Executes every shared suite defined in <see cref="EditDistanceSuites"/> and prints a
    /// colored, tabular summary. Exit code 0 means all tests passed; a non-zero exit code
    /// means at least one test failed. Pass <c>--results &lt;path&gt;</c> to additionally write
    /// a structured JSON results file.
    /// </summary>
    public static class Program
    {
        #region Entrypoint

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command-line arguments. Optional: <c>--results &lt;path&gt;</c>.</param>
        /// <returns>Process exit code: 0 on success, non-zero on failure.</returns>
        public static async Task<int> Main(string[] args)
        {
            string resultsPath = null;

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], "--results", StringComparison.OrdinalIgnoreCase))
                {
                    resultsPath = args[i + 1];
                    break;
                }
            }

            int exitCode = await ConsoleRunner.RunAsync(EditDistanceSuites.All, null, resultsPath).ConfigureAwait(false);
            return exitCode;
        }

        #endregion
    }
}
