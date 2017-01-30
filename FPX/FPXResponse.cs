using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace MS.FPX
{
    public class FPXResponse
    {
        public string OrderNo { get; set; }
        public string ExOrderNo { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public string CreditAuthenticationCode { get; set; }
        public string DebitAuthenticationCode { get; set; }
        public string BankName { get; set; }
        public bool PaymentIsSuccessful { get; set; }
        public string MessageType { get; set; }
        public string TransactionDate { get; set; }



        public FPXResponse(Page page)
            : this(page.Request)
        {

        }

        public FPXResponse(HttpRequest request)
            : this(request.Form)
        {

        }

        public FPXResponse(string parameters, bool ignoreChecksum = false)
            : this(FPXResUtil.GetValidParams(parameters), ignoreChecksum)
        {

        }


        public FPXResponse(NameValueCollection values, bool ignoreChecksum = false)
        {
            PaymentIsSuccessful = false;

            String fpx_buyerBankBranch = values["fpx_buyerBankBranch"];
            String fpx_buyerBankId = values["fpx_buyerBankId"];
            String fpx_buyerIban = values["fpx_buyerIban"];
            String fpx_buyerId = values["fpx_buyerId"];
            String fpx_buyerName = values["fpx_buyerName"];
            String fpx_creditAuthCode = values["fpx_creditAuthCode"];
            String fpx_creditAuthNo = values["fpx_creditAuthNo"];
            String fpx_debitAuthCode = values["fpx_debitAuthCode"];
            String fpx_debitAuthNo = values["fpx_debitAuthNo"];
            String fpx_fpxTxnId = values["fpx_fpxTxnId"];
            String fpx_fpxTxnTime = values["fpx_fpxTxnTime"];
            String fpx_makerName = values["fpx_makerName"];
            String fpx_msgToken = values["fpx_msgToken"];
            String fpx_msgType = values["fpx_msgType"];
            String fpx_sellerExId = values["fpx_sellerExId"];
            String fpx_sellerExOrderNo = values["fpx_sellerExOrderNo"];
            String fpx_sellerId = values["fpx_sellerId"];
            String fpx_sellerOrderNo = values["fpx_sellerOrderNo"];
            String fpx_sellerTxnTime = values["fpx_sellerTxnTime"];
            String fpx_txnAmount = values["fpx_txnAmount"];
            String fpx_txnCurrency = values["fpx_txnCurrency"];
            String fpx_checkSum = values["fpx_checkSum"];




            if (!ignoreChecksum)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(fpx_buyerBankBranch).Append("|");
                sb.Append(fpx_buyerBankId).Append("|");
                sb.Append(fpx_buyerIban).Append("|");
                sb.Append(fpx_buyerId).Append("|");
                sb.Append(fpx_buyerName).Append("|");
                sb.Append(fpx_creditAuthCode).Append("|");
                sb.Append(fpx_creditAuthNo).Append("|");
                sb.Append(fpx_debitAuthCode).Append("|");
                sb.Append(fpx_debitAuthNo).Append("|");
                sb.Append(fpx_fpxTxnId).Append("|");
                sb.Append(fpx_fpxTxnTime).Append("|");
                sb.Append(fpx_makerName).Append("|");
                sb.Append(fpx_msgToken).Append("|");
                sb.Append(fpx_msgType).Append("|");
                sb.Append(fpx_sellerExId).Append("|");
                sb.Append(fpx_sellerExOrderNo).Append("|");
                sb.Append(fpx_sellerId).Append("|");
                sb.Append(fpx_sellerOrderNo).Append("|");
                sb.Append(fpx_sellerTxnTime).Append("|");
                sb.Append(fpx_txnAmount).Append("|");
                sb.Append(fpx_txnCurrency);



                String fpx_checkSumString = sb.ToString();
                if (!FPXCertificateController.Current.VerifyMessage(fpx_checkSumString, fpx_checkSum))
                    throw new Exception("Invalid FPX message, Checksum does not match");
            }





            OrderNo = fpx_sellerOrderNo;
            ExOrderNo = fpx_sellerExOrderNo;
            Amount = decimal.Parse(fpx_txnAmount);
            Currency = fpx_txnCurrency;
            TransactionID = fpx_fpxTxnId;
            CreditAuthenticationCode = fpx_creditAuthCode;
            DebitAuthenticationCode = fpx_debitAuthCode;
            BankName = fpx_buyerBankBranch;
            MessageType = fpx_msgType;
            TransactionDate = fpx_fpxTxnTime;

            if (fpx_creditAuthCode == "00" && fpx_debitAuthCode == "00")
                PaymentIsSuccessful = true;
        }

    }


    internal class FPXResUtil
    {
        internal static NameValueCollection GetValidParams(string parameters)
        {
            parameters = parameters.Replace("\n", "").Replace("\r", "").Trim();
            return HttpUtility.ParseQueryString(parameters);
        }
    }
}
