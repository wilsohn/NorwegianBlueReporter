﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using NorwegianBlue.Analysis.Algorithms;
using NorwegianBlue.IagoIntegration.Analysis;
using NorwegianBlue.IagoIntegration.Samples;
using NorwegianBlue.Integration.Azure.Analysis;
using NorwegianBlue.Integration.Azure.Samples;
using NorwegianBlue.Integration.IIS.Analysis;
using NorwegianBlue.Integration.IIS.Samples;
using NorwegianBlue.Samples;
using NorwegianBlue.Util.Pdf;

namespace NorwegianBlueReporter
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic options = new AppOptions(args);

            // set up sample sets
            var azureSamples = new AzureMetricsSampleSet();
            var azureStartTime = new DateTime(2014, 4, 8, 16, 0, 0, DateTimeKind.Local);
            var azureEndTime = new DateTime(2014, 4, 8, 18, 0, 0, DateTimeKind.Local);
            dynamic azureDataSourceSetup = new ExpandoObject();
            azureSamples.Parse(TimeZone.CurrentTimeZone, azureStartTime, azureEndTime, azureDataSourceSetup);

            var iisSamples = new IisSampleSet();
            var iisStartTime = new DateTime(2014, 4, 12, 18, 10, 0);
            var iisEndTime = new DateTime(2014, 4, 12, 20, 11, 0);
            dynamic iisDataSourceSetup = new ExpandoObject();
            iisSamples.Parse(TimeZone.CurrentTimeZone, iisStartTime, iisEndTime, iisDataSourceSetup);
                 
            var iagoSamples = new IagoSampleSet();
            dynamic iagoDataSourceSetup = new ExpandoObject();
            iagoDataSourceSetup.DataSourceFileName = @"c:\tmp\parrot-server-stats.log";
            iagoSamples.Parse(TimeZone.CurrentTimeZone, null, null, iagoDataSourceSetup);

            // TODO: Populate these by reflection
            // set up analysis algorithms
            var commonSetAnalyzers = new CommonSampleSetAnalysis();
            var commonStatAnalyzers = new CommonStatAnalysis();
            var iagoSetAnalyzers = new IagoSampleSetAnalysis();
            var iisSetAnalyzers = new IisSampleSetAnalysis();
            var azureMetricsSetAnalyzers = new AzureMetricsSampleSetAnalysis();


            var commonSetAnalysisMethods = new List<SetAnalyzer<ISampleSetAnalysis<ISampleAnalysis>, ISampleAnalysis>>
                {
                    commonSetAnalyzers.FindAllHeaders,
                    commonSetAnalyzers.SummaryStats,
                    commonSetAnalyzers.ClusterAnalysis
                };

            var statAnalysisMethods = new List<SampleInSetAnalyzer<ISampleSetAnalysis<ISampleAnalysis>, ISampleAnalysis>>();

            // set up type specific analysis
            var iagoSetAnalysisMethods = new List<SetAnalyzer<IagoSampleSet, IagoSample>>();
            iagoSetAnalysisMethods.AddRange(commonSetAnalysisMethods);
