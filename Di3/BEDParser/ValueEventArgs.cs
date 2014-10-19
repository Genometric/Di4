using System;

namespace BEDParser
{
    public class ValueEventArgs : EventArgs
    {
        public string Value { get; set; }

        public ValueEventArgs(string value)
        {
            Value = value;
        }
    }
}
