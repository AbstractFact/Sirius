using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.DTOs
{
    public class SubscribeDTO
    {
        public List<string> SubList { get; set; }
        public List<string> UnsubList { get; set; }
    }
}
