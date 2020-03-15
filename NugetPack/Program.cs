using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NugetPack
{
    class Program
    {
        static void Main(string[] args)
        {
            GetVersion(new Package()
            {
                DataPackageId = "AWSSDK.Core"
            });
        }


        public static  List<Package> LoadPage(int pageIndex)
        {
            List<Package> result = new List<Package>();

            var html = $@"https://www.nuget.org/packages?page={pageIndex}";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='package-title']");

            foreach (var item in nodes)
            {
                result.Add(new Package()
                {
                    DataPackageId = item.Attributes["data-package-id"].Value
                });
            }           
            return result;
        }

        public static void GetVersion(Package package)
        {
            var html = $@"https://www.nuget.org/packages/{package.DataPackageId}";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='version-history']/table/tbody[@class='no-border']/tr/td/a");

            var nodes2 = htmlDoc.DocumentNode.SelectNodes("//div[@id='version-history']/table/tbody[@id='hidden-versions']/tr/td/a");
            

            foreach (var n in nodes)
            {
                package.Versions.Add(n.Attributes["title"]?.Value);
            }

            foreach (var n in nodes2)
            {
                package.Versions.Add(n.Attributes["title"]?.Value);
            }


            Console.WriteLine();

            Console.WriteLine(nodes.Count+nodes2.Count);
            Console.ReadKey();
        }

    }


    public class Package
    {
        public string DataPackageId { get; set; }
        public List<string> Versions { get; set; }
    }
}
