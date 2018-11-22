using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Collections;
using VrpTestCasesGenerator.Model;

namespace VrpTestCasesGenerator.Writer
{
    /// <summary>
    /// An implementation of the IVrpProblemWriter interface.
    /// </summary>
    public class VrpProblemWriter : IVrpProblemWriter
    {
        /// <summary>
        /// Saves VRP problem instance to a file.
        /// </summary>
        /// <param name="problem">VRP problem instance.</param>
        /// <param name="file">Path to file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Write(VrpProblem problem, string file)
        {
            await Task.Run(() => WriteToFile(problem, file));
        }

        /// <summary>
        /// Saves VRP problem instance to a file (it also saves some additional information like locating nodes around the streets).
        /// </summary>
        /// <param name="problem">VRP problem instance.</param>
        /// <param name="file">Path to file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task WriteWithAdditionalInfo(VrpProblem problem, string file)
        {
            await Task.Run(() => WriteToFileWithAdditionalInfo(problem, file));
        }

        private void WriteToFileWithAdditionalInfo(VrpProblem problem, string file)
        {
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"NAME : {problem.Name}");
                    sw.WriteLine($"COMMENT: {problem.Comment}");
                    sw.WriteLine("TYPE : CVRP");
                    sw.WriteLine($"DIMENSION : {problem.Dimension}");
                    sw.WriteLine("EDGE_WEIGHT_TYPE : ADJ"); //revisit
                    sw.WriteLine("EDGE_WEIGHT_FORMAT: FULL_MATRIX"); //revisit
                    if (problem.Coordinates == null)
                    {
                        sw.WriteLine("DISPLAY_DATA_TYPE: NO_DISPLAY");
                    }
                    else
                    {
                        sw.WriteLine("DISPLAY_DATA_TYPE: TWOD_DISPLAY");
                        WriteCoordinates(sw, problem.Coordinates);
                    }
                    sw.WriteLine($"CAPACITY : {problem.Capacity}");
                    WriteMatrixAdj(sw, problem.Distances);
                    WriteDemands(sw, problem.Demands);
                    WriteLocationGroups(sw, problem.LocationGroups);
                    WriteDepot(sw);
                    sw.WriteLine("EOF");
                }
            }
        }

        private void WriteToFile(VrpProblem problem, string file)
        {
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"NAME : {problem.Name}");
                    sw.WriteLine($"COMMENT: {problem.Comment}");
                    sw.WriteLine("TYPE : CVRP");
                    sw.WriteLine($"DIMENSION : {problem.Dimension}");
                    sw.WriteLine("EDGE_WEIGHT_TYPE : EXPLICIT");
                    sw.WriteLine("EDGE_WEIGHT_FORMAT: FULL_MATRIX");
                    if (problem.Coordinates == null)
                    {
                        sw.WriteLine("DISPLAY_DATA_TYPE: NO_DISPLAY");
                    }
                    else
                    {
                        sw.WriteLine("DISPLAY_DATA_TYPE: TWOD_DISPLAY");
                        WriteCoordinates(sw, problem.Coordinates);
                    }
                    sw.WriteLine($"CAPACITY : {problem.Capacity}");
                    WriteMatrix(sw, problem.Distances);
                    WriteDemands(sw, problem.Demands);
                    WriteDepot(sw);
                    sw.WriteLine("EOF");
                }
            }
        }

        private int FindDigits(DistanceMatrix matrix)
        {
            int max = 0;
            for (int i = 0; i < matrix.Dimension; i++)
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
                    builder.AppendFormat(maxFormat, (int)Math.Round(matrix[i, j]));
                }
                sw.WriteLine(builder.ToString());
                builder.Clear();
            }
        }

        private void WriteMatrixAdj(StreamWriter sw, DistanceMatrix matrix)
        {
            sw.WriteLine("EDGE_WEIGHT_SECTION");
            for (int i = 0; i < matrix.Dimension; i++)
            {
                for (int j = 0; j < matrix.Dimension; j++)
                {
                    var d = (int) Math.Round(matrix[i, j]);
                    var c = matrix.GetCrossingCount(i, j);
                    if(d > 0) sw.WriteLine($"{i} {j} {d} {c}");
                }
            }
            sw.WriteLine("-1");
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

        private void WriteLocationGroups(StreamWriter sw, IDictionary<int, LocationGroup> locationGroups)
        {
            sw.WriteLine("LOCATION_GROUP_SECTION");
            foreach (var locationGroup in locationGroups)
            {
                sw.WriteLine($"{locationGroup.Key} {locationGroup.Value.Id}");
            }
            sw.WriteLine("-1");
        }
    }
}
