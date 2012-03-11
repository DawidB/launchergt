using System.Security.Cryptography;
using System.Text;
using System;

namespace DataEncryptionLib
{    
    public class AesEncrypter
    {
        private AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();

        /// <summary>
        /// Konstruktor domyslne, automatycznie generujący klucz szyfrujący oraz wektor inicjalizacji.
        /// Klucz i wektor ważne są wyłącznie dla pojedynczej sesji działania aplikacji.
        /// </summary>
        public AesEncrypter()
        {
            aesCSP.GenerateKey();
            aesCSP.GenerateIV();
        }

        /// <summary>
        /// Konstruktor przyjmujący zdefiniowany przez użytkownika klucz szyfrujący oraz wektor inicjalizacji.
        /// Klucz i wektor są ważne podczas każdorazowego uruchamiania aplikacji.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_iv"></param>
        public AesEncrypter(string _key, string _initVector)
        {
            //AesEncrypter _aes = new AesEncrypter("HrJpIPjRye8ycBsSYum1fJplEfb05/hz", "gWBVw8Ytz2wlhZuOIBuckw==");

            try
            {
                aesCSP.Key = Convert.FromBase64String(_key);
                aesCSP.IV = Convert.FromBase64String(_initVector);
            }
            catch
            {
                aesCSP.GenerateKey();
                aesCSP.GenerateIV();
            }
        }
        
        /// <summary>
        /// Metoda szyfrująca łańcuch znaków i zwracająca tablicę wartości typu "byte".
        /// </summary>
        /// <param name="_stringToEncrypt"></param>
        /// <returns></returns>
        public byte[] EncryptString(string _stringToEncrypt)
        {
            byte[] inBlock = UnicodeEncoding.Unicode.GetBytes(_stringToEncrypt);
            ICryptoTransform xfrm = aesCSP.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

            return outBlock;
        }

        /// <summary>
        /// Metoda deszyfrująca informację podaną za pomocą tablicy typu "byte" do łańcucha znaków.
        /// </summary>
        /// <param name="_bytesToDecrypt"></param>
        /// <returns></returns>
        public string DecryptBytes(byte[] _bytesToDecrypt)
        {
            ICryptoTransform xfrm = aesCSP.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(_bytesToDecrypt, 0, _bytesToDecrypt.Length);

            return UnicodeEncoding.Unicode.GetString(outBlock);
        }

        /// <summary>
        /// Uproszczona metoda szyfrowania - przyjmuje, szyfruje i zwraca wartośc typu "string".
        /// </summary>
        /// <param name="_stringToEncrypt"></param>
        /// <returns></returns>
        public string GetEncryptedString(string _stringToEncrypt)
        {
            byte[] _decryptedArray = EncryptString(_stringToEncrypt);

            return Convert.ToBase64String(_decryptedArray);
        }

        /// <summary>
        /// Uproszczona metoda deszyfrowania - przyjmuje, deszyfruje i zwraca wartośc typu "string".
        /// </summary>
        /// <param name="_stringToDecrypt"></param>
        /// <returns></returns>
        public string GetDecryptedString(string _stringToDecrypt)
        {
            byte[] _encryptedArray = Convert.FromBase64String(_stringToDecrypt);
            
            return DecryptBytes(_encryptedArray);
        }
    }
}
