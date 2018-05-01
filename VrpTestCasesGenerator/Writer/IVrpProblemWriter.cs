using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Writer
{
    public interface IVrpProblemWriter
    {
        void Write(VrpProblem problem, string file);
    }
}