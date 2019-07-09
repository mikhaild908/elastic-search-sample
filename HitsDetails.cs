using System;
namespace OrderLineGeo
{
    public class HitsDetails
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public double _score { get; set; }
        public Source _source { get; set; }
    }
}
