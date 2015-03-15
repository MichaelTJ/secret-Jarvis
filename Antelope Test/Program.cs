using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using Calais;
using IKVM;
using ikvm;
using jena;
using Jena;
using Proxem.NanoProlog;
using Proxem.ShortestPath;
using Proxem.Antelope.Coreferences;
using Proxem.Antelope.Disambiguation;
using Proxem.Antelope.Features;
using Proxem.Antelope.Formatters;
using Proxem.Antelope.Lexicon;
using Proxem.Antelope.LinkGrammar;
using Proxem.Antelope.Paraphrases;
using Proxem.Antelope.Parsing;
using Proxem.Antelope.Predicates;
using Proxem.Antelope.Semantics;
using Proxem.Antelope.Stanford;
using Proxem.Antelope.Tagging;
using Proxem.Antelope.Tagmatica;
using Proxem.Antelope.Tools;
using StanfordParser;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using CustomExtension;
using System.Threading;
using System.Diagnostics;

namespace CustomExtension
{
    public static class assemblyExtension
    {

        public static FileInfo GetXmlDocFile(Assembly assembly)
        {
            string assemblyDirPath = Path.GetDirectoryName(assembly.Location);
            string fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".xml";
            
            return GetFallbackDirectories(CultureInfo.CurrentCulture)
              .Select(dirName => CombinePath(assemblyDirPath, dirName, fileName))
              .Select(filePath => new FileInfo(filePath))
              .Where(file => file.Exists)
              .First();
        }

        public static IEnumerable<string> GetFallbackDirectories(CultureInfo culture)
        {
            return culture
              .Enumerate(c => c.Parent.Name != c.Name ? c.Parent : null)
              .Select(c => c.Name);
        }

        public static IEnumerable<T> Enumerate<T>(this T start, Func<T, T> next)
        {
            for (T item = start; !object.Equals(item, default(T)); item = next(item))
                yield return item;
        }

        public static string CombinePath(params string[] args)
        {
            Console.WriteLine(args.Aggregate(Path.Combine));
            return args.Aggregate(Path.Combine);
        }
    }
}
namespace Antelope_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Program P = new Program();
            P.Onstart();
            
        }
        

        public void Onstart()
        {

            //LEARNING CSHARP
            Console.WriteLine("Loading cs2");
            System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            CSharp2 cs2 = new CSharp2();
            //CSharp cs = new CSharp();
            stopWatch.Stop();
            Console.WriteLine("Done");
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("time: ");
            Console.WriteLine(ts.Minutes);
            Console.WriteLine(ts.Seconds);

            //NLP 
            //Get an input
            //Console.WriteLine("input: ");
            //string input = Console.ReadLine();
            string input = "Create a List.";
            Console.WriteLine("Using '{0}' as an input", input);


            stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("Finding matching types");
            //cs2.compareAll(input);
            Dictionary<string, double> result = cs2.compareAll(input);

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            Console.WriteLine("Done. Time: {0}min {1}sec",ts.Minutes,ts.Seconds);

            Console.WriteLine(FindMax(result));
            Console.Read();


        }

        private string FindMax(Dictionary<string, double> diction)
        {
            string result = "";
            double totalcount = 0;
            
            foreach (string key in diction.Keys)
            {
                if (diction[key] >= totalcount)
                {
                    Console.WriteLine("Key: {0}, Score: {1}.", key, diction[key]);
                    totalcount = diction[key];
                    result = "Key: " + key + "Score: " + diction[key];
                }
            }
            Console.WriteLine("Total in dictionary: {0}", diction.Count);
            return result;
        }


    }
}
