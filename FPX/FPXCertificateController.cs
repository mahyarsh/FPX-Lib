using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MS.FPX
{
    public class FPXCertificateController
    {
        private string PrivateKeyFile = null;
        private FPXCertificateProvider CertificateProvider = null;
        private static FPXCertificateController instance = null;


        public static FPXCertificateController Current
        {
            get
            {
                if (instance == null)
                    throw new Exception("Please call Initialize method before using the controller");

                return instance;
            }
        }

        public static void Initialize(string privateKeyFile, FPXCertificateProvider certProvider)
        {
            if (string.IsNullOrWhiteSpace(privateKeyFile))
                throw new Exception("Invalid private key file");

            if (!File.Exists(privateKeyFile))
                throw new Exception("Private key file does not exists");

            if (certProvider == null)
                throw new Exception("Invalid certificate provider");

            instance = new FPXCertificateController()
            {
                PrivateKeyFile = privateKeyFile,
                CertificateProvider = certProvider
            };
        }

        public virtual string RSASign(string data)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(PrivateKeyFile);
            byte[] dataBytes = System.Text.Encoding.Default.GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return BitConverter.ToString(signatureBytes).Replace("-", null);
        }

        public virtual bool VerifyMessage(string requestChecksum, string responseChecksum)
        {
            var certs = CertificateProvider.GetCertificate();
            if (certs == null)
                throw new Exception("Invalid certificate result");


            try
            {
                RSACryptoServiceProvider rsaEncryptor;
                Boolean checkCert = false;
                byte[] plainData = System.Text.Encoding.Default.GetBytes(requestChecksum);
                byte[] signatureData = HexToBytes(responseChecksum);

                if (certs.AlternativeCertificate == null)
                {
                    rsaEncryptor = (RSACryptoServiceProvider)certs.Certificate.PublicKey.Key;
                    checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                }
                else
                {
                    rsaEncryptor = (RSACryptoServiceProvider)certs.Certificate.PublicKey.Key;
                    checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                    if (!checkCert)
                    {
                        rsaEncryptor = (RSACryptoServiceProvider)certs.AlternativeCertificate.PublicKey.Key;
                        checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
                    }
                }


                if (!checkCert)
                {
                    //ErrorCode = "09";
                    throw new Exception("[09] Your Data cannot be verified against the Signature.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("[03] ERROR:: " + ex.Message, ex);
            }

            throw new Exception("[09] Your Data cannot be verified against the Signature.");
        }


        public static void Test(string encryptedText, string planText, string certificate)
        {
            RSACryptoServiceProvider rsaEncryptor;
            Boolean checkCert = false;
            byte[] plainData = System.Text.Encoding.Default.GetBytes(planText);
            byte[] signatureData = HexToBytes(encryptedText);

            X509Certificate2 x509_2;
            x509_2 = new X509Certificate2(certificate);
            rsaEncryptor = (RSACryptoServiceProvider)x509_2.PublicKey.Key;
            checkCert = rsaEncryptor.VerifyData(plainData, "SHA1", signatureData);
            if (!checkCert)
                throw new Exception("Private and public key does not match");
        }


        private RSACryptoServiceProvider LoadCertificateFile(string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }


                RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res);
                return rsa;
            }
        }

        private byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        private RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
                {
                    highbyte = binr.ReadByte(); // data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;     // we already have the data size
                }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        public static byte[] HexToBytes(string hex)
        {
            //Code from FPX sample, not verified
            hex = hex.Trim();

            byte[] bytes = new byte[hex.Length / 2];

            for (int index = 0; index < bytes.Length; index++)
            {
                bytes[index] = byte.Parse(hex.Substring(index * 2, 2), NumberStyles.HexNumber);
            }

            return bytes;
        }




        private FPXCertificateController()
        {

        }
    }
}
