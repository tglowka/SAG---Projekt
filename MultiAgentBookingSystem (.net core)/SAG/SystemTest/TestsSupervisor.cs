using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.SystemTest.Models;
using MultiAgentBookingSystem.SystemTest.Services;
using SAG.SystemTest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAG.SystemTest
{
    public static class InputFileAdditionalOptions
    {
        public static SystemDelays SystemDelays { get; set; }
    }

    public class TestsSupervisor
    {
        private SystemTestsService _systemTestsService;

        public InputFile InputFile { get; private set; }

        public TestsSupervisor(string inputFilesDirectory, string inputFileName)
        {
            this._systemTestsService = new SystemTestsService();

            this.InputFile = this._systemTestsService.GetInputFIle(inputFilesDirectory, inputFileName);

            this.SetupDeepLogging();
            this.SetupSystemDelays();
            this.SetupSingleRouteCount();
        }


        private void SetupSingleRouteCount()
        {
            TicketsHelper.singleRouteCount = this.InputFile.InitiazlSingleRouteTicketsCount;
        }

        private void SetupDeepLogging()
        {
            LoggingConfiguration.Instance.DeepLogging = this.InputFile.DeepLogging;
        }

        public void SetupSystemDelays()
        {
            InputFileAdditionalOptions.SystemDelays = this.InputFile.SystemDelays;
        }

    }

}
