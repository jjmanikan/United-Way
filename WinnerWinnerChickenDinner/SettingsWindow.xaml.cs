using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WinnerWinnerChickenDinner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        MainWindow mainWindow = new MainWindow();


        public SettingsWindow()
        {
            InitializeComponent();
            //prizeBoard.ItemsSource = MainWindow.prizeList;
        }

        private void AddPrize(object sender, RoutedEventArgs e)
        {
            ShowDialog("Name new Prize:", "NEW PRIZE");
        }

        public void AddNewPrize(string prize)
        {

            //MainWindow.prizeList.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });
            //SettingsWindow();
            if (prize != "")
            {
                MainWindow.prizeList.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });

                prizeBoard.Items.Clear();

                foreach (var prizeItem in MainWindow.prizeList)
                {
                    prizeBoard.Items.Add(prizeItem);
                }
            }
        }

        public string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                BackColor = System.Drawing.Color.MistyRose,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent
            };

            System.Windows.Forms.Label txtLabel = new System.Windows.Forms.Label() { Left = 50, Top = 20, Text = text };
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 200 };
            System.Windows.Forms.Button button = new System.Windows.Forms.Button() { Text = "Add and Go Back", Left = 50, Width = 110, Top = 70, DialogResult = System.Windows.Forms.DialogResult.OK };
            System.Windows.Forms.Button add = new System.Windows.Forms.Button()
            {
                Text = "+",
                Width = 50,
                Top = 70,
                Left = 160
            };

            add.Font = new System.Drawing.Font(button.Font.FontFamily, 20);
            button.Click += (sender, e) => {
                string t = textBox.Text;
                AddNewPrize(t);
                prompt.Close();
            };
            add.Click += (sender, e) =>
            {
                string t = textBox.Text;
                AddNewPrize(t);
                textBox.Text = "";

            };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(button);
            prompt.Controls.Add(add);
            prompt.Controls.Add(txtLabel);
            prompt.AcceptButton = button;

            return prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK ? textBox.Text : "";

        }

        //save the state when window is closing for the next time it is opened sp all changes are still there
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            string settingsState = "Contestants";
            using (StreamWriter outfile = new StreamWriter(@"C:\unitedway-State.txt"))
            {
                outfile.Write(settingsState);
            }
            string url = filePathBox.Text;
            

            savePrizesToSettings(MainWindow.prizeList);
            Console.WriteLine("Prize List :" +Properties.Settings.Default.PrizeList);
           
            Properties.Settings.Default.ContestantListURL = url;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();

            

            Console.WriteLine("URL: " + Properties.Settings.Default.ContestantListURL);
            System.Windows.Forms.Application.Exit();
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {

            int size = -1;
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {

                Title = "Browse Contenstant Data",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };



            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {


                string file = openFileDialog1.FileName;
                Properties.Settings.Default.ContestantListURL = file;
                try
                {
                    filePathBox.Text = file;
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    mainWindow.ImportContestants();
                    mainWindow.FillPrizeBoard();
                }
                catch (IOException)
                {

                }
            }


        }
        //saves the prizes to a list and adds it to the scoreboard list
        private void savePrizes(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            mainWindow.FillPrizeBoard();
            this.Close();
        }

        void savePrizesToSettings(List<PrizeBoardItem> prizeList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, prizeList);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.PrizeList = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }


        private void prizeBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}

