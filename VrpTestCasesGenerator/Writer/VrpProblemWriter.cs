using System;
using System.Collections.Generic;
using System.Globalization;
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
                    if (problem.Coordinates == null)
                    {
                        sw.WriteLineAsync("DISPLAY_DATA_TYPE: NO_DISPLAY");
                    }
                    else
                    {
                        sw.WriteLineAsync("DISPLAY_DATA_TYPE: TWOD_DISPLAY");
                        WriteCoordinates(sw, problem.Coordinates);
                    }
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
            for(int i = 0; i < matrix.Dimension; i++)
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
            for (int i = 0; i < matrix.Dimension; i++)
            {
                for (int j = 0; j < matrix.Dimension; j++)
                {
                    builder.Append("\t");
                    builder.AppendFormat(maxFormat, (int) Math.Round(matrix[i, j]));
                }
                sw.WriteLine(builder.ToString());
                builder.Clear();
            }
        }

        private void WriteDemands(StreamWriter sw, int[] demands)
        {
            sw.WriteLine("DEMAND_SECTION");
            sw.WriteLine("1 0");
            for (int i = 0; i < demands.Length; i++)
            {
                sw.WriteLine($"{i + 2} {demands[i]}");
            }
        }

        private void WriteCoordinates(StreamWriter sw, Location[] coords)
        {
            sw.WriteLine("NODE_COORD_SECTION");
            for (int i = 0; i < coords.Length; i++)
            {
                sw.WriteLine($"{i + 1} " +
                             $"{coords[i].Latitude.ToString(CultureInfo.InvariantCulture)} " +
                             $"{coords[i].Longitude.ToString(CultureInfo.InvariantCulture)}");
            }
        }

        private void WriteDepot(StreamWriter sw)
        {
            sw.WriteLine($"DEPOT_SECTION");
            sw.WriteLine("1");
            sw.WriteLine("-1");
        }
    }
}
