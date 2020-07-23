using CsvHelper;
using CsvHelper.Configuration.Attributes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace EtsyRestAPI
{
    class Program
    {
        class EtsyListing
        {
            [Index(0)]
            public int ListingId { get; set; }
            [Index(1)]
            public string Title { get; set; }
            [Index(2)]
            public string Price { get; set; }
            [Index(3)]
            public int Quantity { get; set; }
            [Index(4)]
            public int Views { get; set; }
            [Index(5)]
            public int Favorers { get; set; }
            [Index(6)]
            public string ShopName { get; set; }
            [Index(7)]
            public string ShopOwner { get; set; }
            [Index(8)]
            public string Location { get; set; }
        }

        static void Main(string[] args)
        {
            List<EtsyListing> etsyList = new List<EtsyListing>();
            foreach (var item in GetListings().results)
            {
                EtsyListing etsyModel = new EtsyListing()
                {
                    ListingId = item.listing_id,
                    Title = item.title,
                    Price = item.price,
                    Quantity = item.quantity,
                    Views = item.views,
                    Favorers = item.num_favorers,
                    ShopOwner = (item.User.Profile.first_name == null || item.User.Profile.last_name == null) ?
                                item.User.Profile.first_name + " " + item.User.Profile.last_name : item.User.login_name,
                    ShopName = item.Shop.shop_name,
                    Location = item.User.Profile.region + " " + item.User.Profile.city,

                };
                
                etsyList.Add(etsyModel);

                Console.WriteLine(etsyModel.ListingId);
                Console.WriteLine(etsyModel.Title);
                Console.WriteLine(etsyModel.Price);
                Console.WriteLine(etsyModel.Quantity);
                Console.WriteLine(etsyModel.Views);
                Console.WriteLine(etsyModel.Favorers);
                Console.WriteLine(etsyModel.ShopOwner);
                Console.WriteLine(etsyModel.ShopName);
                Console.WriteLine(etsyModel.Location);
            }

            Console.Write("Do you want to download as CSV File ? (Y/N) : ");
            string answer = Console.ReadLine();

            answer = answer.ToLower().Trim();
            string path = @"C:\Users\Rogh-PC\source\repos\EtsyRestAPI\EtsyRestAPI\csv\MyTest.csv";

            if (answer == "y" || answer == "yes" || string.IsNullOrEmpty(answer))
            {
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(etsyList);
                }
            }

        }

        public static dynamic GetListings()
        {
            var client = new RestClient("https://openapi.etsy.com/v2");
            var request = new RestRequest("listings/active")
                .AddParameter("api_key", "zdhw0u396oimuo7k3t2e1g5n")
                .AddParameter("limit", 20)
                .AddParameter("keywords", "art print")
                .AddParameter("sort_on", "score")
                .AddParameter("fields", "listing_id,title,price,quantity,views,num_favorers")
                .AddParameter("includes", "User/Profile(region,city,first_name,last_name)," +
                                          "Shop(shop_name,UserAddress.first_line,UserAddress.second_line,num_favorers,login_name,listing_active_count)");

            var response = client.Get(request);

            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content);
            return responseObject;
        }

    }
}
