using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public class Constant
    {
        public const string FPX_Test_Service_URL = "https://uat.mepsfpx.com.my/FPXMain/sellerNVPReceiver.jsp";
        public const string FPX_Production_Service_URL = "https://www.mepsfpx.com.my/FPXMain/sellerNVPReceiver.jsp";

        public const string FPX_Test_AE_Service_URL = //"https://uat.mepsfpx.com.my/FPXMain/sellerNVPReceiver.jsp";
                                                    "https://uat.mepsfpx.com.my/FPXMain/sellerNVPTxnStatus.jsp";

        public const string DateFormat = "yyyyMMddHHmmss";
    }

    public enum FPXMode
    {
        Production,
        Development
    }
}
