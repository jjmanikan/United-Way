using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace WinnerWinnerChickenDinner
{

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window
    {
        //private System.Windows.Forms.OpenFileDialog openFileDialog1;
        MainWindow mainWindow = new MainWindow();
        public static bool allowMultipleWins = false;
        bool backbutton = false;
        CheckContestantFile checkContestantFile = new CheckContestantFile();

        public SettingsWindow()
        {
            InitializeComponent();
            GetSettings();
            LoadChanges();
        }

        private void AddPrize(object sender, RoutedEventArgs e)
        {
            ShowDialog("Name new Prize:", "NEW PRIZE");
        }

        public void AddNewPrize(string prize)
        {
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


        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            // Reset everything if user encountered an error previously
            filePathBox.BorderBrush = Brushes.Black;
            errorMessage1.Content = "";
            errorMessage2.Foreground = Brushes.Black;
            errorMessage2.Content = "Loading file...";

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
                MainWindow.filePath = file;
                try
                {
                    filePathBox.Text = file;
                    string text = File.ReadAllText(file);
                    size = text.Length;

                    Microsoft.Office.Interop.Excel.Application xlApp;
                    Workbook xlWorkBook;
                    Worksheet xlWorkSheet;
                    Range range;

                    xlApp = new Microsoft.Office.Interop.Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(file);
                    xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    if (checkContestantFile.checkFileHeadings(range) == false)
                    {
                        filePathBox.BorderBrush = Brushes.Red;
                        errorMessage1.Foreground = Brushes.Red;
                        errorMessage1.Content = "Hmm, that didn't work";
                        errorMessage2.Content = " -    File has missing or misformatted headings";
                        checkContestantFile.clearConestantDictionary();
                        closeExcelFile(xlApp, xlWorkBook, xlWorkSheet);
                    }
                    else
                    {
                        closeExcelFile(xlApp, xlWorkBook, xlWorkSheet);
                        
                        mainWindow.ImportContestants();
                        DisplayContestants();
                        btnUploadFile.IsEnabled = false;

                        errorMessage1.Foreground = Brushes.Green;
                        errorMessage2.Foreground = Brushes.Green;
                        errorMessage2.Content = "Contestant file loaded successfully";
                    }
                }
                catch (IOException exception)
                {
                    filePathBox.BorderBrush = Brushes.Red;
                    errorMessage1.Foreground = Brushes.Red;
                    errorMessage1.Content = "Failed to open file";
                    errorMessage2.Content = " -    File is likely missing or open somewhere else";
                }
            }
        }


        private void closeExcelFile(Microsoft.Office.Interop.Excel.Application xlApp, Workbook xlWorkBook, Worksheet xlWorkSheet)
        {
            //Closes workbook, excel will continue to run in the background if you don't
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }

        //saves the prizes to a list and adds it to the scoreboard list
        private void Save(object sender, RoutedEventArgs e)
        {
            bool validation = false;

            validation = ValidateSettings();
            if (validation)
            {
                contestTitle.Content = contestName.Text;
                mainWindow.FillPrizeBoard();
                contestantsListView.ItemsSource = null;
                MainWindow.contestTitle = contestName.Text;
                backbutton = true;
                this.Close();

                mainWindow.savePrizesToSettings(MainWindow.prizeList);
                mainWindow.saveContestantsToSettings(MainWindow.ContestantList);

                Properties.Settings.Default.ContestName = contestName.Text;
                Properties.Settings.Default.FilePath = filePathBox.Text;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();

                System.Windows.Forms.Application.Exit();

                mainWindow.Show();
            }

        }

        public bool ValidateSettings()
        {
            bool validation = false;
            contestName.BorderBrush = System.Windows.Media.Brushes.Gray;
            prizeBoard.BorderBrush = System.Windows.Media.Brushes.Gray;
            filePathBox.BorderBrush = System.Windows.Media.Brushes.Black;

            if ((contestName.Text != "") & (MainWindow.prizeList.Count() > 0) & (filePathBox.Text != "Choose File to Upload"))
            {
                errorMessage1.Foreground = Brushes.Green;
                errorMessage1.Content = "Saved!";
                errorMessage2.Content = "";
                validation = true;
                return validation;
            }
            else
            {
                errorMessage1.Foreground = Brushes.Red;
                errorMessage1.Content = "Could not Save";
                errorMessage2.Content = " -    Please fill out the Required Fields";

                if (contestName.Text == "")
                {
                    contestName.BorderBrush = System.Windows.Media.Brushes.Red;
                }

                if (MainWindow.prizeList.Count() == 0)
                {
                    prizeBoard.BorderBrush = System.Windows.Media.Brushes.Red;
                }

                if (filePathBox.Text == "Choose File to Upload")
                {
                    filePathBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }

                    return validation;
            }
        }

        public void LoadChanges()
        {
            if (MainWindow.ContestantList != null)
            {
                DisplayContestants();
                //filePathBox.Text = "File currently loaded";
            }
            if (MainWindow.prizeList != null)
            {
                foreach (var prizeItem in MainWindow.prizeList)
                {
                    prizeBoard.Items.Add(prizeItem);
                }
            }
        }



        public void GetSettings()
        {
            allowMultipleWins = Properties.Settings.Default.MultipleWins;
            contestName.Text = Properties.Settings.Default.ContestName;
            contestTitle.Content = Properties.Settings.Default.ContestName;
            
            if (Properties.Settings.Default.FilePath == "")
            {
                filePathBox.Text = "Choose File to Upload";
                btnUploadFile.IsEnabled = true;
            }
            else
            {
                filePathBox.Text = Properties.Settings.Default.FilePath;
                btnUploadFile.IsEnabled = false;
            }

            if (allowMultipleWins)
            {
                AllowMultipleWins.IsChecked = true;

            }
            else
            {
                AllowMultipleWins.IsChecked = false;

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
                allowMultipleWins = true;
                Properties.Settings.Default.MultipleWins = true;
                Console.WriteLine("Allow Multiple Wins : " + allowMultipleWins);
            Properties.Settings.Default.Save();
        }





        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            allowMultipleWins = false;
            Properties.Settings.Default.MultipleWins = allowMultipleWins;
            Console.WriteLine("Allow Multiple Wins : " + allowMultipleWins);
            Properties.Settings.Default.Save();

        }

        public void DisplayContestants()
        {
            foreach (Contestant contestant in MainWindow.ContestantList)
            {
                this.contestantsListView.Items.Add(new Contestant
                {
                    Tickets = contestant.Tickets,
                    Prefix = contestant.Prefix,
                    FirstName = contestant.FirstName,
                    MiddleName = contestant.MiddleName,
                    LastName = contestant.LastName,
                    FullName = contestant.FullName,
                    PhoneNumber = contestant.PhoneNumber,
                    Email = contestant.Email
                });
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.prizeList.RemoveAt(prizeBoard.SelectedIndex);
            prizeBoard.Items.RemoveAt(prizeBoard.SelectedIndex);
        }

        public string loadPath()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.FilePath)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (string)bf.Deserialize(ms);
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(backbutton == false)
            {
                this.mainWindow.Close();
            }
        }

        private void BtnInfo(object sender, RoutedEventArgs e)
        {
            String htmlPath = Directory.GetCurrentDirectory() + @"\..\..\Assets\helpPage.html";
            Process.Start(htmlPath);
        }
    }

}

