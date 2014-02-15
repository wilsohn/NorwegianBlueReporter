﻿using System;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using StatsReader;

namespace LaserPrinter.Graphs
{
    public class LineStackedGraph : Graph
    {
        public LineStackedGraph(GraphData graphData)
            : base(graphData)
        {
        }

        public override void Draw(Document document)
        {
            var chart = SetUp(ChartType.Line, document);

            foreach (var seriesData in GraphData.SeriesData)
            {
                Series series = chart.SeriesCollection.AddSeries();
                series.Name = seriesData.Name;
                series.Add(seriesData.Data.ToArray());
            }

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