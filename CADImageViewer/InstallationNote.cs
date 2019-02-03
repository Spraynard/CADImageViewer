using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADImageViewer
{
    public class InstallationNote
    {
        private readonly string _noteId;
        private readonly string _noteText;

        public InstallationNote( string id, string noteString )
        {
            _noteId = id;
            _noteText = noteString;
        }

        public string NoteID { get { return _noteId; } }
        public string NoteText { get { return _noteText; } }
    }
}
