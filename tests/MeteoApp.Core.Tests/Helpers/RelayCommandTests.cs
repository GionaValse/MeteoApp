using Xunit;
using MeteoApp.Core.Helpers;
using System;

namespace MeteoApp.Core.Tests.Helpers;

public class RelayCommandTests
{
    [Fact]
    public void Constructor_WhenExecuteIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null));
    }

    [Fact]
    public void Execute_ShouldInvokeAction()
    {
        bool wasInvoked = false;
        var command = new RelayCommand(() => wasInvoked = true);

        command.Execute(null);

        Assert.True(wasInvoked, "Il comando non ha eseguito l'azione passata nel costruttore.");
    }

    [Fact]
    public void CanExecute_WhenNoCanExecuteFuncProvided_ShouldReturnTrue()
    {
        var command = new RelayCommand(() => { });

        bool canExecute = command.CanExecute(null);

        Assert.True(canExecute);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncProvided_ShouldReturnItsResult()
    {
        bool canExecuteCondition = false;

        var command = new RelayCommand(() => { }, () => canExecuteCondition);

        Assert.False(command.CanExecute(null));
        canExecuteCondition = true;
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldFireEvent()
    {
        var command = new RelayCommand(() => { });
        bool eventFired = false;

        command.CanExecuteChanged += (s, e) => eventFired = true;
        command.RaiseCanExecuteChanged();

        Assert.True(eventFired, "L'evento CanExecuteChanged non è stato scatenato.");
    }
}