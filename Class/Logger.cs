using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OminiHost_Server.Class
{
    static class Logger
    {
        static string LogsPath = "./Logs";
        static void Init()
        {
            if(!Directory.Exists(LogsPath)) {
                Directory.CreateDirectory(LogsPath);
                Debug.WriteLine("Criando a pasta dos Logs");
            }
        }
        static void WriteLine(string msg)
        {
            
        }
        private static void WriteToFile(string fileName, string msg)
        {
            using (StreamWriter file = new StreamWriter($"{LogsPath}\\{fileName}.txt", true))
            {
                file.WriteLine(msg);
            }
        }

    }
}
