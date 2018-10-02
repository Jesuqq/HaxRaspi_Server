using System;

namespace HaxRaspi_Server.Entities {
    public class Interface {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public string Broadcast { get; set; }
        public string IPv6 { get; set; }
        public string MAC { get; set; }
        public bool Connected { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}