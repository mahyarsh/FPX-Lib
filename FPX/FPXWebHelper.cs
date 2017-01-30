using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MS.FPX
{
    public class FPXWebHelper
    {
        private static HtmlInputHidden GetHiddenField(string name, string value)
        {
            return new HtmlInputHidden() { ID = name, Name = name, Value = value };
        }

        public static void AddHiddenFields(Control host, FPXMessage message)
        {
            host.Controls.Add(GetHiddenField("fpx_msgType",          message.MessageType));
            host.Controls.Add(GetHiddenField("fpx_msgToken",         message.MessageToken));
            host.Controls.Add(GetHiddenField("fpx_sellerId",         message.Transaction.Merchant.MerchantID));
            host.Controls.Add(GetHiddenField("fpx_sellerExId",       message.Transaction.Merchant.ExchangeID));
            host.Controls.Add(GetHiddenField("fpx_sellerExOrderNo",  message.Transaction.ExOrderNo));
            host.Controls.Add(GetHiddenField("fpx_sellerTxnTime",    message.Transaction.Date.ToString(Constant.DateFormat)));
            host.Controls.Add(GetHiddenField("fpx_sellerOrderNo",    message.Transaction.OrderNo));
            host.Controls.Add(GetHiddenField("fpx_sellerBankCode",   message.Transaction.Merchant.BankCode));
            host.Controls.Add(GetHiddenField("fpx_txnCurrency",      message.Transaction.Currency.Code));
            host.Controls.Add(GetHiddenField("fpx_txnAmount",        message.Transaction.Amount.ToString()));
            host.Controls.Add(GetHiddenField("fpx_buyerEmail",       message.Transaction.Buyer.BuyerEmail));
            host.Controls.Add(GetHiddenField("fpx_buyerName",        message.Transaction.Buyer.BuyerName));
            host.Controls.Add(GetHiddenField("fpx_buyerBankId",      message.Transaction.Buyer.BuyerBankID));
            host.Controls.Add(GetHiddenField("fpx_buyerBankBranch",  message.Transaction.Buyer.BuyerBankBranch));
            host.Controls.Add(GetHiddenField("fpx_buyerAccNo",       message.Transaction.Buyer.BuyerAccountNo));
            host.Controls.Add(GetHiddenField("fpx_buyerId",          message.Transaction.Buyer.BuyerID));
            host.Controls.Add(GetHiddenField("fpx_makerName",        message.Transaction.MakerName));
            host.Controls.Add(GetHiddenField("fpx_buyerIban",        message.Transaction.Buyer.BuyerIBAN));
            host.Controls.Add(GetHiddenField("fpx_productDesc",      message.Transaction.ProductDescription));
            host.Controls.Add(GetHiddenField("fpx_version",          message.FPXVersion));


            string checksum = message.GetRSAChecksum();
            host.Controls.Add(GetHiddenField("checkSum_String",      checksum)); //message.GetChecksum()
            host.Controls.Add(GetHiddenField("fpx_checkSum",         checksum));
        }

        public static HtmlForm GetHtmlForm(FPXMode mode, FPXMessage message)
        {
            var form = new HtmlForm();
            form.Action = mode == FPXMode.Production ? Constant.FPX_Production_Service_URL : Constant.FPX_Test_Service_URL;
            form.Method = "POST";

            AddHiddenFields(form, message);

            return form;
        }

        public static FPXResponse GetResponse(HttpRequest request)
        {
            String fpx_buyerBankBranch = request.Form["fpx_buyerBankBranch"];
            String fpx_buyerBankId = request.Form["fpx_buyerBankId"];
            String fpx_buyerIban = request.Form["fpx_buyerIban"];
            String fpx_buyerId = request.Form["fpx_buyerId"];
            String fpx_buyerName = request.Form["fpx_buyerName"];
            String fpx_creditAuthCode = request.Form["fpx_creditAuthCode"];
            String fpx_creditAuthNo = request.Form["fpx_creditAuthNo"];
            String fpx_debitAuthCode = request.Form["fpx_debitAuthCode"];
            String fpx_debitAuthNo = request.Form["fpx_debitAuthNo"];
            String fpx_fpxTxnId = request.Form["fpx_fpxTxnId"];
            String fpx_fpxTxnTime = request.Form["fpx_fpxTxnTime"];
            String fpx_makerName = request.Form["fpx_makerName"];
            String fpx_msgToken = request.Form["fpx_msgToken"];
            String fpx_msgType = request.Form["fpx_msgType"];
            String fpx_sellerExId = request.Form["fpx_sellerExId"];
            String fpx_sellerExOrderNo = request.Form["fpx_sellerExOrderNo"];
            String fpx_sellerId = request.Form["fpx_sellerId"];
            String fpx_sellerOrderNo = request.Form["fpx_sellerOrderNo"];
            String fpx_sellerTxnTime = request.Form["fpx_sellerTxnTime"];
            String fpx_txnAmount = request.Form["fpx_txnAmount"];
            String fpx_txnCurrency = request.Form["fpx_txnCurrency"];
            String fpx_checkSum = request.Form["fpx_checkSum"];
            String fpx_checkSumString = "";
            fpx_checkSumString = fpx_buyerBankBranch + "|" + fpx_buyerBankId + "|" + fpx_buyerIban + "|" + fpx_buyerId + "|" + fpx_buyerName + "|" + fpx_creditAuthCode + "|" + fpx_creditAuthNo + "|" + fpx_debitAuthCode + "|" + fpx_debitAuthNo + "|" + fpx_fpxTxnId + "|" + fpx_fpxTxnTime + "|" + fpx_makerName + "|" + fpx_msgToken + "|" + fpx_msgType + "|";
            fpx_checkSumString += fpx_sellerExId + "|" + fpx_sellerExOrderNo + "|" + fpx_sellerId + "|" + fpx_sellerOrderNo + "|" + fpx_sellerTxnTime + "|" + fpx_txnAmount + "|" + fpx_txnCurrency;

            
            String mesg_type = request.Form["mesg_type"];
            String mesgFromFpx = request.Form["mesgFromFpx"];
            String mesg_token = request.Form["mesg_token"];
            String key_type = request.Form["key_type"];
            String seller_ex_desc = request.Form["seller_ex_desc"];
            String seller_ex_id = request.Form["seller_ex_id"];
            String order_no = request.Form["order_no"];
            String seller_txn_time = request.Form["seller_txn_time"];
            String seller_order_no = request.Form["seller_order_no"];
            String seller_id = request.Form["seller_id"];
            String seller_fpx_bank_code = request.Form["seller_fpx_bank_code"];
            String buyer_mail_id = request.Form["buyer_mail_id"];
            String txn_amt = request.Form["txn_amt"];
            String checksum = request.Form["checksum"];
            String debit_auth_code = request.Form["debit_auth_code"];
            String debit_auth_no = request.Form["debit_auth_no"];
            String buyer_bank = request.Form["buyer_bank"];
            String buyer_bank_branch = request.Form["buyer_bank_branch"];
            String buyer_name = request.Form["buyer_name"];
            String FPX_TXN_ID = request.Form["FPX_TXN_ID"];
            String finalVerifiMsg = "ERROR";
            String xmlMessage = "";
            xmlMessage = fpx_checkSumString;

            FPXCertificateController.Current.VerifyMessage(xmlMessage, fpx_checkSum);

            return null;
        }
    }
}


