using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
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
        public SystemTestsService()
        {

        }

        #region public methods

        public InputFile GetInputFIle(string inputFilesDirectorystring, string inputFileName)
        {
            try
            {
                string currentExecutableDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string inputFilePath = currentExecutableDirectory + inputFilesDirectorystring + inputFileName;
                string inputFileContent = File.ReadAllText(inputFilePath);

                InputFile inputFile = JsonConvert.DeserializeObject<InputFile>(inputFileContent);

                return inputFile;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}

