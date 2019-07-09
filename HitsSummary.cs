using System;
using System.Collections.Generic;

namespace OrderLineGeo
{
    public class HitsSummary
    {
        public int total { get; set; }
        public double max_score { get; set; }
        public List<HitsDetails> hits { get; set; }
    }
}
