using System.IO;

using FluentAssertions;

using FolderObserver.Common;

using Xunit;

namespace UnitTests
{
    public class UnitTestCompressionFileSystem
    {
        private const string TestDataFileName = "test-data";

        [Fact]
        public void TestMethodCreate()
        {
            if (File.Exists(TestDataFileName + ".zip"))
            {
                File.Delete(TestDataFileName + ".zip");
            }

            FileCompressor.Compress(TestDataFileName + ".txt");
            bool zipExist = File.Exists(TestDataFileName + ".zip");
            zipExist.Should().BeTrue();
            //Assert.AreEqual(true,zipExist);
        }
    }
}
