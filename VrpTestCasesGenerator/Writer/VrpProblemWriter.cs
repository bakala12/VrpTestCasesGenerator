using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Writer
{
    public class VrpProblemWriter : IVrpProblemWriter
    {
        public async Task Write(VrpProblem problem, string file)
        {
            await Task.Run(() => WriteToFile(problem, file));
        }

        private void WriteToFile(VrpProblem problem, string file)
        {
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLineAsync($"NAME : {problem.Name}");
                    sw.WriteLineAsync($"COMMENT: {problem.Comment}");
                    sw.WriteLineAsync("TYPE : CVRP");
                    sw.WriteLineAsync($"DIMENSION : {problem.Dimension}");
                    sw.WriteLineAsync("EDGE_WEIGHT_TYPE : EXPLICIT");
                    sw.WriteLineAsync("EDGE_WEIGHT_FORMAT: FULL_MATRIX");
                    sw.WriteLineAsync("DISPLAY_DATA_TYPE: NO_DISPLAY");
                    sw.WriteLineAsync($"CAPACITY : {problem.Capacity}");
                    WriteMatrix(sw, problem.Distances);
                    WriteDemands(sw, problem.Demands);
                    WriteDepot(sw);
                    sw.WriteLineAsync("EOF");
                }
            }
        }

        private int FindDigits(DistanceMatrix matrix)
        {
            int max = 0;
            for(int i=0; i<matrix.Dimension; i++)
            {
                for (int j = 0; j < matrix.Dimension; j++)
                {
                    int d = (int)Math.Round(matrix[i, j]);
                    int x = 0;
                    while (d > 1)
                    {
                        x++;
                        d /= 10;
                    }
                    if (x > max)
                        max = x;
                }
            }
            return max;
        }

        private void WriteMatrix(StreamWriter sw, DistanceMatrix matrix)
        {
            sw.WriteLine("EDGE_WEIGHT_SECTION");
            int max = FindDigits(matrix);
            var maxFormat = "{0," + max + "}";
            StringBuilder builder = new StringBuilder();
            int k = 0;
            for(int i=0; i<matrix.Dimension; i++)
            {
                for(int j=0; j<matrix.Dimension; j++)
                {
                    if (k < 10)
                    {
                        builder.Append("\t");
                        builder.AppendFormat(maxFormat, matrix[i, j]);
                        k++;
                    }
                    else
                    {
                        sw.WriteLine(builder.ToString());
                        k = 0;
                        builder.Clear();
                    }
                }
            }
            if (k > 0)
            {
                sw.WriteLine(builder.ToString());
            }
        }

        private async Task WriteDemands(StreamWriter sw, int[] demands)
        {
            await sw.WriteLineAsync("DEMAND_SECTION");
            await sw.WriteLineAsync("1 0");
            for (int i = 0; i < demands.Length; i++)
            {
                await sw.WriteLineAsync($"{i + 2} {demands[i]}");
            }
        }

        private async Task WriteDepot(StreamWriter sw)
        {
            await sw.WriteLineAsync($"DEPOT_SECTION");
            await sw.WriteLineAsync("1");
            await sw.WriteLineAsync("-1");
        }
    }
}
