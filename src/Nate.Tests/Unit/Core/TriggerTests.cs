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

namespace Nate.Tests.Unit.Core
{
    public class TriggerTests
    {
        [Fact]
        public void Trigger_Equals_TwoInstances_SameName_ReturnTrue()
        {
            var trigger1 = new Trigger("trigger");
            var trigger2 = new Trigger("trigger");
            Assert.True(trigger1.Equals(trigger2));
        }

        [Fact]
        public void Trigger_GetHashCode_VerifySameAsNameHash()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name.GetHashCode(), trigger.GetHashCode());
        }

        [Fact]
        public void Trigger_Name_VerifyAssign()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name, trigger.Name);
        }

        [Fact]
        public void Trigger_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Trigger(null));
        }

        [Fact]
        public void Trigger_ToString_VerifySameAsName()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name, trigger.ToString());
        }
    }
}