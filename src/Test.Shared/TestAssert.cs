namespace EditDistance.Test.Shared
{
    using System;

    /// <summary>
    /// Minimal assertion helpers used by the shared Touchstone test cases.
    /// Test cases signal failure by throwing; these helpers wrap the common
    /// comparisons so the same assertions run identically under every runner
    /// (console, xUnit, and NUnit).
    /// </summary>
    public static class TestAssert
    {
        #region Public-Methods

        /// <summary>
        /// Throws if the condition is false.
        /// </summary>
        /// <param name="condition">Condition that must be true.</param>
        /// <param name="message">Failure message when condition is false.</param>
        /// <exception cref="InvalidOperationException">Thrown when condition is false.</exception>
        public static void True(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Assertion failed: " + message);
        }

        /// <summary>
        /// Throws if the condition is true.
        /// </summary>
        /// <param name="condition">Condition that must be false.</param>
        /// <param name="message">Failure message when condition is true.</param>
        /// <exception cref="InvalidOperationException">Thrown when condition is true.</exception>
        public static void False(bool condition, string message)
        {
            if (condition) throw new InvalidOperationException("Assertion failed: " + message);
        }

        /// <summary>
        /// Throws if expected and actual are not equal.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="message">Failure message.</param>
        /// <exception cref="InvalidOperationException">Thrown when values differ.</exception>
        public static void Equal<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    "Assertion failed: " + message + ". Expected=" + expected + ", Actual=" + actual);
            }
        }

        /// <summary>
        /// Throws if expected and actual are equal.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="notExpected">Value the actual must not equal.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="message">Failure message.</param>
        /// <exception cref="InvalidOperationException">Thrown when values are equal.</exception>
        public static void NotEqual<T>(T notExpected, T actual, string message)
        {
            if (object.Equals(notExpected, actual))
            {
                throw new InvalidOperationException(
                    "Assertion failed: " + message + ". Did not expect=" + notExpected + ", Actual=" + actual);
            }
        }

        /// <summary>
        /// Throws if the value is null.
        /// </summary>
        /// <param name="value">Value that must not be null.</param>
        /// <param name="message">Failure message when the value is null.</param>
        /// <exception cref="InvalidOperationException">Thrown when the value is null.</exception>
        public static void NotNull(object value, string message)
        {
            if (value == null) throw new InvalidOperationException("Assertion failed: " + message + ". Value was null.");
        }

        /// <summary>
        /// Executes the action and verifies it throws an exception assignable to TException.
        /// </summary>
        /// <typeparam name="TException">Expected exception type.</typeparam>
        /// <param name="action">Action expected to throw.</param>
        /// <param name="message">Failure message if no exception or wrong type is thrown.</param>
        /// <exception cref="InvalidOperationException">Thrown when the expected exception does not occur.</exception>
        public static void Throws<TException>(Action action, string message) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Assertion failed: " + message + ". Expected " + typeof(TException).Name +
                    ", got " + ex.GetType().Name + ": " + ex.Message);
            }

            throw new InvalidOperationException(
                "Assertion failed: " + message + ". Expected " + typeof(TException).Name +
                " but no exception was thrown.");
        }

        #endregion
    }
}
