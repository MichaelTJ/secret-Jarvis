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
using CustomExtension;


namespace Antelope_Test
{

    class CSharp
    {
        DocsByReflection docsBR;
        //All created objects by name
        Dictionary<string, object> objects;

        //Assembly names by name
        Dictionary<string, AssemblyName> dllNames;
        //Types associated with assembly
        Dictionary<string, Type[]> types;
        //Summaries associeated with Type
        Dictionary<Type, string> summaries;

        //Assembly -> type -> 
        Dictionary<Assembly, Dictionary<csThings, Dictionary<string, string>>> NutCShell;

        public CSharp()
        {
            docsBR = new DocsByReflection();
            dllNames = new Dictionary<string, AssemblyName>();
            objects = new Dictionary<string, object>();
            types = new Dictionary<string, Type[]>();
            summaries = new Dictionary<Type, string>();

            Dictionary<csThings, Dictionary<string, string>> csByType = new Dictionary<csThings, Dictionary<string, string>>();
            csByType.Add(csThings.Type, new Dictionary<string, string>());

            findAssemblies(@"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");

        }

        private void findAssemblies(string filePath)
        {
            Console.WriteLine("Finding Assemblies...");
            foreach (string str in Directory.GetFiles(filePath, "*dll", SearchOption.AllDirectories))
            {
                    try
                    {
                        AssemblyName asmnm = AssemblyName.GetAssemblyName(str);
                        
                        if (asmnm.Name.StartsWith("System"))
                        {
                            Console.WriteLine("Assembly name: " + asmnm.Name);
                            dllNames.Add(asmnm.Name, asmnm);
                            Assembly asm = Assembly.Load(asmnm);
                            Type[] types = asm.GetTypes();
                            //Dictionary Assembly/Dict
                            try
                            {
                                XmlDocument doc = new XmlDocument();
                                Console.WriteLine("filePath: " + str);
                                string fileName = Path.ChangeExtension(str, ".xml");
                                Console.WriteLine("fileName " + fileName);
                                doc.Load(fileName);


                                //Dictionary<Assembly, Dictionary<csThings, Dictionary<string, string>>> NutCShell;
                                
                                foreach (XmlElement elm  in doc["doc"]["members"])
                                {

                                    string alphaMode = elm.Attributes["name"].Value.Substring(2, elm.Attributes["name"].Value.Length - 2);
                                    Console.WriteLine("element: " + alphaMode);
                                    foreach (Type t in types)
                                    {
                                        if (t.FullName == alphaMode)
                                        {
                                            Console.WriteLine(t.FullName);
                                            //Console.WriteLine("success");
                                            break;
                                        }
                                        else
                                        {
                                            //Console.WriteLine("failure");
                                        }
                                    }
                                    Console.WriteLine(elm["summary"].InnerText);
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
                                Console.WriteLine(e);
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


        private XmlElement XMLFromName(XmlDocument doc, Type type, char prefix, string name)
        {
            string fullName;

            if (String.IsNullOrEmpty(name))
            {
                fullName = prefix + ":" + type.FullName;
            }
            else
            {
                fullName = prefix + ":" + type.FullName + "." + name;
            }


            XmlElement matchedElement = null;

            foreach (XmlElement xmlElement in doc["doc"]["members"])
            {
                if (xmlElement.Attributes["name"].Value.Equals(fullName))
                {
                    if (matchedElement != null)
                    {
                        throw new DocsByReflectionException("Multiple matches to query", null);
                    }

                    matchedElement = xmlElement;
                }
            }

            if (matchedElement == null)
            {
                throw new DocsByReflectionException("Could not find documentation for specified element", null);
            }

            return matchedElement;
        }

        public void loadAssembly(string strasmnm)
        {
            try
            {
                loadAssembly(dllNames[strasmnm]);
            }
            catch { }
        }
        public void loadAssembly(AssemblyName asmnm)
        {
            Console.WriteLine("Loading assembly {0}...", asmnm.Name);
            try
            {
                Assembly asm = Assembly.Load(asmnm);

                types.Add(asmnm.Name, asm.GetTypes());
                /*
                foreach (Type t in types[asmnm.Name])
                {
                    if (t.Namespace == asmnm.Name && t.IsClass)
                    {
                        Console.WriteLine(t.Name);
                    }
                }
                 */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Done.");
        }
        public Assembly GetAssembly(string strasmnm)
        {
            try
            {
                return GetAssembly(dllNames[strasmnm]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public Assembly GetAssembly(AssemblyName asmnm)
        {
            Console.WriteLine("Loading assembly {0}...", asmnm.Name);
            try
            {
                Assembly asm = Assembly.Load(asmnm);

                return asm;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public List<Type> getTypes(Assembly assembly)
        {
            List<Type> retList = new List<Type>();
            Console.WriteLine("Get type");
            try
            {
                foreach (Type t in assembly.GetTypes())
                {
                    retList.Add(t);
                }
            }
            catch
            { }

            return retList;

        }
        public List<Type> getTypes(AssemblyName assemblyName)
        {
            List<Type> retList = new List<Type>();
            Assembly asm = Assembly.Load(assemblyName);
            Console.WriteLine("Get type");
            try
            {
                foreach (Type t in asm.GetTypes())
                {
                    retList.Add(t);
                }
            }
            catch
            { }

            return retList;
        }
        //create an instance of "type input" called 'input'
        public void createObject(string name, Type t)
        {
            Console.WriteLine("Creating object");
            ConstructorInfo cto = t.GetConstructor(Type.EmptyTypes);
            objects.Add(name, cto.Invoke(null));
        }


        public MethodInfo[] getMethods(string name)
        {
            Type t = objects[name].GetType();
            return t.GetMethods();
        }
        public MethodInfo[] getMethods(Type t)
        {
            Console.WriteLine("Getting methods");
            return t.GetMethods();
        }
        public List<string> getAssemblies()
        {
            Console.WriteLine("Getting Assemblies...");
            List<string> names = new List<string>();
            foreach (string name in dllNames.Keys)
            {
                names.Add(name);
            }
            return names;
        }
        public List<string> getLoadedAssemblies()
        {
            Console.WriteLine("Getting Loaded Assemblies");
            List<string> loaded = new List<string>();
            foreach (string name in types.Keys)
            {
                loaded.Add(name);
            }
            return loaded;
        }
        public Dictionary<Type, string> getSummaries()
        {
            return this.summaries;
        }
    }
}
