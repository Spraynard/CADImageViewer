using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CADImageViewer.Classes.Printing
{
    public class InstallationPrintable
    {
        public string InstallationTitle { get; set; }
        public InstallationDataItem[] InstallationData { get; set; }
        public InstallationNote[] InstallationNotes { get; set; }

        // Font sizes for our data output
        private int TableHeaderSize = 25;
        private int TableCellHeaderSize = 18;
        private int TableCellSize = 14;
        private int TableErrorSize = 18;

        public InstallationPrintable(string title, InstallationDataItem[] data, InstallationNote[] notes)
        {
            InstallationTitle = title;
            InstallationData = data;
            InstallationNotes = notes;
        }

        private Table CreateHeaderedTable(string tableHeader, Array source, string[] sourceProperties)
        {
            Table t = new Table();
            int i = 0;

            // Base Styling for Table
            t.CellSpacing = 10;
            t.FontFamily = new FontFamily("Verdana");


            TableColumn headerColumn = new TableColumn();

            for (i = 0; i < sourceProperties.Length; i++)
            {
                t.Columns.Add(new TableColumn());
            }

            // Table Indication Header
            t.RowGroups.Add(new TableRowGroup());
            t.RowGroups[0].Rows.Add(new TableRow());

            // Aliasing current row for better reference
            TableRow currentRow = t.RowGroups[0].Rows[0];

            currentRow.FontWeight = FontWeights.Bold;
            currentRow.FontSize = TableHeaderSize;

            currentRow.Cells.Add(new TableCell(
                new Paragraph(
                    new Run(tableHeader))));

            currentRow.Cells[0].ColumnSpan = sourceProperties.Length;

            // Table Item Properties Header
            t.RowGroups[0].Rows.Add(new TableRow());
            currentRow = t.RowGroups[0].Rows[1];
            currentRow.FontSize = TableCellHeaderSize;
            currentRow.FontWeight = FontWeights.Bold;

            for (i = 0; i < sourceProperties.Length; i++)
            {
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(sourceProperties[i]))));
            }

            return t;
        }

        private void NoInformationIndication( string source, Table t , TableRowGroup rowGroup )
        {
            TableRow tableRow = new TableRow();

            TableCell noInfoCell = new TableCell();
            noInfoCell.RowSpan = t.Columns.Count;
            noInfoCell.FontSize = TableErrorSize;
            noInfoCell.FontWeight = FontWeights.Bold;

            Paragraph noInfoParagraph = new Paragraph();
            noInfoParagraph.Inlines.Add(new Run(String.Format("NO {0} Available", source)));

            noInfoCell.Blocks.Add(noInfoParagraph);

            tableRow.Cells.Add(noInfoCell);

            rowGroup.Rows.Add(tableRow);
        }

        private Table GetDataTable( InstallationDataItem[] installationData )
        {
            // Now add our header items
            string[] installationDataProperties = { "Item", "Part", "Description", "Quantity", "Status" };


            Table dataTable = CreateHeaderedTable("Data", installationData, installationDataProperties);
            //TableRow currentRow = dataTable.RowGroups[0].Rows[1];

            TableRowGroup rowGroup = dataTable.RowGroups.Last();

            if ( installationData.Length > 0 )
            {
                foreach (InstallationDataItem item in installationData)
                {
                    TableRow row = new TableRow();
                    row.FontSize = TableCellSize;
                    foreach (string property in installationDataProperties)
                    {
                        // Get our properties value from our InstallationDataItem (i.e. get the InstallationDataItem's "item" property value)
                        Type installDataType = item.GetType();
                        System.Reflection.PropertyInfo propertyInfo = installDataType.GetProperty(property);
                        string propertyValue = (string)propertyInfo.GetValue(item);

                        TableCell cell = new TableCell();
                        cell.Blocks.Add(new Paragraph(new Run(propertyValue)));

                        row.Cells.Add(cell);
                    }
                    rowGroup.Rows.Add(row);
                }
            }
            else
            {
                NoInformationIndication("Installation Data", dataTable, rowGroup);
            }
            return dataTable;
        }

        private Table GetNotesTable(InstallationNote[] installationNotes)
        {
            string[] notesProperties = { "NoteID", "NoteText" };

            Table notesTable = CreateHeaderedTable("Notes", installationNotes, notesProperties);

            // Specific styling for the notes table requires us to add columns.
            notesTable.Columns.Add(new TableColumn());
            notesTable.Columns.Add(new TableColumn());
            notesTable.Columns.Add(new TableColumn());
            notesTable.Columns.Add(new TableColumn());
            notesTable.Columns.Add(new TableColumn());

            TableRowGroup rowGroup = notesTable.RowGroups.Last();

            if (installationNotes.Length > 0)
            {
                foreach (InstallationNote item in installationNotes)
                {
                    TableRow row = new TableRow();
                    row.FontSize = TableCellSize;
                    foreach (string property in notesProperties)
                    {
                        // Get our properties value from our InstallationDataItem (i.e. get the InstallationDataItem's "item" property value)
                        Type installDataType = item.GetType();
                        System.Reflection.PropertyInfo propertyInfo = installDataType.GetProperty(property);
                        string propertyValue = (string)propertyInfo.GetValue(item);

                        TableCell cell = new TableCell();

                        if ( property == "NoteText" )
                        {
                            cell.ColumnSpan = 5;
                        }

                        cell.Blocks.Add(new Paragraph(new Run(propertyValue)));

                        row.Cells.Add(cell);
                    }
                    rowGroup.Rows.Add(row);
                }
            }
            else
            {
                NoInformationIndication("Notes Information", notesTable, rowGroup);
            }
            

            return notesTable;
        }

        public Section GetSection()
        {
            Section installationSection = new Section();

            // Create and insert our section header into the section block space before we insert any other blocks
            Paragraph sectionHeader = new Paragraph();
            sectionHeader.FontSize = 30;
            sectionHeader.FontFamily = new FontFamily("Courier New");
            sectionHeader.FontWeight = FontWeights.Bold;
            sectionHeader.Inlines.Add("Installation: " + InstallationTitle);

            installationSection.Blocks.Add(sectionHeader);

            // Inserting the rest of our data into the installation section
            installationSection.Blocks.Add(GetDataTable(InstallationData));
            installationSection.Blocks.Add(GetNotesTable(InstallationNotes));

            return installationSection;

        }
    }
}
