﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LaserOptics.Common;
using LaserYaml;
using NorwegianBlue.Azure;
using NorwegianBlue.Azure.DTOs.WebSiteGetHistoricalUsageMetricsResponse;

namespace LaserOptics.AzureMetricsStats
{
    public class AzureMetricsStatisticsSet : IStatisticsSet, IStatisticsSetAnalysis
    {
        private readonly IDictionary<string, object> _configuration;

        public AzureMetricsStatisticsSet()
        {
            _configuration = YamlParser.GetConfiguration();
        }

        public ReadOnlyCollection<IStatisticsValues> Statistics { get; private set; }
        public ReadOnlyCollection<AnalysisNote> AnalysisNotes { get; private set; }
        public DateTime ActualStartTime { get; private set; }
        public DateTime ActualEndTime { get; private set; }
        public DateTime DesiredStartTime { get; set; }
        public DateTime DesiredEndTime { get; set; }

        public IStatisticsValues GetNearest(DateTime time)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ReadOnlyDictionary<string, double>> ExportStatistics(bool firstRowHeaders = true, string defValue = "missing")
        {
            throw new System.NotImplementedException();
        }

        public dynamic AnalysisScratchPad { get; private set; }
        public void AddAnalysisNote(AnalysisNote note)
        {
            throw new System.NotImplementedException();
        }

        public void Analyze(IEnumerable<SetAnalyzer> setAnalyzers, IEnumerable<StatAnalyzer> statAnalyzers)
        {
            throw new System.NotImplementedException();
        }

        public void Parse(string dataLocation = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var publishSettings = (IDictionary<object, object>)_configuration["PublishSettings"];
            var azureWebSiteManager = new AzureWebSiteManager(new PublishSettings
            {
                ManagementCertificate = publishSettings["ManagementCert"].ToString(),
                SubscriptionId = publishSettings["SubId"].ToString(),
                WebSpace = publishSettings["WebSpace"].ToString(),
                WebSite = publishSettings["WebSite"].ToString()
            });

            var azureGetHistoricalUsageMetricsDto = new AzureGetHistoricalUsageMetricsRequest
            {
                MetricNames = new List<string> { "CpuTime", "AverageMemoryWorkingSet", "MemoryWorkingSet" },
                StartTime = new DateTime(2014, 2, 24, 16, 13, 0),
                EndTime = new DateTime(2014, 2, 24, 16, 40, 0),
            };

            var azureResponse = azureWebSiteManager.GetHistoricalUsageMetrics(azureGetHistoricalUsageMetricsDto);
        }

        public IEnumerator<IStatisticsValues> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IStatisticsValues item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IStatisticsValues item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IStatisticsValues[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IStatisticsValues item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public int IndexOf(IStatisticsValues item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IStatisticsValues item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IStatisticsValues this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}