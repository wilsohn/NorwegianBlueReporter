﻿using CommandLine;
using CommandLine.Text;

namespace NorwegianBlueReporter
{
    public class CommandLineOptions
    {
        [Option('i', "input", Required = true, HelpText = "Full path to the input file")]
        public string InputFileName { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to output the document that is generated")]
        public string OutputFileName { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}