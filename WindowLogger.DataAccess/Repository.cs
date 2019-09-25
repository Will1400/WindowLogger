using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WindowLogger.DataAccess
{
    public class Repository
    {
        private readonly string fileLocation;
        private readonly string fileName = "WindowLog.csv";

        public Repository(string fileLocation = null)
        {
            if (fileLocation is null)
                this.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{fileName}";
            else
                this.fileLocation = fileLocation + $"\\{fileName}";

            if (!File.Exists(this.fileLocation))
            {
                using (StreamWriter writer = new StreamWriter(this.fileLocation))
                {
                    writer.WriteLine("Time of Focus, Time of lost focus , Window Title");
                }
            }
        }

        public async Task LogAsync(string text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileLocation, true))
                {
                    await writer.WriteLineAsync(text);
                }
            }
            catch (Exception)
            {
            }
        }


    }
}
