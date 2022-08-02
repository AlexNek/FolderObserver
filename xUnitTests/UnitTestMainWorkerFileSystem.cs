using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using FolderObserver;
using FolderObserver.Common;
using FolderObserver.Model;

using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UnitTestMainWorkerFileSystem
    {
        private readonly ITestOutputHelper _testOutput;

        private const string SourceFolderTest = @"C:\TempTestSrcAn";

        private const string SourceFileName = "test.txt";

        private const string TargetFolderTest = @"C:\TempTestTrgAn";

        private readonly DataItems _items = new DataItems();

        private readonly IDataSerializer _serializer = new TestSerializer();

        public UnitTestMainWorkerFileSystem( ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            CreateTestData();
        }

        ~UnitTestMainWorkerFileSystem()
        {
            CleanupData();
        }

        private static void CreateTestData()
        {
            if (!Directory.Exists(TargetFolderTest))
            {
                Directory.CreateDirectory(TargetFolderTest);
            }

            if (!Directory.Exists(SourceFolderTest))
            {
                Directory.CreateDirectory(SourceFolderTest);
            }

            string[] lines = { "First line", "Second line", "Third line" };

            string sourceFullFileName = Path.Combine(SourceFolderTest, SourceFileName);
            File.WriteAllLines(sourceFullFileName, lines);
        }

        private void CleanupData()
        {
            // reset data
            if (Directory.Exists(TargetFolderTest))
            {
                Directory.Delete(TargetFolderTest,true);
            }
        }

        [Fact]
        public void TestAddOneItem()
        {
            MainWorker worker = new MainWorker();

            //CreateTestData();
            bool stored = false;

            worker.TargetFolder = TargetFolderTest;
            FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.All, SourceFolderTest, SourceFileName);
            //FileItem item = new FileItem() { Name = SourceFileName };
            worker.AddWorkItem(args);
            worker.RunHandlingOfWorkingItems(
                (item) =>
                    {
                        _items.Add(item);
                    },
                () =>
                    {
                        //_serializer.Store(_items);
                        stored = true;
                    },
                (ex, item) =>
                    {
                        item.IsError = true;
                    }
                );

            worker.IsRunning.Should().BeTrue();
            worker.CancellationPending = true;
            while (worker.IsRunning)
            {
                Thread.Sleep(100);
            }

            _testOutput.WriteLine("items count {0} stored:{1}", _items.Count, stored);
            FileItem fileItem = _items[0];

            _items.Count.Should().Be(1);
            fileItem.IsError.Should().BeFalse();
            stored.Should().BeTrue("It must be call for storing collection");
        }

        [Fact]
        public void TestMethodMove()
        {
            string srcFullFileName = Path.Combine(SourceFolderTest, SourceFileName);
            string destFullFileName = Path.Combine(TargetFolderTest, SourceFileName);
            string[] sourceFiles;
            string[] targetFiles;
            sourceFiles = Directory.GetFiles(SourceFolderTest, SourceFileName);
            targetFiles = Directory.GetFiles(TargetFolderTest, SourceFileName);
            sourceFiles.Length.Should().Be(1, "Source file must be present");
            targetFiles.Length.Should().Be(0, "Target file must be absent");
            //Assert.AreEqual(1, sourceFiles.Length, "Source file must be present");
            //Assert.AreEqual(0, targetFiles.Length, "Target file must be absent");

            FileMover.Move(srcFullFileName, destFullFileName);
            sourceFiles = Directory.GetFiles(SourceFolderTest, SourceFileName);
            targetFiles = Directory.GetFiles(TargetFolderTest, SourceFileName);

            sourceFiles.Length.Should().Be(0);
            targetFiles.Length.Should().Be(1);
            //Assert.AreEqual(0,sourceFiles.Length);
            //Assert.AreEqual(1, targetFiles.Length);
        }

        [Fact]
        public void TestMethodMoveArchive()
        {
            string srcFullFileName = Path.Combine(SourceFolderTest, SourceFileName);
            string arcFullFileName = FileCompressor.GetArchiveFileName(srcFullFileName);
            string arcFileName = Path.GetFileName(arcFullFileName);
            string destFullFileName = Path.Combine(TargetFolderTest, arcFileName);
            string[] sourceFiles;
            string[] sourceZipFiles;
            string[] targetFiles;
            sourceFiles = Directory.GetFiles(SourceFolderTest, SourceFileName);
            sourceZipFiles = Directory.GetFiles(SourceFolderTest, arcFileName);
            targetFiles = Directory.GetFiles(TargetFolderTest, arcFileName);
            sourceFiles.Length.Should().Be(1, "Source file must be present");
            sourceZipFiles.Length.Should().Be(0, "Source compressed file must be absent");
            targetFiles.Length.Should().Be(0, "Target compressed file must be absent");

            //Assert.AreEqual(1, sourceFiles.Length, "Source file must be present");
            //Assert.AreEqual(0, sourceZipFiles.Length, "Source compressed file must be absent");
            //Assert.AreEqual(0, targetFiles.Length, "Target compressed file must be absent");

            FileCompressor.Compress(srcFullFileName);
            sourceZipFiles = Directory.GetFiles(SourceFolderTest, arcFileName);
            sourceZipFiles.Length.Should().Be(1);

            //Assert.AreEqual(1, sourceZipFiles.Length, "Source compressed file must be present");

            FileMover.Move(arcFullFileName, destFullFileName);
            sourceZipFiles = Directory.GetFiles(SourceFolderTest, arcFileName);
            targetFiles = Directory.GetFiles(TargetFolderTest, arcFileName);
            sourceZipFiles.Length.Should().Be(0, "Source compressed file must be absent");
            targetFiles.Length.Should().Be(1, "Target compressed file must be present");

            //Assert.AreEqual(0, sourceZipFiles.Length, "Source compressed file must be absent");
            //Assert.AreEqual(1, targetFiles.Length, "Target compressed file must be present");
        }

        private class TestSerializer : IDataSerializer
        {
            private static readonly DataItems _items = new DataItems();

            public DataItems Load()
            {
                return _items;
            }

            public async Task<DataItems> LoadAsync()
            {
                return await Task.FromResult(_items);
            }

            public void Store(DataItems data)
            {
            }

            public Task StoreAsync(DataItems data)
            {
                return null;
            }
        }
    }
}

/*
 * Too much time for it
 * https://habr.com/ru/post/150859/
 */