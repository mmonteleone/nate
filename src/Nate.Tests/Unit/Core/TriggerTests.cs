using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using Nate.Core;

namespace Nate.Tests.Unit.Core
{
    public class TriggerTests
    {
        [Fact]
        public void Trigger_NullName_ThrowsNullEx()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Trigger(null));
        }

        [Fact]
        public void Trigger_Name_VerifyAssign()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name, trigger.Name);
        }

        [Fact]
        public void Trigger_ToString_VerifySameAsName()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name, trigger.ToString());
        }

        [Fact]
        public void Trigger_GetHashCode_VerifySameAsNameHash()
        {
            var name = "trigger";
            var trigger = new Trigger(name);
            Assert.Equal(name.GetHashCode(), trigger.GetHashCode());
        }

        [Fact]
        public void Trigger_Equals_TwoInstances_SameName_ReturnTrue()
        {
            var trigger1 = new Trigger("trigger");
            var trigger2 = new Trigger("trigger");
            Assert.True(trigger1.Equals(trigger2));
        }
    }
}
