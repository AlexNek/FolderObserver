using FakeItEasy;

using NUnit.Framework;

namespace SampleApplication3_2
{
    [TestFixture]
    internal class TestClass
    {
        [Test]
        public void HandlerCallTest()
        {
            //create fake interface implementation
            IMessage fakeInterface = A.Fake<IMessage>();
            MainWindowViewModel viewModel = new MainWindowViewModel(fakeInterface);
            //run command/simulate button press
            viewModel.CommandClickMe.Execute(null);
            //check if function Show was called with exact parameter
            A.CallTo(() => fakeInterface.Show(A<string>.That.IsEqualTo("Hello World!"))).MustHaveHappenedOnceExactly();
        }
    }
}
