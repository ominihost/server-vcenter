using System;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Forms;

namespace OminiHost_Server
{
    public class Omini
    {
        public static void debug(string str)
        {
            Debug.WriteLine("[" + DateTime.Now + "] " + str);
        }
        public static void ShowERRO(string titulo, string msg, bool fechar = false)
        {
            MessageBox.Show(msg, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (fechar)
            {
                Environment.Exit(0);
            }
        }
        public static void var_dump(object obj)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(obj));
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData,bool showError=false)
        {
            string saida = "";
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                saida = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception e)
            {
                if (showError == false)
                {       
                    return "";
                }
                if (e.HResult == -2146233033)
                {
                    ShowERRO("ERRO", "Texto digitado não é um texto encryptado em base64\nEncrypt primeiro para poder desencryptar");
                }                
            }
            return saida;
        }
    }
}
