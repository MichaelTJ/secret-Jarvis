using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Configuration.Internal;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.ProviderBase;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Deployment;
using System.Deployment.Internal;
using System.Deployment.Internal.Isolation;
using System.Deployment.Internal.Isolation.Manifest;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Contracts.Internal;
using System.Diagnostics.Eventing;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.PerformanceData;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.IsolatedStorage;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Management.Instrumentation;
using System.Media;
using System.Net;
using System.Net.Cache;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Serialization.Advanced;
using System.Xml.Serialization.Configuration;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl;
using Proxem.Antelope.Tagging;
using Proxem.Antelope.Lexicon;

namespace Antelope_Test
{
    public enum csThings
    {
        Type,
        Method,
        Parameter,
        Constructor
    }
    class CSharp2
    {

        //Assembly -> type -> string(name)/string description
        Dictionary<Assembly, Dictionary<csThings, Dictionary<string, IList<IWord>>>> NutCShell;
        Antelope ant = new Antelope();

        public CSharp2()
        {
            //creates nutcshell
            NutCShell = new Dictionary<Assembly, Dictionary<csThings, Dictionary<string, IList<IWord>>>>();
            findAssemblies(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
            //findAssemblies(@"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
        }


        private void findAssemblies(string filePath)
        {
            
            Console.WriteLine("Finding Assemblies...");
            //Get all the files that end in .dll in the filepath
            foreach (string str in Directory.GetFiles(filePath, "*dll", SearchOption.AllDirectories))
            {
                try
                {
                    
                    //Get the assembly name (required to get the assembly)
                    AssemblyName asmnm = AssemblyName.GetAssemblyName(str);
                    //If the assembly name starts with System
                    if (asmnm.Name.StartsWith("System"))
                    {
                        //Console.WriteLine("Assembly name: " + asmnm.Name);
                        //get the assembly
                        Assembly asm = Assembly.Load(asmnm);
                        //get all types in the assembly
                        Type[] types = asm.GetTypes();
                        
                        //create part 2 of nutCShell (the dictionary containing all methods
                        NutCShell.Add(asm, new Dictionary<csThings,Dictionary<string, IList<IWord>>>());
                        NutCShell[asm].Add(csThings.Constructor, new Dictionary<string, IList<IWord>>());
                        NutCShell[asm].Add(csThings.Method, new Dictionary<string, IList<IWord>>());
                        NutCShell[asm].Add(csThings.Parameter, new Dictionary<string, IList<IWord>>());
                        NutCShell[asm].Add(csThings.Type, new Dictionary<string, IList<IWord>>());

                        try
                        {
                            //Get the related xml file as 'doc'
                            XmlDocument doc = new XmlDocument();
                            //Console.WriteLine("filePath: " + str);
                            string fileName = Path.ChangeExtension(str, ".xml");
                            //Console.WriteLine("fileName " + fileName);
                            doc.Load(fileName);
                            
                            //foreach element in the doc
                            foreach (XmlElement elm in doc["doc"]["members"])
                            {
                                string csele = elm.Attributes["name"].Value.Substring(0, 2);
                                //Console.WriteLine(csele);

                                if (csele == "T:")
                                {
                                    //get a simplified string for element
                                    string eleString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + eleString);

                                    //foreach type in the assembly check if the xml doc element is the same as the element
                                    foreach (Type t in types)
                                    {
                                        if (t.FullName == eleString)
                                        {
                                            /*
                                            Console.WriteLine("Name: " + t.FullName);
                                            Console.WriteLine("Class: " + t.IsClass);
                                            Console.WriteLine("Enum: " + t.IsEnum);
                                            Console.WriteLine("Generic Type: " + t.IsGenericType);
                                            Console.WriteLine("Interface: " + t.IsInterface);
                                            Console.WriteLine("ValueType: " + t.IsValueType);
                                            Console.WriteLine("TypeHandle" + t.TypeHandle);
                                            */

                                            //Antelope.Tags the sentence and adds to NutCshell
                                            NutCShell[asm][csThings.Type].Add(t.FullName, ant.ITag(elm["summary"].InnerText));

                                            /*
                                            //exploring whats inside the tag
                                            foreach (IWord lemming in NutCShell[asm][csThings.Type][t.FullName])
                                            {
                                                try
                                                {
                                                    Console.WriteLine(lemming.Text);
                                                    IList<ILemma> assemblyWordSenses = ant.lexicon.FindSenses(lemming.Text, lemming.PartOfSpeech);
                                                    foreach (ILemma lem in assemblyWordSenses)
                                                    {
                                                        Console.WriteLine(lem.Text);
                                                        Console.WriteLine(lem.Synset);
                                                        Console.WriteLine(lem.Synset.Definition);
                                                    }
                                                }
                                                catch { }
                                            
                                            }
                                             * */
                                            break;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("failure");
                                        }
                                    }
                                }
                                
                                else if(csele == "M:")
                                {
                                    //simple string for method
                                    string methodString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + methodString);
                                }
                                else if(csele == "P:")
                                {
                                    //simple string for param
                                    string paramString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + paramString);
                                }
                                else if (csele == "F:")
                                {
                                    //simple string for enum
                                    string enumString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + enumString);
                                }
                                else if (csele == "E:")
                                {
                                    //simple string for enum
                                    string eventString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + eventString);
                                }
                                else
                                {
                                    //simple string for method
                                    string otherString = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    //Console.WriteLine("element: " + otherString);
                                }
                                //Console.WriteLine(elm["summary"].InnerText);
                            }

                            /*
                            foreach (Type t in asm.GetTypes())
                            {
                                try
                                {
                                    XmlElement elma = XMLFromName(doc, t, 'T', "");
                                    XmlTextWriter writer = new XmlTextWriter(Console.Out);
                                    writer.Formatting = Formatting.Indented;
                                    Console.WriteLine("Type: " + t.Name);
                                    elma.WriteContentTo(writer);
                                    Console.WriteLine();
                                }
                                catch
                                { }
                            }*/
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                        }

