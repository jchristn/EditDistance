namespace EditDistance.Test.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Touchstone.Core;

    /// <summary>
    /// Central source of truth for the EditDistance test suite.
    ///
    /// Every test case is defined here once as a Touchstone <see cref="TestCaseDescriptor"/>
    /// and is executed unchanged by every runner: the console runner (Test.Automated),
    /// the xUnit host (Test.Xunit), and the NUnit host (Test.Nunit).
    ///
    /// Coverage spans the entire public surface of <see cref="WagnerFischer"/>:
    /// constructor argument validation (negative cases), the String1/String2/EditDistance
    /// members, PopulateMatrix, DisplayMatrix, and the mathematical properties of the
    /// Levenshtein (Wagner-Fischer) edit distance it computes.
    /// </summary>
    public static class EditDistanceSuites
    {
        #region Public-Members

        /// <summary>
        /// The complete set of test suites, in execution order.
        /// </summary>
        public static IReadOnlyList<TestSuiteDescriptor> All
        {
            get
            {
                return new List<TestSuiteDescriptor>
                {
                    ConstructorValidationSuite(),
                    MemberAccessorSuite(),
                    IdenticalStringSuite(),
                    SingleEditOperationSuite(),
                    ClassicLevenshteinSuite(),
                    CaseAndCharacterSuite(),
                    MigratedLegacySuite(),
                    AlgorithmPropertySuite(),
                    DisplayMatrixSuite()
                };
            }
        }

        #endregion

        #region Suites

        /// <summary>
        /// Negative cases: the constructor rejects null or empty inputs for either argument.
        /// </summary>
        private static TestSuiteDescriptor ConstructorValidationSuite()
        {
            const string id = "ConstructorValidation";

            return new TestSuiteDescriptor(
                id,
                "Constructor argument validation (negative cases)",
                new List<TestCaseDescriptor>
                {
                    ThrowsCase(id, "NullFirst",    "null, \"value\" throws ArgumentNullException",  null,   "value"),
                    ThrowsCase(id, "NullSecond",   "\"value\", null throws ArgumentNullException",   "value", null),
                    ThrowsCase(id, "NullBoth",     "null, null throws ArgumentNullException",        null,   null),
                    ThrowsCase(id, "EmptyFirst",   "\"\", \"value\" throws ArgumentNullException",   "",     "value"),
                    ThrowsCase(id, "EmptySecond",  "\"value\", \"\" throws ArgumentNullException",   "value", ""),
                    ThrowsCase(id, "EmptyBoth",    "\"\", \"\" throws ArgumentNullException",        "",     ""),
                    ThrowsCase(id, "NullEmpty",    "null, \"\" throws ArgumentNullException",        null,   ""),
                    ThrowsCase(id, "EmptyNull",    "\"\", null throws ArgumentNullException",        "",     null)
                });
        }

        /// <summary>
        /// Positive cases: the public members expose the inputs and the computed distance.
        /// </summary>
        private static TestSuiteDescriptor MemberAccessorSuite()
        {
            const string id = "MemberAccessors";

            return new TestSuiteDescriptor(
                id,
                "Public member accessors",
                new List<TestCaseDescriptor>
                {
                    Case(id, "String1", "String1 returns the first input verbatim", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        TestAssert.Equal("kitten", wf.String1, "String1");
                    }),
                    Case(id, "String2", "String2 returns the second input verbatim", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        TestAssert.Equal("sitting", wf.String2, "String2");
                    }),
                    Case(id, "InputsPreservedIndependently", "String1 and String2 are preserved independently", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("alpha", "omega");
                        TestAssert.Equal("alpha", wf.String1, "String1");
                        TestAssert.Equal("omega", wf.String2, "String2");
                    }),
                    Case(id, "DistanceAfterPopulate", "EditDistance reflects the DP result after PopulateMatrix", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        wf.PopulateMatrix();
                        TestAssert.Equal(3, wf.EditDistance, "EditDistance(kitten, sitting)");
                    }),
                    Case(id, "PopulateMatrixIsIdempotent", "Calling PopulateMatrix twice yields the same distance", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("gumbo", "gambol");
                        wf.PopulateMatrix();
                        int first = wf.EditDistance;
                        wf.PopulateMatrix();
                        int second = wf.EditDistance;
                        TestAssert.Equal(first, second, "EditDistance is stable across repeated PopulateMatrix calls");
                        TestAssert.Equal(2, second, "EditDistance(gumbo, gambol)");
                    })
                });
        }

        /// <summary>
        /// Identical strings always have an edit distance of zero.
        /// </summary>
        private static TestSuiteDescriptor IdenticalStringSuite()
        {
            const string id = "IdenticalStrings";

            return new TestSuiteDescriptor(
                id,
                "Identical strings have distance zero",
                DistanceCases(id, new[]
                {
                    Data("a", "a", 0),
                    Data("abc", "abc", 0),
                    Data("hello", "hello", 0),
                    Data("Levenshtein", "Levenshtein", 0),
                    Data("12345", "12345", 0),
                    Data("a b c", "a b c", 0),
                    Data("The quick brown fox", "The quick brown fox", 0)
                }));
        }

        /// <summary>
        /// Single insertion, deletion, or substitution yields a distance of exactly one.
        /// </summary>
        private static TestSuiteDescriptor SingleEditOperationSuite()
        {
            const string id = "SingleEditOperations";

            return new TestSuiteDescriptor(
                id,
                "Single insertion / deletion / substitution",
                DistanceCases(id, new[]
                {
                    // Substitutions.
                    Data("a", "b", 1),
                    Data("cat", "cot", 1),
                    Data("cat", "bat", 1),
                    // Insertions.
                    Data("cat", "cart", 1),   // insert 'r' in the middle
                    Data("cat", "cats", 1),   // insert 's' at the end
                    Data("cat", "scat", 1),   // insert 's' at the front
                    // Deletions.
                    Data("cart", "cat", 1),   // delete 'r'
                    Data("cats", "cat", 1),   // delete trailing 's'
                    Data("scat", "cat", 1),   // delete leading 's'
                    // Repeated characters.
                    Data("aaa", "aaaa", 1),
                    Data("aaaa", "aaa", 1)
                }));
        }

        /// <summary>
        /// Well-known textbook Levenshtein examples with independently verified distances.
        /// </summary>
        private static TestSuiteDescriptor ClassicLevenshteinSuite()
        {
            const string id = "ClassicExamples";

            return new TestSuiteDescriptor(
                id,
                "Classic Levenshtein examples",
                DistanceCases(id, new[]
                {
                    Data("kitten", "sitting", 3),
                    Data("Saturday", "Sunday", 3),
                    Data("sunday", "saturday", 3),
                    Data("flaw", "lawn", 2),
                    Data("gumbo", "gambol", 2),
                    Data("book", "back", 2),
                    Data("horse", "ros", 3),
                    Data("intention", "execution", 5),
                    Data("abc", "xyz", 3),       // three substitutions
                    Data("abc", "cba", 2),       // swap ends
                    Data("ab", "ba", 2),         // transposition costs two under Levenshtein
                    Data("a", "abc", 2),         // two insertions
                    Data("abc", "a", 2),         // two deletions
                    Data("12345", "54321", 4)    // reversed numeric string
                }));
        }

        /// <summary>
        /// Case sensitivity, whitespace, punctuation, and non-ASCII characters.
        /// </summary>
        private static TestSuiteDescriptor CaseAndCharacterSuite()
        {
            const string id = "CaseAndCharacters";

            return new TestSuiteDescriptor(
                id,
                "Case sensitivity and character handling",
                DistanceCases(id, new[]
                {
                    Data("ABC", "abc", 3),                       // case sensitive: all three differ
                    Data("Cat", "cat", 1),                       // only the leading letter differs
                    Data("HELLO", "hello", 5),
                    Data("café", "cafe", 1),                // 'é' -> 'e'
                    Data("naïve", "naive", 1),              // 'ï' -> 'i'
                    Data("hello world", "hello  world", 1),      // extra space inserted
                    Data("a.b.c", "a,b,c", 2),                   // punctuation substitutions
                    Data("(value)", "[value]", 2),               // bracket substitutions
                    Data("tab\tend", "tab end", 1)               // tab vs space
                }));
        }

        /// <summary>
        /// The exact cases carried over from the original MSTest suite, preserved verbatim so
        /// nothing is lost in the migration to Touchstone.
        /// </summary>
        private static TestSuiteDescriptor MigratedLegacySuite()
        {
            const string id = "MigratedLegacy";

            return new TestSuiteDescriptor(
                id,
                "Migrated legacy MSTest cases",
                new List<TestCaseDescriptor>
                {
                    // Original TestEmpty(...) data rows -> all expect ArgumentNullException.
                    ThrowsCase(id, "LegacyEmptyBoth",   "\"\", \"\" throws (legacy TestEmpty)",           "",          ""),
                    ThrowsCase(id, "LegacyOneEmpty",    "\"Something\", \"\" throws (legacy TestEmpty)",  "Something", ""),
                    ThrowsCase(id, "LegacyOtherEmpty",  "\"\", \"Other\" throws (legacy TestEmpty)",      "",          "Other"),

                    // Original TestLevenshteinDistance(...) data rows.
                    DistanceCase(id, "LegacyIdentical", "Example1",                              "Example1",                          0),
                    DistanceCase(id, "LegacyDeletion",  "Example1",                              "Example",                           1),
                    DistanceCase(id, "LegacyInsertion", "Example",                               "MyExample",                         2),
                    DistanceCase(id, "LegacyMultiple",  "ABCD",                                  "aBCde",                             3),
                    DistanceCase(id, "LegacyLongToken", "A very long token with many changes",   "A pretty long token with changes",  9)
                });
        }

        /// <summary>
        /// Mathematical invariants that must hold for any valid pair of inputs.
        /// </summary>
        private static TestSuiteDescriptor AlgorithmPropertySuite()
        {
            const string id = "AlgorithmProperties";

            string[][] pairs = new[]
            {
                new[] { "kitten", "sitting" },
                new[] { "flaw", "lawn" },
                new[] { "abc", "xyz" },
                new[] { "book", "back" },
                new[] { "intention", "execution" },
                new[] { "a", "abcdef" }
            };

            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>();

            foreach (string[] pair in pairs)
            {
                string s1 = pair[0];
                string s2 = pair[1];

                cases.Add(Case(id, "Symmetry_" + s1 + "_" + s2,
                    "Distance is symmetric for \"" + s1 + "\" and \"" + s2 + "\"", ct =>
                    {
                        int forward = Distance(s1, s2);
                        int backward = Distance(s2, s1);
                        TestAssert.Equal(forward, backward,
                            "Distance(" + s1 + "," + s2 + ") should equal Distance(" + s2 + "," + s1 + ")");
                    }));

                cases.Add(Case(id, "Bounds_" + s1 + "_" + s2,
                    "Distance stays within [|len1-len2|, max(len1,len2)] for \"" + s1 + "\" and \"" + s2 + "\"", ct =>
                    {
                        int distance = Distance(s1, s2);
                        int lower = Math.Abs(s1.Length - s2.Length);
                        int upper = Math.Max(s1.Length, s2.Length);
                        TestAssert.True(distance >= lower,
                            "Distance " + distance + " must be >= lower bound " + lower);
                        TestAssert.True(distance <= upper,
                            "Distance " + distance + " must be <= upper bound " + upper);
                    }));
            }

            // Non-negativity across a spread of inputs.
            cases.Add(Case(id, "NonNegative", "Distance is never negative", ct =>
            {
                TestAssert.True(Distance("anything", "different") >= 0, "Distance must be non-negative");
                TestAssert.True(Distance("x", "y") >= 0, "Distance must be non-negative");
            }));

            return new TestSuiteDescriptor(id, "Algorithm invariants", cases);
        }

        /// <summary>
        /// DisplayMatrix produces a human-readable rendering that changes once the matrix is populated.
        /// </summary>
        private static TestSuiteDescriptor DisplayMatrixSuite()
        {
            const string id = "DisplayMatrix";

            return new TestSuiteDescriptor(
                id,
                "DisplayMatrix rendering",
                new List<TestCaseDescriptor>
                {
                    Case(id, "NotEmptyBeforePopulate", "DisplayMatrix is non-empty before PopulateMatrix", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        string rendered = wf.DisplayMatrix();
                        TestAssert.NotNull(rendered, "DisplayMatrix output");
                        TestAssert.False(string.IsNullOrEmpty(rendered), "DisplayMatrix output must not be empty");
                    }),
                    Case(id, "IsMultiLine", "DisplayMatrix output contains line breaks", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        string rendered = wf.DisplayMatrix();
                        TestAssert.True(rendered.Contains(Environment.NewLine),
                            "DisplayMatrix output should be rendered across multiple lines");
                    }),
                    Case(id, "ChangesAfterPopulate", "DisplayMatrix reflects the populated matrix", ct =>
                    {
                        WagnerFischer wf = new WagnerFischer("kitten", "sitting");
                        string before = wf.DisplayMatrix();
                        wf.PopulateMatrix();
                        string after = wf.DisplayMatrix();
                        TestAssert.NotEqual(before, after,
                            "DisplayMatrix output should change once the matrix is populated");
                    })
                });
        }

        #endregion

        #region Private-Helpers

        /// <summary>
        /// Computes the edit distance for a pair of inputs using the full construct/populate/read flow.
        /// </summary>
        private static int Distance(string s1, string s2)
        {
            WagnerFischer wf = new WagnerFischer(s1, s2);
            wf.PopulateMatrix();
            return wf.EditDistance;
        }

        /// <summary>
        /// Builds a general-purpose synchronous test case from an action.
        /// </summary>
        private static TestCaseDescriptor Case(string suiteId, string caseId, string displayName, Action<CancellationToken> body)
        {
            return new TestCaseDescriptor(
                suiteId,
                caseId,
                displayName,
                ct =>
                {
                    body(ct);
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// Builds a test case asserting that <see cref="Distance"/> for the pair equals the expected value.
        /// </summary>
        private static TestCaseDescriptor DistanceCase(string suiteId, string caseId, string s1, string s2, int expected)
        {
            return new TestCaseDescriptor(
                suiteId,
                caseId,
                "\"" + s1 + "\" -> \"" + s2 + "\" == " + expected,
                ct =>
                {
                    int actual = Distance(s1, s2);
                    TestAssert.Equal(expected, actual, "Edit distance from \"" + s1 + "\" to \"" + s2 + "\"");
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// Builds a test case asserting the constructor throws <see cref="ArgumentNullException"/> for the pair.
        /// </summary>
        private static TestCaseDescriptor ThrowsCase(string suiteId, string caseId, string displayName, string s1, string s2)
        {
            return new TestCaseDescriptor(
                suiteId,
                caseId,
                displayName,
                ct =>
                {
                    TestAssert.Throws<ArgumentNullException>(
                        () => { WagnerFischer wf = new WagnerFischer(s1, s2); GC.KeepAlive(wf); },
                        displayName);
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// Expands an array of (s1, s2, expected) tuples into distance test cases with generated ids.
        /// </summary>
        private static List<TestCaseDescriptor> DistanceCases(string suiteId, DistanceDatum[] data)
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>();
            for (int i = 0; i < data.Length; i++)
            {
                DistanceDatum datum = data[i];
                string caseId = "Case" + (i + 1).ToString("D2");
                cases.Add(DistanceCase(suiteId, caseId, datum.S1, datum.S2, datum.Expected));
            }
            return cases;
        }

        /// <summary>
        /// Convenience factory for a distance datum.
        /// </summary>
        private static DistanceDatum Data(string s1, string s2, int expected)
        {
            return new DistanceDatum(s1, s2, expected);
        }

        /// <summary>
        /// Immutable (s1, s2, expected) triple describing one distance expectation.
        /// </summary>
        private sealed class DistanceDatum
        {
            public string S1 { get; }
            public string S2 { get; }
            public int Expected { get; }

            public DistanceDatum(string s1, string s2, int expected)
            {
                S1 = s1;
                S2 = s2;
                Expected = expected;
            }
        }

        #endregion
    }
}
