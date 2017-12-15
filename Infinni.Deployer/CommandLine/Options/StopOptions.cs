using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace Infinni.Deployer.CommandLine.Options
{
    [Verb("stop", HelpText = "Stops application.")]
    public class StopOptions : ICommandOptions
    {
        [Value(0)]
        public IEnumerable<string> PackageFullNames { get; set; }

        public Lazy<string[]> PackageFullNamesArray => new Lazy<string[]>(PackageFullNames.ToArray());
    }
}