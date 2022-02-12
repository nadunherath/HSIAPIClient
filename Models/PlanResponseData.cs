using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.ComponentModel.Com2Interop;

namespace HSIAPIClient.Models
{
    public class PlanResponseData
    {
        //success":true,"modal_prem":"956.00","message":"Premium Calculated successfully"

        public bool success { get; set; }

        public string modal_prem { get; set; }  

        public string message { get; set; }
    }
}
