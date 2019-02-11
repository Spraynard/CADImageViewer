using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;


namespace CADImageViewer.Classes.Printing
{
    public class InstallationPrintable
    {
        public string InstallationTitle { get; set; }
        public InstallationDataItem[] InstallationData { get; set; }
        public InstallationNote[] InstallationNotes { get; set; }

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
            currentRow.FontSize = 14;

            currentRow.Cells.Add(new TableCell(
                new Paragraph(
                    new Run(tableHeader))));

            currentRow.Cells[0].ColumnSpan = sourceProperties.Length;

            // Table Item Properties Header
            t.RowGroups[0].Rows.Add(new TableRow());
            currentRow = t.RowGroups[0].Rows[1];

            currentRow.FontWeight = FontWeights.Bold;

            for (i = 0; i < sourceProperties.Length; i++)
            {
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(sourceProperties[i]))));
            }

            return t;
        }

        private Table GetDataTable()
        {
            // Now add our header items
            string[] installationDataProperties = { "Item", "Part", "Description", "Quantity", "Status" };

            Table dataTable = CreateHeaderedTable("Data", InstallationData, installationDataProperties);
            //TableRow currentRow = dataTable.RowGroups[0].Rows[1];

            TableRowGroup rowGroup = dataTable.RowGroups.Last();

            foreach (InstallationDataItem item in InstallationData)
            {
                TableRow row = new TableRow();
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

            return dataTable;
        }

        private Table GetNotesTable()
        {
            string[] notesProperties = { "NoteID", "NoteText" };

            Table notesTable = CreateHeaderedTable("Notes", InstallationNotes, notesProperties);

            TableRowGroup rowGroup = notesTable.RowGroups.Last();

            foreach (InstallationNote item in InstallationNotes)
            {
                TableRow row = new TableRow();

                foreach (string property in notesProperties)
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

            return notesTable;
        }

        public Section GetSection()
        {
            Section installationSection = new Section();

            // Create and insert our section header into the section block space before we insert any other blocks
            Paragraph sectionHeader = new Paragraph();
            sectionHeader.FontSize = 16;
            sectionHeader.FontWeight = FontWeights.Bold;
            sectionHeader.Inlines.Add("Installation: " + InstallationTitle);

            installationSection.Blocks.Add(sectionHeader);

            // Inserting the rest of our data into the installation section
            installationSection.Blocks.Add(GetDataTable());
            installationSection.Blocks.Add(GetNotesTable());

            return installationSection;

        }

        //public Grid GetDataTable()
        //{
        //    // Variables used to place our text elements in grid.
        //    int columnCount = 0;
        //    int rowCount = 0;

        //    Grid dataTable = new Grid();
        //    dataTable.ShowGridLines = false;

        //    // 6 Columns of data
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());
        //    dataTable.ColumnDefinitions.Add(new ColumnDefinition());

        //    // Variable amount of rows, except for the first row, which will be the header row.
        //    dataTable.RowDefinitions.Add(new RowDefinition());

        //    // Now add our header items
        //    string[] InstallationDataProperties = ["Item", "Part", "Description", "Quantity", "Status"];

        //    foreach (string header in InstallationDataProperties)
        //    {
        //        TextBlock headerBlock = new TextBlock();
        //        headerBlock.Text = header;
        //        headerBlock.FontSize = 14;
        //        headerBlock.FontWeight = FontWeights.Bold;

        //        Grid.SetRow(headerBlock, rowCount);
        //        Grid.SetColumn(headerBlock, columnCount);

        //        // Adding our created header to the grid.
        //        dataTable.Children.Add(headerBlock);
        //        columnCount++;
        //    }

        //    // Resetting column count
        //    columnCount = 0;

        //    // incrementing row count to account for the row we just made.
        //    rowCount++;

        //    foreach (InstallationDataItem item in InstallationData)
        //    {

        //        foreach (string property in InstallationDataProperties)
        //        {
        //            // Get our properties value from our InstallationDataItem (i.e. get the InstallationDataItem's "item" property value)
        //            Type installDataType = item.GetType();
        //            System.Reflection.PropertyInfo propertyInfo = installDataType.GetProperty(property);
        //            string propertyValue = (string)propertyInfo.GetValue(item);

        //            TextBlock infoBlock = new TextBlock();
        //            infoBlock.Text = propertyValue;

        //            Grid.SetRow(infoBlock, rowCount);
        //            Grid.SetColumn(infoBlock, columnCount);
        //            columnCount++;
        //        }
        //        columnCount = 0;
        //        rowCount++;
        //    }

        //    return dataTable;
        //}
    }
}
