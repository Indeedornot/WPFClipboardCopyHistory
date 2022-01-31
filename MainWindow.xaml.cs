using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using ControlzEx.Theming;
using ListView = System.Windows.Controls.ListView;

namespace ClipboardCopyHistory
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            _ = ThemeManager.Current.ChangeTheme(this, "Dark.Steel");

            ClipboardListener clipboard = new();
            HistoryListView.ItemsSource = clipboard.History;

            TextCheckBox.Click += (s, e) => { clipboard.TypeChange("Text"); };
            FileCheckBox.Click += (s, e) => { clipboard.TypeChange("Files"); };
            ImageCheckBox.Click += (s, e) => { clipboard.TypeChange("Image"); };

            RemoveAllButton.Click += (s, e) => { HistoryListView.Items.Clear(); };

            IList lastSelectedItems = null;
            HistoryListView.SelectionChanged += (s, e) => { lastSelectedItems = HistoryListView.SelectedItems; };
            RemoveSelectedButton.Click += (s, e) =>
            {
                if (lastSelectedItems is null) return;
                foreach (object t in lastSelectedItems)
                {
                    if (HistoryListView.Items.Contains(t))
                        HistoryListView.Items.Remove(t);
                }

                lastSelectedItems = null;
            };

            //These are to be worked on
            SaveButton.Click += (s, e) =>
            {
                FolderBrowserDialog openFolder = new();
                if (openFolder.ShowDialog() != System.Windows.Forms.DialogResult.Cancel) return;
            };
            ReadButton.Click += (s, e) =>
            {
                SaveFileDialog saveFileDialog = new();
                if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel) return;
                if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);

                foreach (var item in clipboard.History)
                {
                    //File.AppendText(saveFileDialog.FileName, string.Join(" ", item));
                }

                //txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            };
        }
    }

    /// <summary>
    ///     Calculates the column width required to fill the view in a GridView
    ///     For usage examples, see http://leghumped.com/blog/2009/03/11/wpf-gridview-column-width-calculator/
    /// </summary>
    public class WidthConverter : IValueConverter
    {
        private List<int> hi = new();
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The parent Listview.
        ///     Width="{Binding RelativeSource={RelativeSource FindAncestor,AncestorType={x:Type
        ///     ListView}},Converter={StaticResource WidthConverter}}"
        /// </param>
        /// <param name="type">The type.</param>
        /// <param name="parameter">
        ///     If no parameter is given, the remaining width will be returned.
        ///     If the parameter is an integer acts as MinimumWidth, the remaining with will be returned only if it's greater than
        ///     the parameter
        ///     If the parameter is anything else, it's taken to be a percentage. Eg: 0.3* = 30%, 0.15* = 15%
        /// </param>
        /// <param name="culture">The culture.</param>
        /// <returns>The width, as calculated by the parameter given</returns>
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not ListView listView || parameter is null) return null;
            var gridView = listView.View as GridView;
            var column = gridView.Columns[i];
                var minWidth = 0;
                bool widthIsPercentage = !int.TryParse(tag, out minWidth);
                if (widthIsPercentage)
                {
                    double percentage = double.Parse(parameter.ToString()[..^1]);
                    column.Width = listView.ActualWidth * percentage;
                }

                double totalCol = 0;
                for (var j = 0; i < gridView!.Columns.Count - 1; j++)
                {
                    totalCol += gridView.Columns[j].ActualWidth;
                }

                double remainingWidth = listView.ActualWidth - totalCol;
                if (remainingWidth > minWidth)
                {
                    // fill the remaining width in the ListView
                    column.Width = remainingWidth;
                }

                // fill remaining space with MinWidth
                column.Width = minWidth;
            }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
