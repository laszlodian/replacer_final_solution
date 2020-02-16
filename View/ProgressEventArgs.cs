using System;

namespace replacer.View
{
    public class ProgressEventArgs : EventArgs
    {
        public string Status { get; private set; }

        private ProgressEventArgs() { }

        public ProgressEventArgs(string status)
        {
            Status = status;
        }
    }
}