using Sirius.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.DTOs
{
    public class RequestDTO
    {
        public string ID { get; set; }
        public Request Request { get; set; }
    }
}
