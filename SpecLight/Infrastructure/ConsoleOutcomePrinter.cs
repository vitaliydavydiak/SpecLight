﻿using System;
using System.Linq;

namespace SpecLight.Infrastructure
{
    static class ConsoleOutcomePrinter
    {
        const string Empty = " (Empty)";
        const string ExceptionCaught = " (Exception Caught)";
        public static readonly int MaxStepOutcomeNameLength = Enum.GetNames(typeof(Status)).Max(x => x.Length) + Empty.Length;

        public static void PrintOutcomes(Spec spec, Action<string> writeLine)
        {
            writeLine("> SpecLight results:");
            Console.WriteLine();
            if (spec.SpecTags.Any())
            {
                writeLine(String.Join(", ", spec.SpecTags.Select(x => "@" + x)));
            }
            writeLine(spec.Description);
            Console.WriteLine();

            var specData = spec.DataDictionary.FormatExtraData();
            if (!string.IsNullOrWhiteSpace(specData))
            {
                writeLine(specData);
                Console.WriteLine();
            }

            if (!spec.Outcomes.Any())
            {
                return;
            }

            var maxMessageWidth = spec.Outcomes.Max(x => x.Step.Description.Length + x.Step.FormattedType.Length) + 3;
            foreach (var o in spec.Outcomes)
            {
                var step = o.Step;
                var message = $"{step.FormattedType} {step.Description}:";
                var s = o.Status.ToString();
                if (o.Empty)
                {
                    s += Empty;
                }
                if (o.ExceptionCaught)
                {
                    s += ExceptionCaught;
                }
                s += $"\t({o.ExecutionTime.ToString("s\\.fff")}) sec.";
                var cells = new[]
                {
                    message.PadRight(maxMessageWidth),
                    s.PadRight(MaxStepOutcomeNameLength + 1),
                    string.Join(", ", step.Tags.Select(x => "@" + x)),
                    step.DataDictionary.FormatExtraData()
                };
                writeLine(string.Join("\t", cells.Where(x => x != null)));
            }
        }
    }
}
