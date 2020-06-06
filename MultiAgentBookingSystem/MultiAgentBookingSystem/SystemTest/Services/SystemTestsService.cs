using MultiAgentBookingSystem.SystemTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.SystemTest.Services
{
    public class SystemTestsService
    {
        private readonly string InputFilesDirectory = @"\SystemTest\TestInputFiles\";

        public SystemTestsService()
        {

        }

        public InputFile GetInputFIle(string inputFileName)
        {
            try
            {
                string currentExecutableDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string inputFilePath = currentExecutableDirectory + this.InputFilesDirectory + inputFileName;
                string inputFileContent = File.ReadAllText(inputFilePath);

                InputFile inputFile = JsonConvert.DeserializeObject<InputFile>(inputFileContent);

                return inputFile;
            }
            catch
            {
                throw;
            }
        }

    }
}

