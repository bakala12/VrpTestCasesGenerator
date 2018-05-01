using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

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

        //By default Pałac kultury i Nauki
        [Option("lat", Required = false, Default = 52.231838, HelpText = "Depot latitude")]
        public double DepotLatitude { get; set; }

        //By default Pałac kultury i Nauki
        [Option("lon", Required = false, Default = 21.005995, HelpText = "Depot longitude")]
        public double DepotLongitude { get; set; }
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
            
        }

        static void RunWithDefault()
        {
            Arguments args = new Arguments()
            {
                ProblemName = "Test",
                Clients = 5,
                DepotLatitude = 52.231838,
                DepotLongitude = 21.005995,
                Streets = new List<string>()
                {
                    "Warszawa,Noakowskiego"                    
                }
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
