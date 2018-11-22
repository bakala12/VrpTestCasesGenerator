namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents a distance matrix of the VRP problems.
    /// </summary>
    public class DistanceMatrix
    {
        private readonly double[,] _distances;
        private readonly int[,] _crossingCounts;

        /// <summary>
        /// Gets the dimension of the (square) matrix.
        /// </summary>
        public int Dimension { get; }

        /// <summary>
        /// Initializes a new instance of distance matrix.
        /// </summary>
        /// <param name="dimension">Dimension of the matrix.</param>
        public DistanceMatrix(int dimension)
        {
            Dimension = dimension;
            _distances = new double[dimension, dimension];
            _crossingCounts = new int[dimension,dimension];
        }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <param name="from">Source vertex.</param>
        /// <param name="to">Destination vertex.</param>
        /// <returns></returns>
        public double this[int from, int to]
        {
            get => _distances[from, to];
            set => _distances[from, to] = value;
        }

        /// <summary>
        /// Gets number of crossing between points.
        /// </summary>
        /// <param name="from">Source vertex.</param>
        /// <param name="to">Destination vertex.</param>
        /// <returns>Number of crossings between two vertices.</returns>
        public int GetCrossingCount(int from, int to) => _crossingCounts[from, to];

        /// <summary>
        /// Sets number of crossing between points.
        /// </summary>
        /// <param name="from">Source vertex.</param>
        /// <param name="to">Destination vertex.</param>
        /// <param name="value">Number of crossings.</param>
        public void SetCrossingCount(int from, int to, int value) => _crossingCounts[from, to] = value;
    }
}