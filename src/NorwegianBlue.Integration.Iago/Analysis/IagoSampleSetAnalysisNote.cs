﻿using System.Collections.Generic;
using NorwegianBlue.Notes.AnalysisNotes;
using NorwegianBlue.Reporting;
using OxyPlot;

namespace NorwegianBlue.Integration.Iago.Analysis
{
    [ReportingMetaData(ReportingTypes.SampleSet)]
    class IagoSampleSetAnalysisNote : AnalysisNote
    {
        public override string FriendlyTypeName
        {
            get { return @"Iago Sample Set Analysis"; }
        }

        public IagoSampleSetAnalysisNote(string name, string summary, AnalysisNoteDetailLevel noteDetailLevel,
                                AnalysisNotePriorities notePriority, List<PlotModel> graphData)
            : base(name, summary, noteDetailLevel, notePriority, graphData)
        {
        }
    }
}
