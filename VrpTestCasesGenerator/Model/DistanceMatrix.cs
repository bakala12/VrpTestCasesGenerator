namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents a distance matrix of the VRP problems.
    /// </summary>
    public class DistanceMatrix
    {
        private readonly double[,] _distances;

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
    }
}