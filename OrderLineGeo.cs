using System;

namespace OrderLineGeo
{
    public class OrderLineGeo
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public HitsSummary hits { get; set; }
    }
}
