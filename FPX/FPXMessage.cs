using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MS.FPX
{
    public class FPXMessageType
    {
        public const string AE = "AE";
        public const string AR = "AR";
    }


    public class FPXMessage
    {
        public FPXTransaction Transaction { get; set; }

        public string MessageType { get; set; }
        public string MessageToken { get; set; }
        public string FPXVersion { get; set; }


        public FPXMessage(FPXTransaction transaction)
        {
            if (transaction == null)
                throw new Exception("Invalid Transaction");

            if (transaction.Buyer == null)
                throw new Exception("Invalid Buyer");



            this.FPXVersion = "5.0";
            this.Transaction = transaction;
            this.MessageType = FPXMessageType.AR;
            this.MessageToken = "01";
        }
        public FPXMessage()
        {

        }

        public string GetChecksum()
        {
            StringBuilder checkSum = new StringBuilder();
            checkSum.Append(Transaction.Buyer.BuyerAccountNo).Append("|");      //1
            checkSum.Append(Transaction.Buyer.BuyerBankBranch).Append("|");
            checkSum.Append(Transaction.Buyer.BuyerBankID).Append("|");
            checkSum.Append(Transaction.Buyer.BuyerEmail).Append("|");
            checkSum.Append(Transaction.Buyer.BuyerIBAN).Append("|");           //5
            checkSum.Append(Transaction.Buyer.BuyerID).Append("|");
            checkSum.Append(Transaction.Buyer.BuyerName).Append("|");
            checkSum.Append(Transaction.MakerName).Append("|");
            checkSum.Append(MessageToken).Append("|");
            checkSum.Append(MessageType).Append("|");                           //10
            checkSum.Append(Transaction.ProductDescription).Append("|");
            checkSum.Append(Transaction.Merchant.BankCode).Append("|");
            checkSum.Append(Transaction.Merchant.ExchangeID).Append("|");
            checkSum.Append(Transaction.ExOrderNo).Append("|");
            checkSum.Append(Transaction.Merchant.MerchantID).Append("|");       //15
            checkSum.Append(Transaction.OrderNo).Append("|");
            checkSum.Append(Transaction.Date.ToString(Constant.DateFormat)).Append("|");
            checkSum.Append(Transaction.Amount).Append("|");
            checkSum.Append(Transaction.Currency.Code).Append("|");
            checkSum.Append(FPXVersion);                                        //20



            return checkSum.ToString().Trim();
        }

        public string GetRSAChecksum()
        {
            return FPXCertificateController.Current.RSASign(GetChecksum());
        }

        public void TestChecksum(string certificate)
        {
            FPXCertificateController.Test(GetRSAChecksum(), GetChecksum(), certificate);
        }

        public string GetXML()
        {
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(FPXMessage));
            using (StringWriter sw = new StringWriter())
            {
                xs.Serialize(sw, this);
                return sw.ToString();
            }
        }


        public static FPXMessage FromXML(string xml)
        {
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(FPXMessage));
            using (StringReader sr = new StringReader(xml))
            {
                return (FPXMessage)xs.Deserialize(sr);
            }
        }

        public string GetParameters()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}={1}&", "fpx_msgType", MessageType);
            sb.AppendFormat("{0}={1}&", "fpx_msgToken", MessageToken);
            sb.AppendFormat("{0}={1}&", "fpx_sellerId", Transaction.Merchant.MerchantID);
            sb.AppendFormat("{0}={1}&", "fpx_sellerExId", Transaction.Merchant.ExchangeID);
            sb.AppendFormat("{0}={1}&", "fpx_sellerExOrderNo", Transaction.ExOrderNo);
            sb.AppendFormat("{0}={1}&", "fpx_sellerTxnTime", Transaction.Date.ToString(Constant.DateFormat));
            sb.AppendFormat("{0}={1}&", "fpx_sellerOrderNo", Transaction.OrderNo);
            sb.AppendFormat("{0}={1}&", "fpx_sellerBankCode", Transaction.Merchant.BankCode);
            sb.AppendFormat("{0}={1}&", "fpx_txnCurrency", Transaction.Currency.Code);
            sb.AppendFormat("{0}={1}&", "fpx_txnAmount", Transaction.Amount.ToString());
            sb.AppendFormat("{0}={1}&", "fpx_buyerEmail", Transaction.Buyer.BuyerEmail);
            sb.AppendFormat("{0}={1}&", "fpx_buyerName", Transaction.Buyer.BuyerName);
            sb.AppendFormat("{0}={1}&", "fpx_buyerBankId", Transaction.Buyer.BuyerBankID);
            sb.AppendFormat("{0}={1}&", "fpx_buyerBankBranch", Transaction.Buyer.BuyerBankBranch);
            sb.AppendFormat("{0}={1}&", "fpx_buyerAccNo", Transaction.Buyer.BuyerAccountNo);
            sb.AppendFormat("{0}={1}&", "fpx_buyerId", Transaction.Buyer.BuyerID);
            sb.AppendFormat("{0}={1}&", "fpx_makerName", Transaction.MakerName);
            sb.AppendFormat("{0}={1}&", "fpx_buyerIban", Transaction.Buyer.BuyerIBAN);
            sb.AppendFormat("{0}={1}&", "fpx_productDesc", Transaction.ProductDescription);
            sb.AppendFormat("{0}={1}&", "fpx_version", FPXVersion);


            string checksum = GetRSAChecksum();
            sb.AppendFormat("{0}={1}", "fpx_checkSum", checksum);

            return sb.ToString();
        }
    }
}
