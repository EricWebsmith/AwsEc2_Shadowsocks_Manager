using System;

namespace AwsEc2Manager
{
    public class Server
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string PublicDns { get; set; }

        public string IP { get; set; }

        public DateTime LaunchTime { get; set; }
    }
}
