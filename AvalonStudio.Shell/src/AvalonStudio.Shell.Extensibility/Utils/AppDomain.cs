﻿using AvalonStudio.Commands;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AvalonStudio.Extensibility.Utils
{
    public class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();

            var compileDependencies = DependencyContext.Default.RuntimeLibraries;

            foreach (var library in compileDependencies)
            {
                if (IsCandidateCompilationLibrary(library))
                {
                    try
                    {
                        var assembly = Assembly.Load(new AssemblyName(library.Name));
                        assemblies.Add(assembly);
                    }
                    catch(Exception)
                    {

                    }
                }
            }

            assemblies.Add(Assembly.GetAssembly(typeof(CommandService)));

            return assemblies.Distinct().ToArray();
        }

        private static bool IsCandidateCompilationLibrary(Library compilationLibrary)
        {
            return compilationLibrary.Name.ToLower() == "avalonStudio"
                || compilationLibrary.Name.ToLower().StartsWith("avalonstudio")
                || compilationLibrary.Dependencies.Any(d => d.Name.ToLower().StartsWith("avalonstudio"));
        }
    }
}