using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IVrpGenerator
    {
        Task<VrpProblem> Generate(GeneratorParameters parameters);
    }
}