using Microsoft.Azure.Management.ResourceManager.Fluent;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common
{
    public static class Utilities
    {
        public static Action<string> LoggerMethod { get; set; }
        public static Func<string> PauseMethod { get; set; }

        public static string ProjectPath { get; set; }
        static Utilities()
        {
            LoggerMethod = Console.WriteLine;
            PauseMethod = Console.ReadLine;
            ProjectPath = ".";
        }
        public static void Log(string message)
        {
            LoggerMethod.Invoke(message);
        }

        public static void Log(object obj)
        {
            if (obj != null)
            {
                LoggerMethod.Invoke(obj.ToString());
            }
            else
            {
                LoggerMethod.Invoke("(null)");
            }
        }

        public static void Log()
        {
            Log("");
        }

        internal static string GetArmTemplate(string templateFileName)
        {
            var adminUsername = "tirekicker";
            var adminPassword = "12NewPA$$w0rd!";
            var hostingPlanName = SdkContext.RandomResourceName("hpRSAT", 24);
            var webAppName = SdkContext.RandomResourceName("wnRSAT", 24);
            var armTemplateString = File.ReadAllText(Path.Combine(ProjectPath, "Asset", templateFileName));

            var parsedTemplate = JObject.Parse(armTemplateString);

            if (string.Equals("ArmTemplate.json", templateFileName, StringComparison.OrdinalIgnoreCase))
            {
                parsedTemplate.SelectToken("parameters.hostingPlanName")["defaultValue"] = hostingPlanName;
                parsedTemplate.SelectToken("parameters.webSiteName")["defaultValue"] = webAppName;
                parsedTemplate.SelectToken("parameters.skuName")["defaultValue"] = "B1";
                parsedTemplate.SelectToken("parameters.skuCapacity")["defaultValue"] = 1;
            }
            return parsedTemplate.ToString();
        }
    }
}
