using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EditDistance
{
    /// <summary>
    /// Levenshtein
    /// </summary>
    public class WagnerFischer
    {
        #region Public-Members

        /// <summary>
        /// String 1.
        /// </summary>
        public string String1
        {
            get
            {
                return _String1;
            }
        }

        /// <summary>
        /// String 2.
        /// </summary>
        public string String2
        {
            get
            {
                return _String2;
            }
        }

        /// <summary>
        /// Edit distance.
        /// </summary>
        public int EditDistance
        {
            get
            {
                return _Matrix[_Rows - 1, _Columns - 1];
            }
        }

        #endregion

        #region Private-Members

        private string _String1 = null;
        private string _String2 = null;
        private char[] _CharArray1 = null;
        private char[] _CharArray2 = null;
        private int _Rows = 0;
        private int _Columns = 0;
        private int[,] _Matrix = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public WagnerFischer(string s1, string s2)
        {
            if (String.IsNullOrEmpty(s1)) throw new ArgumentNullException(nameof(s1));
            if (String.IsNullOrEmpty(s2)) throw new ArgumentNullException(nameof(s2));

            _String1 = s1;
            _String2 = s2;

            _CharArray1 = _String1.ToCharArray();
            _CharArray2 = _String2.ToCharArray();

            _Rows = _CharArray1.Length + 2;
            _Columns = _CharArray2.Length + 2;

            GenerateMatrix();
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Calculate the distance from string 1 to string 2.
        /// </summary>
        /// <param name="s1">String 1.</param>
        /// <param name="s2">String 2.</param>
        /// <returns>Distance.</returns>
        public void PopulateMatrix()
        {
            for (int row = 2; row < _Rows; row++)
            {
                for (int col = 2; col < _Columns; col++)
                {
                    // Console.Write("Row " + row + " Column " + col + ": " + _Matrix[row, 0] + " and " + _Matrix[0, col] + " ");

                    if (_Matrix[row, 0] == _Matrix[0, col])
                    {
                        // Console.WriteLine("same");

                        _Matrix[row, col] = GetMinimum(
                            _Matrix[row - 1, col - 1],    // diag
                            _Matrix[row, col - 1] + 1,    // left
                            _Matrix[row - 1, col] + 1     // up
                            );
                    }
                    else
                    {
                        // Console.WriteLine("*** different");

                        _Matrix[row, col] = GetMinimum(
                            _Matrix[row - 1, col - 1] + 1,  // diag
                            _Matrix[row, col - 1] + 1,      // left
                            _Matrix[row - 1, col] + 1       // up
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Return a human-readable version of the matrix.
        /// </summary>
        /// <returns>String.</returns>
        public string DisplayMatrix()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _Rows; i++)
            {
                for (int j = 0; j < _Columns; j++)
                {
                    sb.Append(_Matrix[i, j].ToString().PadLeft(4, ' '));
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Private-Methods

        private void GenerateMatrix()
        {
            int rows = _String1.Length + 2;
            int cols = _String2.Length + 2;

            _Matrix = new int[rows, cols];

            // row zero contains str2
            // col zero contains str1

            // set everything to zero
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    _Matrix[row, col] = 0;
                }
            }

            // set str1 values in column 0
            for (int c = 0; c < _String1.Length; c++)
            {
                _Matrix[(c + 2), 0] = (int)(_String1[c]);
            }

            // set str2 values in rows 0
            for (int c = 0; c < _String2.Length; c++)
            {
                _Matrix[0, (c + 2)] = (int)(_String2[c]);
            }

            // set base 1..n values
            for (int row = 1; row < rows; row++)
            {
                _Matrix[row, 1] = (row - 1);
            }

            for (int col = 1; col < cols; col++)
            {
                _Matrix[1, col] = (col - 1);
            }
        }

        private int GetMinimum(int upperLeft, int left, int above)
        {
            return new int[] { upperLeft, left, above }.Min();
        }

        #endregion
    }
}