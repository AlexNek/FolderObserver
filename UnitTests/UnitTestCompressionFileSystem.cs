using System.IO;

using FolderObserver.Common;

using NUnit.Framework;

namespace UnitTests
{
    // Uncomment for local test
    //[TestFixture]
    public class UnitTestCompressionFileSystem
    {
        [Test]
        public void TestMethodCreate()
        {
            FileCompressor.Compress(@"E:\Alex\a932.jpg");
            bool zipExist=File.Exists(@"E:\Alex\a932.jpg");
            Assert.AreEqual(true,zipExist);
        }
    }
}
