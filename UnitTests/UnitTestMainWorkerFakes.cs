using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FakeItEasy;

using FolderObserver;
using FolderObserver.Model;

using log4net.Util;

using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class UnitTestMainWorkerFakes
    {
        private const string SourceFolderTest = @"C:\TempTestSrcAn";

        private const string SourceFileName = "test.txt";

        private const string TargetFolderTest = @"C:\TempTestTrgAn";

        private  string _sourceFullPath;

        private readonly DataItems _items = new DataItems();

        [SetUp]
        public  void RunBeforeAnyTests()
        {
            _sourceFullPath = Path.Combine(SourceFolderTest, SourceFileName);
            //NOTE: uncomment if any troubles with logging
            //LogLog.InternalDebugging = true;
            //Note: uncomment for logging
            //log4net.Config.XmlConfigurator.Configure();
        }

        [Test]
        public void TestMethodSimpleCall()
        {
            MainWorkerUnderTest worker = A.Fake<MainWorkerUnderTest>();
            DateTime creationDate = new DateTime(1999,11,30,21,15,10);
            A.CallTo(() => worker.GetFileCreationTime(A<string>.Ignored)).Returns(creationDate);
           
            bool stored = BaseTest(worker);
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => worker.DeleteFile(A<string>.Ignored)).MustHaveHappenedOnceExactly();


            FileItem fileItem = _items[0];
            Assert.AreEqual(false, fileItem.IsError);
            Assert.AreEqual(SourceFileName, fileItem.Name);
            Assert.AreEqual(creationDate, fileItem.TimeStamp,"Wrong file time stamp");
            Assert.AreEqual(true, stored, "It must be call for storing collection");
        }

        [Test]
        public void TestMethodALotOfCall()
        {
            MainWorkerUnderTest worker = A.Fake<MainWorkerUnderTest>();
            DateTime creationDate = new DateTime(1999, 11, 30, 21, 15, 10);
            A.CallTo(() => worker.GetFileCreationTime(A<string>.Ignored)).Returns(creationDate);

            DateTime starTime;
            starTime = DateTime.UtcNow;
            TimeSpan timeDiffPrev = TimeSpan.Zero;

            A.CallTo(() => worker.CompressFile(A<string>.Ignored))
                .Invokes((string fileName) =>
                    {
                        Task.Delay(200);
                        DateTime finished =DateTime.UtcNow;
                        TimeSpan timeDiff = finished - starTime;

                        Console.Out.WriteLine($"Compress {fileName} absolute:{timeDiff} per step:{timeDiff-timeDiffPrev}");
                        timeDiffPrev = timeDiff;
                    });
            int MaxCount = 10;
            int storedCount=0;

            for (int i = 0; i < MaxCount; i++)
            {
                FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.All, SourceFolderTest, SourceFileName);
                //bool stored = false;

                worker.TargetFolder = TargetFolderTest;
                //FileItem item = new FileItem() { Name = SourceFileName };
                worker.AddWorkItem(args);
                
            }

            Assert.AreEqual(MaxCount, worker.WorkingQueueCount, "Wrong working queue size");

            worker.RunHandlingOfWorkingItems(
                (item) => { _items.Add(item); },
                () => { storedCount++; },
                (ex, item) => { item.IsError = true; }
                );

            Assert.AreEqual(true, worker.IsRunning, "worker must be in running state");

            //worker.CancellationPending = true;
            WaitExecutionFinished(worker);

            A.CallTo(() => worker.GetFileCreationTime(A<string>.That.IsEqualTo(_sourceFullPath))).MustHaveHappened(MaxCount, Times.Exactly);
            A.CallTo(() => worker.CompressFile(A<string>.That.IsEqualTo(_sourceFullPath))).MustHaveHappened(MaxCount, Times.Exactly);
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened(MaxCount, Times.Exactly);
            A.CallTo(() => worker.DeleteFile(A<string>.Ignored)).MustHaveHappened(MaxCount, Times.Exactly);


            FileItem fileItem = _items[0];
            Assert.AreEqual(false, fileItem.IsError);
            Assert.AreEqual(SourceFileName, fileItem.Name);
            Assert.AreEqual(creationDate, fileItem.TimeStamp, "Wrong file time stamp");
            Assert.AreEqual(MaxCount, storedCount, $"It must be {MaxCount} calls for storing collection");
        }

        [Test]
        public void TestCompressException()
        {
            MainWorkerUnderTest worker = A.Fake<MainWorkerUnderTest>();
            DateTime creationDate = new DateTime(1999, 11, 30, 21, 15, 10);
            A.CallTo(() => worker.GetFileCreationTime(A<string>.Ignored)).Returns(creationDate);
            //throw any exception
            A.CallTo(() => worker.CompressFile(A<string>.Ignored))
                .Throws<ArgumentException>();

            bool stored = BaseTest(worker);
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => worker.DeleteFile(A<string>.Ignored)).MustNotHaveHappened();

            FileItem fileItem = _items[0];
            Assert.AreEqual(true, fileItem.IsError, "Item error");
            Assert.AreEqual(SourceFileName, fileItem.Name);
            Assert.AreEqual(creationDate, fileItem.TimeStamp, "Wrong file time stamp");
            Assert.AreEqual(true, stored, "It must be call for storing collection");
        }

        [Test]
        public void TestMoveFileException()
        {
            
            MainWorkerUnderTest worker = A.Fake<MainWorkerUnderTest>();
            DateTime creationDate = new DateTime(1999, 11, 30, 21, 15, 10);
            A.CallTo(() => worker.GetFileCreationTime(A<string>.Ignored)).Returns(creationDate);
            //throw any exception
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored))
                .Throws<ArgumentException>();

            bool stored = BaseTest(worker);
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => worker.DeleteFile(A<string>.Ignored)).MustNotHaveHappened();

            FileItem fileItem = _items[0];
            Assert.AreEqual(true, fileItem.IsError, "Item error");
            Assert.AreEqual(SourceFileName, fileItem.Name);
            Assert.AreEqual(creationDate, fileItem.TimeStamp, "Wrong file time stamp");
            Assert.AreEqual(true, stored, "It must be call for storing collection");
        }

        [Test]
        public void TestDeleteFileException()
        {

            MainWorkerUnderTest worker = A.Fake<MainWorkerUnderTest>();
            DateTime creationDate = new DateTime(1999, 11, 30, 21, 15, 10);
            A.CallTo(() => worker.GetFileCreationTime(A<string>.Ignored)).Returns(creationDate);
            //throw any exception
            A.CallTo(() => worker.DeleteFile(A<string>.Ignored))
                .Throws<ArgumentException>();

            bool stored = BaseTest(worker);
            A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => worker.DeleteFile(A<string>.That.IsEqualTo(_sourceFullPath))).MustHaveHappenedOnceExactly();

            FileItem fileItem = _items[0];
            Assert.AreEqual(true, fileItem.IsError, "Item error");
            Assert.AreEqual(SourceFileName, fileItem.Name);
            Assert.AreEqual(creationDate, fileItem.TimeStamp, "Wrong file time stamp");
            Assert.AreEqual(true, stored, "It must be call for storing collection");
        }

        private bool BaseTest(MainWorkerUnderTest worker)
        {
            FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.All, SourceFolderTest, SourceFileName);
            bool stored = false;
            _items.Clear();
            worker.TargetFolder = TargetFolderTest;
            //FileItem item = new FileItem() { Name = SourceFileName };
            worker.AddWorkItem(args);
            worker.RunHandlingOfWorkingItems(
                (item) => { _items.Add(item); },
                () => { stored = true; },
                (ex, item) => { item.IsError = true; });

            Assert.AreEqual(true, worker.IsRunning);
            //bool stored = stored1;
            //worker.CancellationPending = true;
            WaitExecutionFinished(worker);

            A.CallTo(() => worker.GetFileCreationTime(A<string>.That.IsEqualTo(_sourceFullPath))).MustHaveHappened(1, Times.Exactly); 
            A.CallTo(() => worker.CompressFile(A<string>.That.IsEqualTo(_sourceFullPath))).MustHaveHappenedOnceExactly();
            //A.CallTo(() => worker.MoveFile(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
            //A.CallTo(() => worker.DeleteFile(A<string>.Ignored)).MustHaveHappened();

            Assert.AreEqual(1, _items.Count);
            return stored;
        }

        private static void WaitExecutionFinished(MainWorkerUnderTest worker)
        {
            while (worker.IsRunning)
            {
                Thread.Sleep(100);
            }
        }

        internal class MainWorkerUnderTest : MainWorker
        {
            public override DateTime GetFileCreationTime(string fileFullPath)
            {
                return base.GetFileCreationTime(fileFullPath);
            }

            public override void CompressFile(string sourceFilePath)
            {
                base.CompressFile(sourceFilePath);
            }

            public override void MoveFile(string archiveName, string destFilePath)
            {
                base.MoveFile(archiveName, destFilePath);
            }

            public override void DeleteFile(string sourceFilePath)
            {
                base.DeleteFile(sourceFilePath);
            }
        }
    }
}

/*
 * https://fakeiteasy.readthedocs.io/en/stable/invoking-custom-code/
 */
