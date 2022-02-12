using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSIAPIClient.Models
{
    public class AuthenticateData
    {
        public string client { get; set; }

        public string secret_key { get; set; }
    }
}