//            iagoSetAnalysisMethods.Add(
//                    iagoSetAnalyzers.IagoSummaryGraphs
//                );

            // individual stat analysis
            // statAnalysisMethods.Add(statAnalyzers.SummaryStatComparison);
            statAnalysisMethods.Add(commonStatAnalyzers.SummaryStatComparisonAsTables);

            iagoSamples.Analyze(iagoSetAnalysisMethods, statAnalysisMethods);
            iagoSetAnalyzers.IagoSummaryGraphs(iagoSamples);

            var iisSetAnalysisMethods = new List<SetAnalyzer<IisSampleSet, IisSample>>();
            iisSetAnalysisMethods.AddRange(commonSetAnalysisMethods);
            //iisSetAnalysisMethods.Add(iisSetAnalyzers.IisSummaryGraphs);

            // individual stat analysis
            // statAnalysisMethods.Add(statAnalyzers.SummaryStatComparison);
            // statAnalysisMethods.Add(commonStatAnalyzers.SummaryStatComparisonAsTables);

            //iisSamples.Analyze(iisSetAnalysisMethods, statAnalysisMethods);

            var azureMetricsSetAnalysisMethods = new List<SetAnalyzer<AzureMetricsSampleSet, AzureMetricsSample>>();
            azureMetricsSetAnalysisMethods.AddRange(commonSetAnalysisMethods);
            //azureMetricsSetAnalysisMethods.Add(azureMetricsSetAnalyzers.AzureMetricsSummaryGraphs);

            // individual stat analysis
            // statAnalysisMethods.Add(statAnalyzers.SummaryStatComparison);
            // statAnalysisMethods.Add(commonStatAnalyzers.SummaryStatComparisonAsTables);

            azureSamples.Analyze(azureMetricsSetAnalysisMethods, statAnalysisMethods);
            azureMetricsSetAnalyzers.AzureMetricsSummaryGraphs(azureSamples);

            //------------------------------------------------------------------

            var document = new Document();
            
            // Add notes about the previous report if available
            if (!string.IsNullOrEmpty(options.MarkdownNotesFileName))
            {
                string markdown = File.ReadAllText(options.MarkdownNotesFileName);
                document.AddMarkdown(markdown);
            }

            const int numHeaderLevels = 6;
            for (int i = 1; i <= numHeaderLevels; i++)
            {
                var mdHeaderName = string.Format("MdHeading{0}", i);
                var mdHeaderBaseName = string.Format("Heading{0}", i);
                var mdHeaderStyle = new Style(mdHeaderName, mdHeaderBaseName)
                    {
                        Font = {Bold = true, Size = 8 + (numHeaderLevels - i)*2},
                        ParagraphFormat = {SpaceBefore = Unit.FromPoint(numHeaderLevels - i)}
                    };
                document.Add(mdHeaderStyle);
            }

            var style = document.Styles.Normal;
            style.ParagraphFormat.SpaceBefore = Unit.FromPoint(6d);
            style.ParagraphFormat.SpaceAfter = Unit.FromPoint(6d);
 
            document.AddMarkdown(@"# Analysis of the Aggregate Set

The following sections are various analysis over the entire set of data collected.
");

            // Add the Analysis Notes for the set
            foreach (var analysisNote in iagoSamples.AnalysisNotes)
            {
                document.AppendAnalysisNote(analysisNote);
            }

            foreach (var analysisNote in azureSamples.AnalysisNotes)
            {
                document.AppendAnalysisNote(analysisNote);
            }

            foreach (var analysisNote in iisSamples.AnalysisNotes)
            {
                document.AppendAnalysisNote(analysisNote);
            }

//            document.AddMarkdown(@"# Each Stat
//
//The following sections are an analysis for anomolies for each entry in the collected stats file.
//
//");

//            foreach (var stat in stats.Statistics)
//            {
//                var timestamp = string.Format("###{0}\n", stat.TimeStamp);
//                document.AppendMarkdown(timestamp);
//                foreach (var analysisNote in stat.AnalysisNotes)
//                {
//                    document.AppendAnalysisNote(analysisNote);
//                }
//            }


            //TODO: would be nice to reduce the size of the "Each Stats" output section
            // below worked for all top level paragraphs, but needs to recursively go through each
            // element.Elements recursively and apply the font size to each paragraph.
            // NOTE: using GetType/ "is" from object worked fine for me, but making element a 
            // DocumentElement -> I couldn't use it as a paragraph etc...???? Odd???
            //foreach (object element in document.LastSection.Elements)
            //{                
            //    if (element is Paragraph)
            //    {
            //        (element as Paragraph).Format.Font.Size = Unit.FromPoint(8);
            //    }
            //}

            const string ext = ".pdf";
            string outputFileName = options.OutputFileName;
            document.SaveFile(outputFileName, ext);

            //Process.Start(fileName);

            // pause if a debugger is running, so we can see any console output
            if (Debugger.IsAttached)
            {
                Console.WriteLine("-----------------------");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
    }
}
