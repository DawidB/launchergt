using System.Security.Cryptography;
using System.Text;
using System;

namespace DataEncryptionLib
{    
    public class AesEncrypter
    {
        private AesCryptoServiceProvider _aesCSP = new AesCryptoServiceProvider();
        public AesCryptoServiceProvider AesCSP { get { return _aesCSP; } }

        /// <summary>
        /// Default constructor. Automatically generates crypt key and initialization vector.
        /// Key and vector are valid only for active session.
        /// </summary>
        public AesEncrypter()
        {
            _aesCSP.GenerateKey();
            _aesCSP.GenerateIV();
        }

        /// <summary>
        /// Constructor accepting crypt key and initialization vector defined by user.
        /// Key and vector are valid for multiple sessions.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_iv"></param>
        public AesEncrypter(string _key, string _initVector)
        {
            //AesEncrypter _aes = new AesEncrypter("HrJpIPjRye8ycBsSYum1fJplEfb05/hz", "gWBVw8Ytz2wlhZuOIBuckw==");

            try
            {
                _aesCSP.Key = Convert.FromBase64String(_key);
                _aesCSP.IV = Convert.FromBase64String(_initVector);
            }
            catch
            {
                _aesCSP.GenerateKey();
                _aesCSP.GenerateIV();
            }
        }
        
        /// <summary>
        /// Encrypts string and return array of byte.
        /// </summary>
        /// <param name="_stringToEncrypt"></param>
        /// <returns></returns>
        public byte[] EncryptString(string _stringToEncrypt)
        {
            try
            {
                byte[] inBlock = UnicodeEncoding.Unicode.GetBytes(_stringToEncrypt);
                ICryptoTransform xfrm = _aesCSP.CreateEncryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

                return outBlock;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Decrypts array of byte and returns string.
        /// </summary>
        /// <param name="_bytesToDecrypt"></param>
        /// <returns></returns>
        public string DecryptBytes(byte[] _bytesToDecrypt)
        {
            try
            {
                ICryptoTransform xfrm = _aesCSP.CreateDecryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(_bytesToDecrypt, 0, _bytesToDecrypt.Length);

                return UnicodeEncoding.Unicode.GetString(outBlock);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Simplified encrypter - returns encrypted string.
        /// </summary>
        /// <param name="_stringToEncrypt"></param>
        /// <returns></returns>
        public string GetEncryptedString(string _stringToEncrypt)
        {
            try
            {
                byte[] _decryptedArray = EncryptString(_stringToEncrypt);

                return Convert.ToBase64String(_decryptedArray);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Simplified decrypter - returns decrypted string.
        /// </summary>
        /// <param name="_stringToDecrypt"></param>
        /// <returns></returns>
        public string GetDecryptedString(string _stringToDecrypt)
        {
            try
            {
                byte[] _encryptedArray = Convert.FromBase64String(_stringToDecrypt);

                return DecryptBytes(_encryptedArray);
            }
            catch
            {
                return "";
            }
        }
    }
}
