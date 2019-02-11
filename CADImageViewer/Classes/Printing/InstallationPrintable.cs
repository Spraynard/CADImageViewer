using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Grid GetDataTable()
        {
            // Variables used to place our text elements in grid.
            int columnCount = 0;
            int rowCount = 0;

            Grid dataTable = new Grid();
            dataTable.ShowGridLines = false;

            // 6 Columns of data
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());
            dataTable.ColumnDefinitions.Add(new ColumnDefinition());

            // Variable amount of rows, except for the first row, which will be the header row.
            dataTable.RowDefinitions.Add(new RowDefinition());

            // Now add our header items
            string[] InstallationDataProperties = ["Item", "Part", "Description", "Quantity", "Status"];

            foreach (string header in InstallationDataProperties)
            {
                TextBlock headerBlock = new TextBlock();
                headerBlock.Text = header;
                headerBlock.FontSize = 14;
                headerBlock.FontWeight = FontWeights.Bold;

                Grid.SetRow(headerBlock, rowCount);
                Grid.SetColumn(headerBlock, columnCount);

                // Adding our created header to the grid.
                dataTable.Children.Add(headerBlock);
                columnCount++;
            }

            // Resetting column count
            columnCount = 0;

            // incrementing row count to account for the row we just made.
            rowCount++;

            foreach (InstallationDataItem item in InstallationData)
            {

                foreach (string property in InstallationDataProperties)
                {
                    // Get our properties value from our InstallationDataItem (i.e. get the InstallationDataItem's "item" property value)
                    Type installDataType = item.GetType();
                    System.Reflection.PropertyInfo propertyInfo = installDataType.GetProperty(property);
                    string propertyValue = (string)propertyInfo.GetValue(item);

                    TextBlock infoBlock = new TextBlock();
                    infoBlock.Text = propertyValue;

                    Grid.SetRow(infoBlock, rowCount);
                    Grid.SetColumn(infoBlock, columnCount);
                    columnCount++;
                }
                columnCount = 0;
                rowCount++;
            }

            return dataTable;
        }
    }
