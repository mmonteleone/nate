using System;

namespace Nate.Core
{
    public class Trigger
    {
        public string Name { get; protected set; }

        public Trigger(string name)
        {
            if (String.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

            Name = name;
        }

        #region comparison overrides

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        #endregion

    }
}
