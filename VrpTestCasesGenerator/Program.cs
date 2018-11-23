﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using VrpTestCasesGenerator.Generator;
using VrpTestCasesGenerator.Model;
using VrpTestCasesGenerator.Writer;

namespace VrpTestCasesGenerator
{
    /// <summary>
    /// A class that describes application parameters.
    /// </summary>
    public class Arguments
    {
        [Option('p', "problemName", Required = true, HelpText = "Problem name")]
        public string ProblemName { get; set; }

        [Option('s', "streets", Required = true, HelpText = "Streets")]
        public IEnumerable<string> Streets { get; set; }

        [Option('c', "clients", Required = false, Default = 100)]
        public int Clients { get; set; }

        [Option('n', Required = false, Default = 1, HelpText = "Number of instances generated")]
        public int NumberOfInstances { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output path")]
        public string OutputPath { get; set; }

        [Option("lat", Required = false, Default = 52.254429, HelpText = "Depot latitude")]
        public double DepotLatitude { get; set; }

        [Option("lon", Required = false, Default = 20.970360, HelpText = "Depot longitude")]
        public double DepotLongitude { get; set; }

        [Option("capacity", Default = 100, Required = false, HelpText = "Vehicle capacity")]
        public int Capacity { get; set; }

        [Option("includeCoords", Default = true, Required = false, HelpText = "Output node coordinates")]
        public bool IncludeCoords { get; set; }

        [Option('a', "additionalInfo", Required = false, Default = null, HelpText = "A path for benchmark file with additional info as LOCATION_GROUPS_SECTION")]
        public string AdditionalInfoFilePath { get; set; }

        [Option("dalpha", Required = false, Default = 0.1, HelpText = "Alpha parameter for Beta distribution used for generating demands")]
        public double AlphaDemandDistribution { get; set; }

        [Option("dbeta", Required = false, Default = 0.1, HelpText = "Beta parameter for Beta distribution used for generating demands")]
        public double BetaDemandDistribution { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            Parser.Default.ParseArguments<Arguments>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleError);
#else
            RunWithDefault();
#endif
        }

        static void Run(Arguments arguments)
        {
            IGraphHopperClient graphHopperClient = new GraphHopperClient();
            INominatimClient nominatimClient = new NominatimClient();

            IVrpGenerator generator = new VrpGenerator(
                new DemandGenerator(arguments.AlphaDemandDistribution, arguments.BetaDemandDistribution, arguments.Capacity), 
                new ClientCoordsGenerator(nominatimClient, 0.001), 
                new DistanceMatrixGenerator(graphHopperClient));

            IVrpProblemWriter writer = new VrpProblemWriter();
            var param = new GeneratorParameters()
            {
                ProblemName = arguments.ProblemName,
                Clients = arguments.Clients,
                Streets = arguments.Streets,
                Comment = "Generated by a tool",
                Capacity = arguments.Capacity,
                Depot = new Location()
                {
                    Latitude = arguments.DepotLatitude,
                    Longitude = arguments.DepotLongitude
                },
                IncludeCoords = arguments.IncludeCoords,
            };
            var tasks = new Task[arguments.NumberOfInstances];
            for (int i = 0; i < arguments.NumberOfInstances; i++)
            {
                int num = i+1;
                tasks[i] = Task.Run(async () =>
                    {
                        var problem = await generator.Generate(param);
                        if (arguments.NumberOfInstances > 1)
                            problem.Name += num;
                        await WriteFiles(writer, problem, arguments, num);
                    });
            }
            Task.WaitAll(tasks);
        }

        static void RunWithDefault()
        {
            Arguments args = new Arguments()
            {
                ProblemName = "Test",
                OutputPath = "Benchmarks/Testa.vrp",
                Clients = 100,
                DepotLatitude = 52.231838,
                DepotLongitude = 21.005995,
                Capacity = 100,
                Streets = new List<string>()
                {
                    "Marszalkowska",
                    "Noakowskiego",
                    "Polna",
                    "Koszykowa",
                    "Aleje Jerozolimskie",
                    "Puławska"
                },
                NumberOfInstances = 1,
                IncludeCoords = true,
                AdditionalInfoFilePath = "Benchmarks\\TestAdd.vrp",
                AlphaDemandDistribution = 0.1,
                BetaDemandDistribution = 1
            };
            Run(args);
        }

        static async Task WriteFiles(IVrpProblemWriter writer, VrpProblem problem, Arguments args, int instanceNum=1)
        {
            var output = args.OutputPath ?? (args.ProblemName + ".vrp");
            if (args.NumberOfInstances > 1)
            {
                output = AdjustPath(output, instanceNum);
            }
            await writer.Write(problem, output);
            if (!string.IsNullOrEmpty(args.AdditionalInfoFilePath))
            {
                var path = args.AdditionalInfoFilePath;
                if (args.NumberOfInstances > 1)
                    path = AdjustPath(path, instanceNum);
                await writer.WriteWithAdditionalInfo(problem, path);
            }
        }

        static string AdjustPath(string path, int instanceNum)
        {
            var fileName = Path.GetFileNameWithoutExtension(path) + instanceNum + ".vrp";
            var dir = Path.GetDirectoryName(path);
            return Path.Combine(dir, fileName);
        }

        static void HandleError(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
