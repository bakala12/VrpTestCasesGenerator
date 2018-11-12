using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    /// <summary>
    /// Provides a way to generate random map VRP problem instance.
    /// </summary>
    public interface IVrpGenerator
    {
        /// <summary>
        /// Generates random map VRP problem instance.
        /// </summary>
        /// <param name="parameters">Problem parameters.</param>
        /// <returns>A task that represents asynchronous operation.</returns>
        Task<VrpProblem> Generate(GeneratorParameters parameters);
    }
}