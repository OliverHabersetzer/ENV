using System;
using NUnit.Framework;

namespace ENV.Debug
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void _LoggerTest()
        {
            var log = new Logger(true, "D:/");
            log.Info(log.LogFilePath);
            log.Info("This is a \nmultiline\nstring!");
            log.Warning("Warning");
            log.Error("Error");
        }
    }
}