using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public class FPXMerchant
    {
        public string ExchangeID { get; set; }
        public string MerchantID { get; set; }
        public string BankCode { get; set; }

        public FPXMerchant()
        {
            this.BankCode = "01";
        }

        public FPXMerchant(string exchangeID, string merchantID):base()
        {
            this.ExchangeID = exchangeID;
            this.MerchantID = merchantID;
        }
    }
}
