using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Writer
{
    public interface IVrpProblemWriter
    {
        Task Write(VrpProblem problem, string file);
    }
}