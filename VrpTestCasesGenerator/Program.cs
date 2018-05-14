﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using VrpTestCasesGenerator.Generator;
using VrpTestCasesGenerator.Model;
using VrpTestCasesGenerator.Writer;

namespace VrpTestCasesGenerator
{
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

        //By default Pałac kultury i Nauki
        [Option("lat", Required = false, Default = 52.231838, HelpText = "Depot latitude")]
        public double DepotLatitude { get; set; }

        //By default Pałac kultury i Nauki
        [Option("lon", Required = false, Default = 21.005995, HelpText = "Depot longitude")]
        public double DepotLongitude { get; set; }

        [Option("capacity", Default = 100, Required = false, HelpText = "Vehicle capacity")]
        public int Capacity { get; set; }
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
            IVrpGenerator generator = new VrpGenerator(new DemandGenerator(0.1,1,arguments.Capacity), new DistanceMatrixGenerator(new StreetPointGenerator()));
            IVrpProblemWriter writer = new VrpProblemWriter();
            var output = arguments.OutputPath ?? arguments.ProblemName;
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
                }
            };
            var tasks = new Task[arguments.NumberOfInstances];
            for (int i = 0; i < arguments.NumberOfInstances; i++)
            {
                tasks[i] = Task.Run(async () =>
                    {
                        var problem = await generator.Generate(param);
                        var outputPath = arguments.NumberOfInstances == 1 ? output + ".vrp" : output + $"{i + 1}.vrp";
                        await writer.Write(problem, outputPath);
                    });
            }
            Task.WaitAll(tasks);
        }

        static void RunWithDefault()
        {
            Arguments args = new Arguments()
            {
                ProblemName = "Test",
                Clients = 5,
                DepotLatitude = 52.231838,
                DepotLongitude = 21.005995,
                Capacity = 100,
                Streets = new List<string>()
                {
                    "Noakowskiego"                    
                },
                NumberOfInstances = 1
            };
            Run(args);
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
