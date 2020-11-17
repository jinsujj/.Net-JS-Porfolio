using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonLibrary
{
    public class Security
    {
        private readonly string _key;
        public Security()
        {
            this._key = "MyAPP_JS";
        }
        public Security(string key)
        {
            this._key = key;
        }

        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            byte[] numArray;
            if (useHashing)
            {
                MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
                // Seed 문자열에 대한 해시 배열 생성
                numArray = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(this._key));
                cryptoServiceProvider.Clear();
            }
            else
                // Seed 문자열에 대한 배열 생성
                numArray = Encoding.UTF8.GetBytes(this._key);

            TripleDESCryptoServiceProvider cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider1.Key = numArray;
            cryptoServiceProvider1.Mode = CipherMode.ECB;
            cryptoServiceProvider1.Padding = PaddingMode.PKCS7;

            // 현재 Key 속성 및 초기화 벡터(IV)를 사용하여 대칭 encryptor 개체를 만듭니다.
            // 지정된 바이트 배열의 지정된 영역에 대해 해시 값을 계산합니다. (inputbuffer, inputoffset, inputcount);
            byte[] inArray = cryptoServiceProvider1.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            // SymmetricAlgorithm 클래스에서 사용하는 모든 리소스를 해제합니다.
            cryptoServiceProvider1.Clear();

            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        public string Decrypt(string cipherString, bool useHashing)
        {
            byte[] inputbuffer = Convert.FromBase64String(cipherString);
            byte[] numArray1;
            if (useHashing)
            {
                MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
                numArray1 = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(this._key));
                cryptoServiceProvider.Clear();
            }
            else
                numArray1 = Encoding.UTF8.GetBytes(this._key);
            TripleDESCryptoServiceProvider cryptoServiceProvider1 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider1.Key = numArray1;
            cryptoServiceProvider1.Mode = CipherMode.ECB;
            cryptoServiceProvider1.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = cryptoServiceProvider1.CreateDecryptor();
            byte[] numArray2 = new byte[inputbuffer.Length];
            byte[] bytes;
            try
            {
                bytes = decryptor.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            }
            catch
            {
                bytes = new byte[0];
            }
            cryptoServiceProvider1.Clear();
            return Encoding.UTF8.GetString(bytes);
        }

        public string EncryptPassword(string password) => this.SHA256Hash(this.MD5Hash(password));

        private string MD5Hash(string Data)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(Data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in hash)
                stringBuilder.AppendFormat("{0:x2}", (object)num);
            return stringBuilder.ToString();
        }

        private string SHA256Hash(string Data)
        {
            byte[] hash = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(Data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in hash)
                stringBuilder.AppendFormat("{0:x2}", (object)num);
            return stringBuilder.ToString();
        }
    }
}
