using System;
using Xunit;

namespace Pluralsight.MockingWithMoq
{
    public class MockTests
    {

        [Fact]
        public void SensitiveDataShouldBeScrubbedFromTheLogMessage()
        {
            var _mockScrubber = new MockScrubber();
            var _mockHeader = new MockHeader();
            var _mockFooter = new MockFooter();
            var _mockSystemConfig = new MockSystemConfig();


            var logger = new Logging(_mockScrubber, _mockHeader, _mockFooter, _mockSystemConfig);
            logger.CreateEntryFor("", new LogLevel());

            Assert.True(_mockScrubber.FromWasCalled);
        }
    }

    public class MockScrubber : IScrubSensitiveData
    {
        public bool FromWasCalled { get; private set; }

        public string From(string message)
        {
            FromWasCalled = true;
            return string.Empty;
        }
    }

    public class MockHeader : ICreateLogEntryHeaders
    {
        public void For(LogLevel logLevel)
        {
            
        }
    }

    public class MockFooter : ICreateLogEntryFooter
    {
        public void For(LogLevel logLevel)
        {
            
        }
    }

    public class MockSystemConfig : IConfigureSystem
    {
        public bool LogStackFor(LogLevel logLevel)
        {
            //But what if I want this to sometimes return true?
            return false;
        }
    }


    public class Logging
    {
        IScrubSensitiveData _scrubSensitiveData;
        ICreateLogEntryHeaders _createLogEntryHeaders;
        ICreateLogEntryFooter _createLogEntryFooter;
        IConfigureSystem _configureSystem;

        public Logging(IScrubSensitiveData scrubSensitiveData,
                       ICreateLogEntryHeaders createLogEntryHeaders,
                       ICreateLogEntryFooter createLogEntryFooter,
                       IConfigureSystem configureSystem)
        {
            _scrubSensitiveData = scrubSensitiveData;
            _createLogEntryHeaders = createLogEntryHeaders;
            _createLogEntryFooter = createLogEntryFooter;
            _configureSystem = configureSystem;
        }

        public void CreateEntryFor(string message, LogLevel logLevel)
        {
            _createLogEntryHeaders.For(logLevel);

            if (_configureSystem.LogStackFor(logLevel))
            {
                Console.WriteLine(string.Format("Stack /n/n {0}",
                    Environment.StackTrace));
            }

            Console.Write(string.Format("{0} - {1}",
                logLevel,
                _scrubSensitiveData.From(message)));

            _createLogEntryFooter.For(logLevel);
        }
    }
    
}
