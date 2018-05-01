using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Generator
{
    public interface IVrpGenerator
    {
        VrpProblem Generate(GeneratorParameters parameters);
    }
}