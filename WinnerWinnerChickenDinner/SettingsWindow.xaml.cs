using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
//using ListViewScrollPosition.Commands;
//using ListViewScrollPosition.Models;

using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace WinnerWinnerChickenDinner
{

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        MainWindow mainWindow = new MainWindow();
        public static bool allowMultipleWins = false;



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
            button.Click += (sender, e) =>
            {
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
                Properties.Settings.Default.ContestantList = file;
                try
                {
                    filePathBox.Text = file;
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    mainWindow.ImportContestants();
                }
                catch (IOException)
                {

                }
            }


        }

        //saves the prizes to a list and adds it to the scoreboard list
        private void savePrizes(object sender, RoutedEventArgs e)
        {

            if ((contestName.Text != "") & (MainWindow.prizeList.Count() > 0))
            {
                errorMessage1.Foreground = Brushes.Green;
                errorMessage1.Content = "Saved!";
                errorMessage2.Content = "";
                contestTitle.Content = contestName.Text;
                mainWindow.FillPrizeBoard();
                this.Close();

                string settingsState = "Contestants";
                using (StreamWriter outfile = new StreamWriter(@"C:\unitedway-State.txt"))
                {
                    outfile.Write(settingsState);
                }


                mainWindow.savePrizesToSettings(MainWindow.prizeList);
                mainWindow.saveContestantsToSettings(MainWindow.ContestantList);
                Console.WriteLine("Prize List :" + Properties.Settings.Default.PrizeList);
                Console.WriteLine("ContestantList: " + Properties.Settings.Default.ContestantList);

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Exit();

                mainWindow.Show();
            }
            else if ((contestName.Text == "") & (MainWindow.prizeList.Count() == 0))
            {
                errorMessage1.Foreground = Brushes.Red;
                errorMessage1.Content = "Could not Save";
                errorMessage2.Content = "Missing Contest Name and Prize Items";
            }
            else if (contestName.Text == "")
            {
                errorMessage1.Foreground = Brushes.Red;
                errorMessage1.Content = "Could not Save";
                errorMessage2.Content = "Missing Contest Name";
            }
            else if (MainWindow.prizeList.Count() == 0)
            {
                errorMessage1.Foreground = Brushes.Red;
                errorMessage1.Content = "Could not Save";
                errorMessage2.Content = "Missing Prize Items";
            }

        }

        


        private void prizeBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllowMultipleWins_Checked(object sender, RoutedEventArgs e)
        {

            allowMultipleWins = true;

            Console.WriteLine("Allow Multiple Wins : " + allowMultipleWins);

        }

        private void AllowMultipleWins_Unchecked(object sender, RoutedEventArgs e)
        {

            allowMultipleWins = false;
            Console.WriteLine("Allow Multiple Wins : " + allowMultipleWins);
        }
    }

}

