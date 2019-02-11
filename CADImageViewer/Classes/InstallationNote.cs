using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADImageViewer
{
    public class InstallationNote
    {
        public string NoteID { get; set; }
        public string NoteText { get; set; }

        public InstallationNote( string id, string noteString )
        {
            NoteID = id;
            NoteText = noteString;
        }
    }
}
