﻿
namespace Asiacell.ITADLibWCF_v1.Business
{
    public class ElementLogin
    {
        public int Id { get; set; }
        public int ElementTypeID { get; set; }
        public string Name { get; set; }
        public string LoginKey { get; set; }
        public int Status { get; set; }
        public string Url { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int RoundRobin { get; set; }
    }
}
