// See https://aka.ms/new-console-template for more information

using Nate;

var bartender = new Bartender(Console.ReadLine, Console.WriteLine);
while (true)
{
    bartender.AskForDrink();
}

public class Bartender 
{
    private readonly Func<string?> _request;
    private readonly Action<string> _response;

    private readonly IFluentStateMachine<StubStateModel> orderProcessor = FluentStateMachine<StubStateModel>.Describe()
        .State("idle")
        .TransitionsTo("ask").On("greet")
        .Initiates()
        .State("ask").AfterEntry(e=>Console.WriteLine("What would you like to drink?"))
        .State("complete")
        .Compile();
    
    public Bartender(Func<string?> inputFunc, Action<string> outputAction)
    {
        _request = inputFunc;
        _response = outputAction;
    }

    public void AskForDrink()
    {
        var model = new StubStateModel() { CurrentState = orderProcessor.InitialState };
        orderProcessor.Trigger("greet",model);
        while (model.CurrentState != orderProcessor.StateNamed("complete"))
        {
            var feedback = _request() ?? String.Empty;
            orderProcessor.Trigger(feedback, model);
        }
    }

}

internal class StubStateModel : IStateModel
{
    public object CurrentState { get; set; }
}