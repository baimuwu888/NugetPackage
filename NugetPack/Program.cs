using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace NugetPack
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "";

           
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

        //使用要添加引用System.Management.Automation.dll； 
        //此DLL在C:\Program Files (x86)\Reference Assemblies\Microsoft\WindowsPowerShell
        private static string RunScript(string scriptText)
        {

            // create Powershell runspace

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript(scriptText);

            pipeline.Commands.Add("Out-String");

            // execute the script

            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            StringBuilder stringBuilder = new StringBuilder();

            foreach (PSObject obj in results)
            {

                stringBuilder.AppendLine(obj.ToString());

            }

            return stringBuilder.ToString();

        }
        public void RunScript(List<string> scripts)
        {
            try
            {
                Runspace runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                foreach (var scr in scripts)
                {
                    pipeline.Commands.AddScript(scr);
                }
                //返回结果   
                var results = pipeline.Invoke();
                runspace.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); 
            }


        }

    }


    public class Package
    {
        public string DataPackageId { get; set; }
        public List<string> Versions { get; set; }
    }
}
