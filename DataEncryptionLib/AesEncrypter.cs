namespace DataEncryptionLib
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// A simple class used to encrypt and decrypt strings with given key and initialization vector.
    /// </summary>
    public class AesEncrypter
    {
        /// <summary>
        /// A field containing crypt service.
        /// </summary>
        private AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="AesEncrypter" /> class.
        /// Key and vector are valid only for active session.
        /// </summary>
        public AesEncrypter()
        {
            this.aesCSP.GenerateKey();
            this.aesCSP.GenerateIV();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AesEncrypter" /> class.
        /// Key and vector are valid for multiple sessions.
        /// </summary>
        /// <param name="key">A crypt key</param>
        /// <param name="initVector">An initialization vector.</param>
        public AesEncrypter(string key, string initVector)
        {
            ////AesEncrypter aes = new AesEncrypter("HrJpIPjRye8ycBsSYum1fJplEfb05/hz", "gWBVw8Ytz2wlhZuOIBuckw==");

            try
            {
                this.aesCSP.Key = Convert.FromBase64String(key);
                this.aesCSP.IV = Convert.FromBase64String(initVector);
            }
            catch
            {
                this.aesCSP.GenerateKey();
                this.aesCSP.GenerateIV();
            }
        }

        /// <summary>
        /// Gets an accessor referencing crypt service.
        /// </summary>
        public AesCryptoServiceProvider AesCSP
        {
            get { return this.aesCSP; }
        }
        
        /// <summary>
        /// Encrypts string and return array of byte.
        /// </summary>
        /// <param name="stringToEncrypt">A string to encrypt.</param>
        /// <returns>Encrypted bytes array.</returns>
        public byte[] EncryptString(string stringToEncrypt)
        {
            try
            {
                byte[] inBlock = UnicodeEncoding.Unicode.GetBytes(stringToEncrypt);
                ICryptoTransform xfrm = this.aesCSP.CreateEncryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

                return outBlock;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Decrypts array of byte and returns string.
        /// </summary>
        /// <param name="bytesToDecrypt">Bytes array to decrypt.</param>
        /// <returns>Decrypted string.</returns>
        public string DecryptBytes(byte[] bytesToDecrypt)
        {
            try
            {
                ICryptoTransform xfrm = this.aesCSP.CreateDecryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);

                return UnicodeEncoding.Unicode.GetString(outBlock);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Simplified encrypting function - returns encrypted string.
        /// </summary>
        /// <param name="stringToEncrypt">A string to encrypt.</param>
        /// <returns>Decrypted string array.</returns>
        public string GetEncryptedString(string stringToEncrypt)
        {
            try
            {
                byte[] decryptedArray = this.EncryptString(stringToEncrypt);

                return Convert.ToBase64String(decryptedArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Simplified decrypting function - returns decrypted string.
        /// </summary>
        /// <param name="stringToDecrypt">A string to decrypt.</param>
        /// <returns>Decrypted string.</returns>
        public string GetDecryptedString(string stringToDecrypt)
        {
            try
            {
                byte[] encryptedArray = Convert.FromBase64String(stringToDecrypt);

                return this.DecryptBytes(encryptedArray);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