/*

<input name="fpx_msgType        ">1
<input name="fpx_msgToken       ">2
<input name="fpx_sellerExId     ">3
<input name="fpx_sellerExOrderNo">4
<input name="fpx_sellerTxnTime  ">5
<input name="fpx_sellerOrderNo  ">6
<input name="fpx_sellerBankCode ">7
<input name="fpx_txnCurrency    ">8
<input name="fpx_txnAmount      ">9
<input name="fpx_buyerEmail     ">0
<input name="fpx_checkSum       ">1
<input name="fpx_buyerName      ">2
<input name="fpx_buyerBankId    ">3
<input name="fpx_buyerBankBranch">4
<input name="fpx_buyerAccNo     ">5
<input name="fpx_buyerId        ">6
<input name="fpx_makerName      ">7
<input name="fpx_buyerIban      ">8
<input name="fpx_productDesc    ">9
<input name="fpx_version        ">0
<input name="fpx_sellerId       ">1
<input name="checkSum_String    ">2



 
<input type="hidden" value='<%=fpx_msgType%>' name="fpx_msgType">
<input type="hidden" value='<%=fpx_msgToken%>' name="fpx_msgToken">
<input type="hidden" value='<%=fpx_sellerExId%>' name="fpx_sellerExId">
<input type="hidden" value='<%=fpx_sellerExOrderNo%>' name="fpx_sellerExOrderNo">
<input type="hidden" value='<%=fpx_sellerTxnTime%>' name="fpx_sellerTxnTime">
<input type="hidden" value='<%=fpx_sellerOrderNo%>' name="fpx_sellerOrderNo">
<input type="hidden" value='<%=fpx_sellerBankCode%>' name="fpx_sellerBankCode">
<input type="hidden" value='<%=fpx_txnCurrency%>' name="fpx_txnCurrency">
<input type="hidden" value='<%=fpx_txnAmount%>' name="fpx_txnAmount">
<input type="hidden" value='<%=fpx_buyerEmail%>' name="fpx_buyerEmail">
<input type="hidden" value='<%=checksum%>' name="fpx_checkSum">
<input type="hidden" value='' name="fpx_buyerName">
<input type="hidden" value='<%=fpx_buyerBankId%>' name="fpx_buyerBankId">
<input type="hidden" value='' name="fpx_buyerBankBranch">
<input type="hidden" value='' name="fpx_buyerAccNo">
<input type="hidden" value='' name="fpx_buyerId">
<input type="hidden" value='' name="fpx_makerName">
<input type="hidden" value='' name="fpx_buyerIban">
<input type="hidden" value='<%=fpx_productDesc%>' name="fpx_productDesc">
<input type="hidden" value='<%=fpx_version%>' name="fpx_version">
<input type="hidden" value='<%=fpx_sellerId%>' name="fpx_sellerId">
<input type="hidden" value='<%=checksum%>' name="checkSum_String">


*/