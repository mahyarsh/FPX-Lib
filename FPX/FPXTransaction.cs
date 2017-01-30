using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public class FPXTransaction
    {
        public string OrderNo { get; set; }
        public string ExOrderNo { get; set; }
        public DateTime Date { get; set; }
        public FPXCurrency Currency { get; set; }
        public decimal Amount { get; set; }

        public string ProductDescription { get; set; }
        public string MakerName { get; set; }

        public FPXBuyer Buyer { get; set; }
        public FPXMerchant Merchant { get; set; }


        public FPXTransaction(FPXMerchant merchant)
        {
            this.Merchant = merchant;
        }
        public FPXTransaction()
        {

        }

    }

    public class FPXCurrency
    {
        public string Code { get; set; }
        public string Name { get; set; }

        internal FPXCurrency(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public FPXCurrency()
        {

        }
    }

    public class FPXSupportedCurrencies
    {
        public static FPXCurrency MYR { get; private set; }

        static FPXSupportedCurrencies()
        {
            MYR = new FPXCurrency("Malaysian Ringit", "MYR");
        }
    }
}
