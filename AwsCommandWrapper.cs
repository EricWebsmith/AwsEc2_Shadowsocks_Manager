using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AwsEc2Manager
{
    public static class AwsCommandWrapper
    {
        public static string Ssh(string publicDns)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = @"C:\Users\eric";
            startInfo.FileName = "ssh";
            startInfo.Arguments = $" -i usa.pem ec2-user@{publicDns}";
            startInfo.RedirectStandardError = true;
            Process p = Process.Start(startInfo);
            using (StreamReader errorReader = p.StandardError)
            {
                string errorMessage = errorReader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return errorMessage;
                }
            }
            return "SSH Closed";
        }

        public static string Call(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = @"C:\Users\eric";
            startInfo.FileName = "aws";
            startInfo.Arguments = arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            //startInfo.Arguments=""
            Process p = Process.Start(startInfo);
            StreamReader sr = p.StandardOutput;
            string output = sr.ReadToEnd();
            return output;
        }

        public static List<Server> GetServers()
        {
            List<Server> servers = new List<Server>();
            string arguments = "ec2 describe-instances";
            string  output = Call(arguments);
            dynamic json = JsonConvert.DeserializeObject(output);
            foreach(var reservation in json["Reservations"])
            {
                foreach(var instance in reservation["Instances"])
                {
                    Server server = new Server();
                    server.Id = instance["InstanceId"].Value;
                    server.Name = instance["Tags"][0]["Value"].Value;
                    server.State = instance["State"]["Name"].Value;
                    server.LaunchTime = instance["LaunchTime"].Value;
                    server.PublicDns = instance["PublicDnsName"].Value;
                    server.IP = instance["PublicIpAddress"]?.Value;
                    servers.Add(server);
                }
            }
            return servers;
        }
    }
}
