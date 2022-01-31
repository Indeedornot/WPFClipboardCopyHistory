using System.Collections;
using System.IO;
using System.Windows.Forms;
using ControlzEx.Theming;

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
}
