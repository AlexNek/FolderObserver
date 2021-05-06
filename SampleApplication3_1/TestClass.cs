using FakeItEasy;

using NUnit.Framework;

namespace SampleApplication3_1
{
    [TestFixture]
    internal class TestClass
    {
        [Test]
        public void MessageBoxCallTest()
        {
            //create fake class
            MainWindowViewModel fakeClass = A.Fake<MainWindowViewModel>();
            //run command/simulate button press
            fakeClass.CommandClickMe.Execute(null);
            //check if command handler was called
            A.CallTo(() => fakeClass.MessageBoxShow(A<string>.That.IsEqualTo("Hello World!"))).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void HandlerCallTest()
        {
            //create fake class
            MainWindowViewModelV2 fakeClass = A.Fake<MainWindowViewModelV2>();
            //run command/simulate button press
            fakeClass.CommandClickMe.Execute(null);
            //check if command handler was called
            A.CallTo(() => fakeClass.ExecCommandClickMe()).MustHaveHappenedOnceExactly();
        }

       
    }
}
