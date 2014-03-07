﻿using System;
using LaserOptics.Common;
using LaserPrinter.Graphs.MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;

namespace LaserPrinter.Graphs.Obsolete
{
    public class LineGraph : MigraDocGraph
    {
        public LineGraph(GraphData graphData) : base(graphData)
        {
            if (graphData.SeriesData.Count != 1)
            {
                throw new ArgumentException("Graph data series count is not equal to 1 ...");
            }
        }

        public override void Draw(Document document)
        {
            var chart = SetUp(ChartType.Line, document);

            Series series = chart.SeriesCollection.AddSeries();
            series.Name = GraphData.SeriesData[0].Name;
            series.Add(GraphData.SeriesData[0].Data.ToArray());

            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "X-Axis";

            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.Title.Caption = "Y-Axis";
            chart.YAxis.HasMajorGridlines = true;

            SetGlobalChartOptions(chart);

            document.LastSection.Add(chart);
        }
    }
}