# Unit tests

Testing is a critical stage of the software development life cycle. We have a lot of different
 [testing types](https://testsigma.com/blog/the-different-software-testing-types-explained) but explain only one - Unit Testing.

One of the most valuable benefits of using Unit Tests for your development is that it may give you positive confidence that your code will work as you have expected.
In the software development process Unit Tests basically test individual parts ( also called as Unit ) 
of code. 
The main objective in unit testing is to isolate a code part and 
validate its to correctness and reliability.

Unit testing frameworks for C#:
- [MSTest](https://www.visualstudio.com/)
- [NUnit](https://www.nunit.org/)
- [XUnit](http://xunit.github.io/)

There is two different approach to write unit tests: unit test first (TDD - Test Driven Development) or
code first.
As a lot of projects don't have test at all then we try to use second approach.
It could be really hard to add a unit test to existing code if nobody thinking about application testability.


If you cannot extract some important parts from the code for testing
then your application is not testable and written in a bad way.

Here is the sample of non testable code:
> Sample 1
````csharp
<Window x:Class="SampleApplication1.MainWindow"
        ...
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <Button Content="Click me" Click="Button_Click"/>
    </Grid>
</Window>
````

````csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
        MessageBox.Show("Hello World!");
}
````

[ sample 1 from wpf-tutorial](https://www.wpf-tutorial.com/xaml/events-in-xaml/)

In some cases you can extract the code but can not test it in unknown environment as code used calls to file or database functions.
In this case you can write unit tests for your own development environment only or replace external functions with special test functions.
We can talk about mocking. 
Mocking means creating a fake version of an external or internal service that can stand in for the real one, helping your tests run more quickly and more reliably. 
Most of mocking frameworks could test interfaces and virtual functions.

Mocking Frameworks for .Net:

- [Moq](https://github.com/Moq/moq4)
- [RhinoMocks](http://www.ayende.com/projects/rhino-mocks.aspx)
- [FakeItEasy](https://fakeiteasy.github.io/)
- [Castle Windsor](http://www.castleproject.org/)

But code from sample 1 you can not start in any test environment as you need to see the window, press the button and see the message box.
At the first step we must split logic from user interface.

> Sample 2
````csharp
<Window x:Class="SampleApplication2.MainWindow"
        ...
        Title="MainWindow" Height="450" Width="800">
    <Window.DataContext>
        <local:MainWindowViewModel/>
    </Window.DataContext>
    <Grid>
        <Button Content="Click me" Command="{Binding CommandClickMe}"/>
    </Grid>
</Window>
````

````csharp
public class MainWindowViewModel
{
    public MainWindowViewModel()
    {
        CommandClickMe = new RelayCommand(ExecCommandClickMe);
    }

    private void ExecCommandClickMe()
    {
        MessageBox.Show("Hello world");
    }

    public ICommand CommandClickMe { get; }
}
````

Sample 2 look better as we can use *MainWindowViewModel* in our tests but there is no sense to see *MessageBox.Show* function in our test. So we need to refactor our sample more. 
There is at least two ways how to "remove" *MessageBox.Show* from the tests.
I can not tell you which way is the best for your project, because the decision depends on different specific project factors.
The first way is very easy and could be recommend if you have a lot of different fixed operations.

> sample 3.1
````csharp
public class MainWindowViewModel
{
...
    private void ExecCommandClickMe()
    {
        MessageBoxShow("Hello World!");
    }

    public virtual void MessageBoxShow(string message)
    {
        MessageBox.Show(message);
    }
...
}
````

It is enough to to wrap the system function to another one.

> It is possible to use *protected virtual void* and create additional test class to prevent creation of undesired public function.
> Another variant with more simple replacement:  
````csharp
    public virtual void ExecCommandClickMe()
    {
        MessageBox.Show("Hello World");
    }

````

Now we will be able to create a test function:

````csharp
    [Test]
    public void HandlerCallTest()
    {
        //create fake class
        MainWindowViewModel fakeClass = A.Fake<MainWindowViewModel>();
        //run command/simulate button press
        fakeClass.CommandClickMe.Execute(null);
        //check if command handler was called
        A.CallTo(() => fakeClass.ExecCommandClickMe()).MustHaveHappenedOnceExactly();
    }
````

> I used NUnit and FakeItEasy for Version I
> I added xUnit project with FluentAssertion for Update I

The second way is complicated but more preferable if you have group of operations for different scenario. 
We need more that one step for refactoring.

1. Create Interface for our operations <br\>
````csharp
public interface IMessage
{
    void Show(string message);
}
````

2. Create interface implementation for WPF application <br\>
````csharp
public class WPFMessage : IMessage
{
    public void Show(string message)
    {
        MessageBox.Show(message);
    }
}
````

3. Use interface calls instead of real calls    
````csharp
public class MainWindowViewModel
{
    private readonly IMessage _message;

    public MainWindowViewModel(IMessage message)
    {
        _message = message;
        CommandClickMe = new RelayCommand(ExecCommandClickMe);
    }

    private void ExecCommandClickMe()
    {
        _message?.Show("Hello World!");
    }

    public ICommand CommandClickMe { get; }
}
````

4. Move view model initialization to code behind  
````csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(new WPFMessage());
        InitializeComponent();
    }
}
````

After all steps we can write the test function
````csharp
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
````
# Extenal resources test

By development process sometimes we want to test external resources like file system or database.
Pay attention that you can do it locally but please don't checkin it as public test.

# nUnit vs xUnit

The main difference is that NUnit creates a new instance of the test class and then runs all of the test methods from the same instance. 
Whereas xUnit.net creates a new instance of the test class for each of the test methods. 
if you use xunit.net, you could be sure that your test methods are completely isolated.

For further reading: (NUnit vs. XUnit vs. MSTest: Comparing Unit Testing Frameworks In C#)[https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/]

# FluentAssertions

Just additional helpful library.
A very extensive set of extension methods that allow you to more naturally specify the expected outcome of a unit tests.

(Fluent Assertions Home)[https://fluentassertions.com/]

Compare NUnit Assertion
````csharp
Assert.AreEqual(true,zipExist)
````
with fluent assertion. I think you can immediatly understood what we want to test.
````csharp
zipExist.Should().BeTrue();
````csharp