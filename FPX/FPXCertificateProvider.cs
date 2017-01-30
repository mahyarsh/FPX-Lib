using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public abstract class FPXCertificateProvider
    {
        public abstract FPXCertificateData GetCertificate();

        protected DateTime GetCertExpireDate(X509Certificate2 cert)
        {
            String[] date = cert.GetExpirationDateString().Split(' ');
            try
            {
                DateTime CertDate = DateTime.ParseExact(date[0], "M/dd/yyyy", null);
                return CertDate;
            }
            catch
            {
                DateTime CertDate = DateTime.ParseExact(date[0], "dd/M/yyyy", null);
                return CertDate;
            }
        }
    }

    public class FPXCertificateData
    {
        public X509Certificate2 Certificate { get; private set; }
        public X509Certificate2 AlternativeCertificate { get; private set; }


        public FPXCertificateData(X509Certificate2 mainCert, X509Certificate2 altCert = null)
        {
            if (mainCert == null)
                throw new Exception("Invalid main certificate");

            this.Certificate = mainCert;
            this.AlternativeCertificate = altCert;
        }

    }


    public class SingleFileCertificateProvider : FPXCertificateProvider
    {
        private string CertificateFile;

        public override FPXCertificateData GetCertificate()
        {
            X509Certificate2 x509_2;
            x509_2 = new X509Certificate2(CertificateFile);
            DateTime expireDate = GetCertExpireDate(x509_2);
            if (expireDate < DateTime.Today)
                throw new Exception("Invalid certificate, cert has expired");

            return new FPXCertificateData(x509_2, null);

        }

        public SingleFileCertificateProvider(string certFile)
        {
            if (!File.Exists(certFile))
                throw new FileNotFoundException(certFile);

            CertificateFile = certFile;
        }
    }

    public class MultipleFileCertificateProvider : FPXCertificateProvider
    {
        private string FolderPath;

        public override FPXCertificateData GetCertificate()
        {
            //TODO: Implement later
            throw new NotImplementedException();
        }

        public MultipleFileCertificateProvider(string certsFolder)
        {
            if (!Directory.Exists(certsFolder))
                throw new DirectoryNotFoundException(certsFolder);

            FolderPath = certsFolder;
        }
    }
}
