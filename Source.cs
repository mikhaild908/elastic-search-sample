using System;
namespace OrderLineGeo
{
    public class Source
    {
        public long object_id { get; set; }
        public long order_line_id { get; set; }
        public long master_product_id { get; set; }
        public int default_product_category_id { get; set; }
        public double retail_unit_price { get; set; }
        public double default_unit_price { get; set; }
        public int product_type_id { get; set; }
        public DateTime date_expected { get; set; }
        public int store_group_id { get; set; }
        public string store_region { get; set; }
        public string store_region_group { get; set; }
        public int? promo_type_id { get; set; }
        public int zipcode { get; set; }
        public string location { get; set; }
        public bool is_conversion { get; set; }
        public int percent_discount { get; set; }
        public double profit_margin { get; set; }
        public double margin_percentage { get; set; }
        public double markup_percentage { get; set; }
        public DateTime datetime_sold { get; set; }
        public DateTime datetime_rollup_added { get; set; }
        public DateTime datetime_rollup_modified { get; set; }
        public DateTime datetime_es_last_updated { get; set; }
    }
}
