Nate
====
Tiny little fluent state machine for .NET

Full documentation coming soon
------------------------------

Inspired by [Martin Fowler's DSL Book](http://martinfowler.com/dslwip/Intro.html#TheStateMachineFramework), [Ruby's ActsAsStateMachine gem](http://github.com/rubyist/aasm), and Workflow Foundation's bloat.

* Simple, embeddable
  * Does not ask application to conform to its framework.  Just drops into any class to give it simple state machine abilities. 
  * Fast and efficient - Designed to be instantiated statically.  A given StateMachine is itself stateless, and transforms the state of StateModels passed to it.  
  * Keeps the state where it belongs, on the model itself.  Not serialized elsewhere.
  * Provides all the plumbing necessary: states, transitions, triggers, callbacks, machines
* Fluent API
  * Describe your state machine, it's transitions, triggers, callbacks, etc. in a minimal, readable, english-like C# syntax, all without repeating yourself.
  * Fluent API is implemented with a Nate state machine, itself.
* Flexible
  * Support for multiple identical triggers per state with varying targets, chosen from at runtime via lambda expression guards
  * Support for globally-available transitions which provide a pragmatic way to always allow short-circuits
  * Exposure of Underlying State Machine model allows for more advanced possibilities

Full documentation coming soon
------------------------------

**An example implementing Martin Fowler's Gothic Security DSL Sample, demonstrating a portion of the Fluent API.**  
[http://martinfowler.com/dslwip/Intro.html#TheStateMachineFramework](http://martinfowler.com/dslwip/Intro.html#TheStateMachineFramework)

The state machine is described succinctly and embedded directly within the StateModel, which holds (and probably persists) the current state.  


    public class MissGrantController : IStateModel
    {
        /* state is stored on the model */
        public object CurrentState { get; set; }
        public MissGrantController() { CurrentState = securityControllerStateMachine.InitialState; }

        /* state machine is described once, statically */
        private static IFluentStateMachine<MissGrantController> 
        securityControllerStateMachine = FluentStateMachine<MissGrantController>.Describe()
            .State("idle")
                .AfterEntry(e => {
                    e.Model.UnlockDoor();
                    e.Model.LockPanel(); })
                .TransitionsTo("active").On("doorClosed")
                .Initiates()
            .State("active")
                .TransitionsTo("waitingForLight").On("drawOpened")
                .TransitionsTo("waitingForDraw").On("lightOn")
            .State("waitingForLight")
                .TransitionsTo("unlockedPanel").On("lightOn")
            .State("waitingForDraw")
                .TransitionsTo("unlockedPanel").On("drawOpened")
            .State("unlockedPanel")
                .AfterEntry(e => {
                    e.Model.UnlockPanel();
                    e.Model.LockDoor(); })
                .TransitionsTo("idle").On("panelClosed")
            .GloballyTransitionsTo("idle").On("doorOpened")
            .Compile();

        /* external controls */
        public void DoorClosed() { securityControllerStateMachine.Trigger("doorClosed", this); }
        public void DrawOpened() { securityControllerStateMachine.Trigger("drawOpened", this); }
        public void LightOn() { securityControllerStateMachine.Trigger("lightOn", this); }
        public void DoorOpened() { securityControllerStateMachine.Trigger("doorOpened", this); }
        public void PanelClosed() { securityControllerStateMachine.Trigger("panelClosed", this); }

        /* locking/unlocking actions */
        private void UnlockPanel() { Console.WriteLine("Unlocking Panel"); }
        private void LockPanel() { Console.WriteLine("Locking Panel"); }
        private void LockDoor() { Console.WriteLine("Locking Door"); }
        private void UnlockDoor() { Console.WriteLine("Unlocking Door"); }
    }

Full documentation coming soon
------------------------------

License
-------

The MIT License

Copyright (c) 2009-2011 Michael Monteleone

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