                        /*
                        Console.WriteLine(asmnm.Name);
                        Console.WriteLine();
                         * */
                    }

                }
                catch { }
            }
        }

        private void writeAll()
        {
            foreach (Assembly aaaa in NutCShell.Keys)
            {
                Console.WriteLine("{0,10}", aaaa.FullName);
                foreach (csThings cs in (csThings[])Enum.GetValues(typeof(csThings)))
                {
                    Console.WriteLine("{0,20}", cs.ToString());
                    foreach (string str in NutCShell[aaaa][cs].Keys)
                    {
                        Console.WriteLine("{0,20}", str);
                        Console.WriteLine("{0,30}", NutCShell[aaaa][cs][str]);
                    }
                }
            }
        }

        private IList<IWord> tagString(string input)
        {
            return ant.ITag(input);
        }

        
        public Dictionary<string, int> compareAssembly(string input)
        {
            //Need to keep a record of each assembly->csthing->Type and their scores
            Dictionary<string, int> score = new Dictionary<string,int>();

            //tag the incoming senstence
            IList<IWord> taggedSentance = ant.ITag(input);
            foreach (IWord word in taggedSentance)
            {
                Console.WriteLine("_____" + word.Text);
                //foreach assembly
                foreach (Assembly aaaa in NutCShell.Keys)
                {
                    Console.WriteLine(aaaa.GetName().Name);
                    //split the name up by .
                    foreach (string nameSec in aaaa.GetName().Name.Split('.'))
                    {
                        try
                        {
                            IList<ILemma> assemblyWordSenses = ant.lexicon.FindSenses(nameSec, word.PartOfSpeech);
                            foreach (ILemma lem in assemblyWordSenses)
                            {
                                Console.WriteLine(lem.Text);
                            }
                            Console.WriteLine(assemblyWordSenses[0].SynsetId);
                            Console.WriteLine(word.Senses[0].Lemma.SynsetId);
                            if (assemblyWordSenses[0].SynsetId == word.Senses[0].Lemma.SynsetId)
                            {
                                Console.WriteLine(nameSec + " matches " + word.Text);
                            }
                        }
                        catch { }
                    }


                }
            }

            return score;
            
        }

        public Dictionary<string, double> compareAll(string input)
        {
            //Need to keep a record of each assembly->csthing->Type and their scores
            
            //Dictionary<Assembly, Dictionary<csThings, Dictionary<string, int>>> score = new Dictionary<Assembly,Dictionary<csThings,Dictionary<string,int>>>();
            Dictionary<string, double> score = new Dictionary<string, double>();

            //tag the incoming senstence
            IList<IWord> taggedSentance = ant.ITag(input);

            //for each word in the sentence
            foreach (IWord word in taggedSentance)
            {
                //if the word is a noun or verb
                if (word.PartOfSpeech == Proxem.Antelope.PartOfSpeech.Noun | word.PartOfSpeech == Proxem.Antelope.PartOfSpeech.Verb)
                {
                    //Get the different senses for that particular word
                    IList<ILemma> synset = ant.lexicon.FindSenses(word.Text, word.PartOfSpeech);
                    Console.WriteLine(word.PartOfSpeech);
                    Console.WriteLine(synset[0].Lemma.Synset.Definition);

                    foreach (RelationType rel in (RelationType[])Enum.GetValues(typeof(RelationType)))
                    {
                        Console.WriteLine(rel.ToString());
                        foreach (ILemma str in synset[0].RelatedLemmas(rel))
                        {
                            Console.WriteLine(str.Text);
                        }
                    }

                    //Compare it to all the assemblies
                    foreach (Assembly aaaa in NutCShell.Keys)
                    {
                        Console.WriteLine("Looking in: " + aaaa.GetName().Name);
                        foreach (csThings cs in (csThings[])Enum.GetValues(typeof(csThings)))
                        {
                            foreach (string str in NutCShell[aaaa][cs].Keys)
                            {
                                //Get each word
                                foreach (IWord wordbit in NutCShell[aaaa][cs][str])
                                {
                                    if (wordbit.PartOfSpeech == word.PartOfSpeech)
                                    {
                                        try
                                        {
                                            //very basic heuristic
                                            //If the word synset equals the input synset
                                            /*
                                            Console.WriteLine(word.Text + ":");
                                            Console.WriteLine("Synset: " + synset[0].Synset);
                                            Console.WriteLine("Synset def: " + synset[0].Synset.Definition);
                                            Console.WriteLine("synset ID: " +synset[0].SynsetId);
                                            Console.WriteLine("Frequency: " +synset[0].Frequency);
                                            Console.WriteLine("Lemma: " + synset[0].Lemma.Text);

                                            Console.WriteLine();
                                            
                                            Console.WriteLine("word in def: " + wordbit.Text);
                                            Console.WriteLine(wordbit.BaseForms[0]);*/
                                            IList<ILemma> wordBitSynset = ant.lexicon.FindSenses(wordbit.Text, wordbit.PartOfSpeech);
                                            /*
                                            Console.WriteLine("Synset: " + wordBitSynset[0].Synset);
                                            Console.WriteLine("synset ID: " + wordBitSynset[0].SynsetId);
                                            Console.WriteLine("Frequency: " + wordBitSynset[0].Frequency);
                                            Console.WriteLine("Lemma: " + wordBitSynset[0].Lemma.Text);

                                            Console.WriteLine();
                                            */
                                            if (synset[0].SynsetId == wordBitSynset[0].SynsetId)
                                            {
                                                tryAddDict(score, str, 1.0); //Added after "1.0": /NutCShell[aaaa][cs][str].Count
                                            }
                                        }
                                        catch
                                        {
                                            //Console.WriteLine("Error");
                                            //Console.WriteLine();
                                            //Console.WriteLine(e);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return score;

        }

        private void tryAddDict(Dictionary<string, double> stint, string st, double tint)
        {
            if (stint.Keys.Contains(st))
            {
                stint[st] += tint;
            }
            else
            {
                stint.Add(st, tint);
            }
        }
    }
}
