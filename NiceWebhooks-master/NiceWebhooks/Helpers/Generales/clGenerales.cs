using System.Security.Cryptography;

namespace NiceWebhooks.Helpers.Generales
{
    public static class clGenerales
    {
        public static string desencriptar(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            
                        }
                    }
                }
            }

            return plaintext;
        }

        public static string urlDecode(string sURL)
        {
            sURL = sURL.Replace("%20", " ");
            sURL = sURL.Replace("%21", "!");
            sURL = sURL.Replace("%22", "\"");
            sURL = sURL.Replace("%23", "#");
            sURL = sURL.Replace("%24", "$");
            sURL = sURL.Replace("%25", "%");
            sURL = sURL.Replace("%26", "&");
            sURL = sURL.Replace("%27", "'");
            sURL = sURL.Replace("%28", "(");
            sURL = sURL.Replace("%29", ")");
            sURL = sURL.Replace("%2A", "*");
            sURL = sURL.Replace("%2B", "+");
            sURL = sURL.Replace("%2C", ",");
            sURL = sURL.Replace("%2E", ".");
            sURL = sURL.Replace("%2F", "/");
            sURL = sURL.Replace("%3D", "=");

            return sURL;
        }

        public static string UrlEncode(string sURL)
        {
            sURL = sURL.Replace(" ", "%20");
            sURL = sURL.Replace("!", "%21");
            sURL = sURL.Replace("\"", "%22");
            sURL = sURL.Replace("#", "%23");
            sURL = sURL.Replace("$", "%24");
            sURL = sURL.Replace("%", "%25");
            sURL = sURL.Replace("&", "%26");
            sURL = sURL.Replace("'", "%27");
            sURL = sURL.Replace("(", "%28");
            sURL = sURL.Replace(")", "%29");
            sURL = sURL.Replace("*", "%2A");
            sURL = sURL.Replace("+", "%2B");
            sURL = sURL.Replace(",", "%2C");
            sURL = sURL.Replace(".", "%2E");
            sURL = sURL.Replace("/", "%2F");

            return sURL;
        }


    }


}
