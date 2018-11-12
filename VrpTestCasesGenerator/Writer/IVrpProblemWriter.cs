using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Writer
{
    /// <summary>
    /// Provides a way to save VRP problem instance to a file.
    /// </summary>
    public interface IVrpProblemWriter
    {
        /// <summary>
        /// Saves VRP problem instance to a file.
        /// </summary>
        /// <param name="problem">VRP problem instance.</param>
        /// <param name="file">Path to file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Write(VrpProblem problem, string file);
    }
}