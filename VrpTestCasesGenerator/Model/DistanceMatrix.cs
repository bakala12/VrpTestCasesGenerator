namespace VrpTestCasesGenerator.Model
{
    public class DistanceMatrix
    {
        private readonly double[,] _distances;

        public int Dimension { get; }

        public DistanceMatrix(int dimension)
        {
            Dimension = dimension;
            _distances = new double[dimension, dimension];
        }

        public double this[int from, int to]
        {
            get => _distances[from, to];
            set => _distances[from, to] = value;
        }
    }
}