﻿using System;
using System.Collections.Generic;

namespace NorwegianBlue.Analysis.Samples
{
    public class SampleTimeComparer: IComparer<ISampleValues>
    {
        public int Compare(ISampleValues x, ISampleValues y)
        {
            return DateTime.Compare(x.TimeStamp, y.TimeStamp);
        }
    }
}
