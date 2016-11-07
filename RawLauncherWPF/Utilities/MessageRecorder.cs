using System;
using System.Collections.Generic;
using System.Linq;

namespace RawLauncherWPF.Utilities
{
    public class MessageRecorder
    {

        private List<string> Messages;

        public MessageRecorder()
        {
            Messages = new List<string>();
        }

        public void AppandMessage(string message)
        {
            Messages.Add(message);
        }

        public void Save()
        {
            Save(null);
        }

        public void Save(string titel)
        {
            var result = Messages.Aggregate(string.Empty, (current, message) => current + (message + "\r\n\r\n"));
            NotepadHelper.ShowMessage(result, titel);
        }

        public void Flush()
        {
            Messages.Clear();
        }
    }
}
