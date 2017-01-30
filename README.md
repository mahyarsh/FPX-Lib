# FPX-Lib
.Net Lib to make transaction with FPX (Malaysian online payment system)

to use FPX, you need merchant certificate, merchant key and exchange certificate, which can obtain from https://fpxexchange.myclear.org.my:8443/MerchantIntegrationKit/ given you already registered with FPX and have merchant ID.

#How to use:
First need to initialize the certificates, only once in your project:
```c#
string key_file = Server.MapPath("~/res/fpx/MercKey.key");
string cert_file = Server.MapPath("~/res/fpx/ExCert.cer");        

FPXCertificateController.Initialize(key_file, new SingleFileCertificateProvider(cert_file));
```

Then you can create transactions and submit to FPX:
```c#
var fpx_merch = new FPXMerchant() { MerchantID = "[MercID]", ExchangeID = "[Exchange Code]", BankCode = "01" };
var fpx_buyer = new FPXBuyer() { };
var fpx_transaction = new FPXTransaction(fpx_merch)
        {
            Amount = 120.45m, // Amount
            Currency = FPXSupportedCurrencies.MYR,
            OrderNo = "[Order No]",
            Date = DateTime.Today,
            ProductDescription = "[Description]",
            Buyer = fpx_buyer,
            ExOrderNo = [Extra Order No]
        };
var fpx_message = new FPXMessage(fpx_transaction);
```

FPX message need to submit through HTTP Request, this library help you to build the form with required values:
```c#
form1.Action = Constant.FPX_Production_Service_URL; //can use FPX_Test_Service_URL for UAT and testing
FPXWebHelper.AddHiddenFields(form1, fpx_message);
```
This will redirect the user to FPX payment gateway. After tranasction is done, the FPX will redirect user to either <i>Direct</i> or <i>Indirect</i> page, based on merchant settings. Following example shows how the status can be checked:
```c#
var fpx_res = new FPXResponse(this);
if (fpx_res.PaymentIsSuccessful)
{
    // successful transaction           
}
else
{
    // transaction failed
}
```
