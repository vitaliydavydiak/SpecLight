﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SpecLight.Output;
using SpecLight.Output.ViewModel;

namespace SpecLight.Infrastructure
{
    internal static class SpecReporter
    {
        static readonly ConcurrentDictionary<Assembly, ConcurrentBag<Spec>> ExecutedSpecs = new ConcurrentDictionary<Assembly, ConcurrentBag<Spec>>();

        static SpecReporter()
        {
            AppDomain.CurrentDomain.DomainUnload += OnCurrentDomainUnload;
        }

        private static void OnCurrentDomainUnload(object sender, EventArgs args)
        {
            WriteSpecs();
        }

        public static void Add(Spec item)
        {
            var a = item.CallingMethod.DeclaringType.Assembly;
            var bag = ExecutedSpecs.GetOrAdd(a, assembly => new ConcurrentBag<Spec>());
            bag.Add(item);
        }

        public static IReadOnlyList<string> WriteSpecs()
        {
            AppDomain.CurrentDomain.DomainUnload -= OnCurrentDomainUnload;

            var results = new List<string>();
            foreach (var kvp in ExecutedSpecs)
            {
                var template = new SinglePageRazorTemplate
                {
                    TemplateModel = new RootViewModel(kvp.Value)
                };

                var filePath = Path.Combine(GetAssemblyDir(kvp.Key), $"{kvp.Key.GetName().Name}.Speclight.html");

                try
                {
                    Console.Out.WriteLine("Writing SpecLight report to '{0}'", filePath);
                    File.WriteAllText(filePath, template.TransformText());
                    results.Add(filePath);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to write SpecLight report to '{0}'", filePath);
                    Console.Error.WriteLine(e);
                    results.Add(e.ToString());
                }
            }

            return results;
        }

        static string GetAssemblyDir(Assembly a)
        {
            if (!string.IsNullOrWhiteSpace(a.CodeBase))
            {
                var uri = new Uri(a.CodeBase);
                if (uri.Scheme == "file")
                {
                    var localPath = uri.LocalPath;
                    return Path.GetDirectoryName(localPath);
                }
            }
            return Path.GetDirectoryName(a.Location);
        }
    }
}
