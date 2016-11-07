using System;
using System.Collections.Generic;
using System.Linq;

namespace RawLauncherWPF.Utilities
{
    public class MessageRecorder
    {

        private readonly List<string> _messages;

        public MessageRecorder()
        {
            _messages = new List<string>();
        }

        public void AppandMessage(string message)
        {
            _messages.Add(message);
        }

        public int Count() => _messages.Count;

        public void Save()
        {
            Save(null);
        }

        public void Save(string titel)
        {
            var result = _messages.Aggregate(string.Empty, (current, message) => current + (message + "\r\n\r\n"));
            NotepadHelper.ShowMessage(result, titel);
        }

        public void Flush()
        {
            _messages.Clear();
        }
    }
}
