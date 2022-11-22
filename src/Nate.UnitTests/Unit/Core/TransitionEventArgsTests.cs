#region license

/* Nate
 * http://github.com/mmonteleone/nate
 * 
 * Copyright (C) 2009 Michael Monteleone (http://michaelmonteleone.net)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 */

#endregion

using System;
using Nate.Core;
using Xunit;

namespace Nate.Tests.Unit.Core;

public class TransitionEventArgsTests
{
    [Fact]
    public void TransitionEventArgs_NullFrom_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TransitionEventArgs<StubStateModel>(
                new StubStateModel(),
                null,
                new State<StubStateModel>("to"),
                new Trigger("trigger")));
    }

    [Fact]
    public void TransitionEventArgs_NullModel_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TransitionEventArgs<StubStateModel>(
                null,
                new State<StubStateModel>("from"),
                new State<StubStateModel>("to"),
                new Trigger("trigger")));
    }

    [Fact]
    public void TransitionEventArgs_NullTo_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TransitionEventArgs<StubStateModel>(
                new StubStateModel(),
                new State<StubStateModel>("from"),
                null,
                new Trigger("trigger")));
    }

    [Fact]
    public void TransitionEventArgs_NullTrigger_ThrowsNullEx()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TransitionEventArgs<StubStateModel>(
                new StubStateModel(),
                new State<StubStateModel>("from"),
                new State<StubStateModel>("to"),
                null));
    }

    [Fact]
    public void TransitionEventArgs_ValidParms_VerifyAssigns()
    {
        var model = new StubStateModel();
        var from = new State<StubStateModel>("from");
        var to = new State<StubStateModel>("to");
        var trigger = new Trigger("trigger");
        var args = new TransitionEventArgs<StubStateModel>(model, from, to, trigger);

        Assert.Same(from, args.From);
        Assert.Same(to, args.To);
        Assert.Same(model, args.Model);
        Assert.Same(trigger, args.Trigger);
    }
}