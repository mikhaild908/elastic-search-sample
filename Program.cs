using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace OrderLineGeo
{
    class Program
    {
        const string POST_URL = "http://es-dev.techstyle.tech/dev_order_line_geo/order_line/_search";
        const string USERNAME = "<username>";
        const string PASSWORD = "<password>";

        static int _numberOfItems = 0;
        //static int _numberOfNullPromoTypeIds = 0;
        static int _numberOfItemsSoldBeforeMinDate = 0;
        static int _numberOfRuns = 0;
        static string _dateTimeSoldMin = "1970-01-01";

        static void Main(string[] args)
        {
            try
            {
                Console.Write("Number of runs: ");
                _numberOfRuns = Int32.Parse(Console.ReadLine());
                Console.Write("\n");

                Console.Write("Max number of items per run: ");
                _numberOfItems = Int32.Parse(Console.ReadLine());
                Console.Write("\n");

                Console.Write("Minimum Sold Date(ex. 1970-01-01): ");
                _dateTimeSoldMin = Console.ReadLine();
                Console.Write("\n");

                var list = GetAllDataAsync();

                foreach (var item in list.Result)
                {
                    PrintResults(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        static async Task<string> GetDataAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{PASSWORD}")));

                //var json = "{\n    \"query\":{\n\t\t\"bool\": {\n\t\t\t\"must\": [\n\t\t\t\t\t\t{ \"term\": { \"store_group_id\": \"16\" } },\n\t\t\t\t\t\t{ \"range\": { \"datetime_sold\": { \"gte\": \"1970-01-01\" } } },\n\t\t\t\t\t\t{ \"term\": { \"is_conversion\": \"true\" } },\n\t\t\t\t\t\t{ \"bool\": { \"should\": [\n\t\t\t\t\t\t\t\t\t\t{ \"term\": { \"default_product_category_id\": \"207\" } },\n\t\t\t\t\t\t\t\t\t\t{ \"term\": { \"default_product_category_id\": \"209\" } }\n\t\t\t\t\t\t\t\t\t]\n\t\t\t\t\t\t\t\t  }\n\t\t\t\t\t\t}\n\t\t\t\t\t],\n\t\t\t\"filter\":\t{\n\t\t\t\t\t\t\t\"geo_distance\":  {\n\t\t\t\t\t\t\t\"distance\": \"500mi\",\n\t\t\t\t\t\t\t\"location\": {\n\t\t\t\t\t\t\t\t\"lat\": \"33.7866\",\n\t\t\t\t\t\t\t\t\"lon\": \"-118.299\"\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t}\n\t\t\t}\n\t\t}\n\t},\n\t\"aggs\": {\n\t\t\"group_by_listing\": {\n\t\t\t\"terms\": {\n\t\t\t\t\"field\": \"master_product_id\",\n\t\t\t\t\"size\": \"5\"\n\t\t\t}\n\t\t}\n\t},\n    \"size\": 0,\n    \"timeout\": \"10ms\",\n    \"from\": 1\n}";
                var json = "{\"query\":{\n\t\t\"bool\": {\n\t\t\t\"must\": [\n\t\t\t\t\t\t{ \"term\": { \"store_group_id\": \"16\" } },\n\t\t\t\t\t\t{ \"range\": { \"datetime_sold\": { \"gte\": \"{dateTimeSoldMin}\" } } },\n\t\t\t\t\t\t{ \"term\": { \"is_conversion\": \"true\" } },\n\t\t\t\t\t\t{ \"bool\": { \"should\": [\n\t\t\t\t\t\t\t\t\t\t{ \"term\": { \"default_product_category_id\": \"207\" } },\n\t\t\t\t\t\t\t\t\t\t{ \"term\": { \"default_product_category_id\": \"209\" } }\n\t\t\t\t\t\t\t\t\t]\n\t\t\t\t\t\t\t\t  }\n\t\t\t\t\t\t}\n\t\t\t\t\t],\n\t\t\t\"filter\":\t{\n\t\t\t\t\t\t\t\"geo_distance\":  {\n\t\t\t\t\t\t\t\"distance\": \"500mi\",\n\t\t\t\t\t\t\t\"location\": {\n\t\t\t\t\t\t\t\t\"lat\": \"33.7866\",\n\t\t\t\t\t\t\t\t\"lon\": \"-118.299\"\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t}\n\t\t\t}\n\t\t}\n\t},\n    \"size\": {_numberOfItems},\n    \"timeout\": \"10ms\",\n    \"from\": 1\n}".Replace("{_numberOfItems}", _numberOfItems.ToString()).Replace("{dateTimeSoldMin}", _dateTimeSoldMin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpResponseMessage result = client.PostAsync(POST_URL, content).Result;

                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //TODO:
                    // refresh token
                    // display results

                    // if (result.StatusCode == HttpStatusCode.Unauthorized)
                    // throw error
                }

                var data = await result.Content.ReadAsStringAsync();

                Console.WriteLine("ID: " + id + " done.");

                return data;
            }
        }

        private static async Task<List<OrderLineGeo>> GetAllDataAsync()
        {
            var list = new List<OrderLineGeo>();
            var tasks = new List<Task<string>>();

            for (int i = 1; i <= _numberOfRuns; i++)
            {
                tasks.Add(GetDataAsync(i));
            }

            foreach (var task in tasks)
            {
                var data = await task;
                list.Add(JsonConvert.DeserializeObject<OrderLineGeo>(data));
            }
            
            return list;
        }

        static void PrintResults(OrderLineGeo orderLineGeo)
        {
            List<HitsDetails> hits = orderLineGeo.hits.hits;

            for (int i = 0; i < hits.Count; i++)
            {
                //if (hits[i]._source.promo_type_id == null)
                //{
                //    _numberOfNullPromoTypeIds++;
                //}

                if (hits[i]._source.datetime_sold < DateTime.Parse(_dateTimeSoldMin))
                {
                    _numberOfItemsSoldBeforeMinDate++;
                }

                //Console.WriteLine($"{i + 1}) Id: {hits[i]._id}");
                //Console.WriteLine($"Master Product Id: {hits[i]._source.master_product_id}");
                //Console.WriteLine($"Default Product Category Id: {hits[i]._source.default_product_category_id}");
                ////Console.WriteLine($"Promo Type Id: {hits[i]._source.promo_type_id}");
                //Console.WriteLine($"Date Sold: {hits[i]._source.datetime_sold}\n");
            }

            //Console.WriteLine($"\n\nThere were {_numberOfNullPromoTypeIds} NULL Promo Type Ids");
            Console.WriteLine($"\nThere were {_numberOfItemsSoldBeforeMinDate} items sold before minimum sold date({_dateTimeSoldMin}).\n");
        }
    }
}

//{
//    "query":{
//		"bool": {
//			"must": [
//						{ "term": { "store_group_id": "16" } },
//						{ "range": { "datetime_sold": { "gte": "1970-01-01" } } },
//						{ "term": { "is_conversion": "true" } },
//						{ "bool": { "should": [
//										{ "term": { "default_product_category_id": "207" } },
//										{ "term": { "default_product_category_id": "209" } }
//									]
//								  }
//						}
//					],
//			"filter":	{
//							"geo_distance":  {
//							"distance": "500mi",
//							"location": {
//								"lat": "33.7866",
//								"lon": "-118.299"
//							}
//						}
//			}
//		}
//	},
//	"aggs": {
//		"group_by_listing": {
//			"terms": {
//				"field": "master_product_id",
//				"size": "5"
//			}
//		}
//	},
//    "size": 0,
//    "timeout": "10ms",
//    "from": 1
//}
