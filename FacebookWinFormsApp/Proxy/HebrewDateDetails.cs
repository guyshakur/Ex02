using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookWinFormsApp.Proxy
{
    public class HebrewDateDetails
    {
        [JsonProperty("Hebrew")]
        public string HebrewDate { get; set; }
    }
}
