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

            if (MainWindow.ContestList != null)
            {
                foreach (var c in MainWindow.ContestList)
                {
                    contestCmbx.Items.Add(c.ContestName);

                }
            }


            contestCmbx.Items.Add("{Start New Contest}");
        }

        private void AddPrize(object sender, RoutedEventArgs e)
        {
            string t = prizeBox.Text;
            AddNewPrize(t);
            prizeBox.Text = "Enter Prize Here...";
        }

        public void AddNewPrize(string prize)
        {
            if ((prize != "") && (prize != "Enter Prize Here..."))
            {
                errorMessage2.Content = "";
                errorMessage1.Content = "";
                MainWindow.prizeList.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });

                prizeBoard.Items.Clear();

                foreach (var prizeItem in MainWindow.prizeList)
                {
                    prizeBoard.Items.Add(prizeItem);
                }
            }

            else
            {
                errorMessage2.Content = "Please specify the Prize Name";
                errorMessage1.Content = "";
            }
        }

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            // Reset everything if user encountered an error previously
            filePathBox.BorderBrush = Brushes.Black;
            errorMessage1.Content = "";
            errorMessage2.Content = "";

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

                if (MainWindow.ContestList.Any(contest => contest.ContestName == contestName.Text))
                {
                    //update
                    // MainWindow.ContestList.Where(c => c.ContestName == contestName.Text).
                }
                else
                {
                    ContestN contest = new ContestN(contestName.Text, Properties.Settings.Default.MultipleWins, filePathBox.Text, MainWindow.prizeList, MainWindow.ContestantList);
                    MainWindow.ContestList.Add(contest);

                    mainWindow.saveContestsToSettings(MainWindow.ContestList);



                }

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
                errorMessage2.Content = "Please fill out the Required Fields";

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

        //when the user clicks in the prize textbox, the "placeholder" is removed
        private void RemoveText(object sender, RoutedEventArgs e)
        {
            if (prizeBox.Text == "Enter Prize Here...")
            {
                prizeBox.Text = "";
            }

        }
        
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contestantsListView.Items.Clear();
            prizeBoard.Items.Clear();
          
            //TODO: Make combobox like a text box to replace textbox
            string currentcontest = contestCmbx.SelectedItem.ToString();

            if(MainWindow.ContestList.Any(c => c.ContestName == currentcontest))
            {
              
                ContestN current = MainWindow.ContestList.Find(c => c.ContestName == currentcontest);
                filePathBox.Text = current.FilePath;
                contestTitle.Content = current.ContestName;
                contestName.Text = current.ContestName;

                allowMultipleWins = current.MultipleWins;
                AllowMultipleWins.IsChecked = allowMultipleWins;


                foreach(var c in current.ContestantList)
                {
                    Console.WriteLine(c.FullName);
                }


                MainWindow.ContestantList = current.ContestantList;
                MainWindow.prizeList = current.Prizes;

                LoadChanges();
            }
            else
            {
                //TODO: Change way of dereferencing static list
                MainWindow.ContestantList = new List<Contestant>();
                MainWindow.prizeList = new List<PrizeBoardItem>();

                contestName.Text = "";
                contestTitle.Content = "";

                filePathBox.Text = "";
                btnUploadFile.IsEnabled = true;
            }

            Console.WriteLine(currentcontest);

        }


        private void prizeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(prizeBox.Text))
            {
                prizeBox.Text = "Enter Prize Here...";
            }
        }

        /// <summary>
        /// TODO: Refactor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteContest(object sender, RoutedEventArgs e)
        {
            var contest = MainWindow.ContestList.SingleOrDefault(c => c.ContestName == contestName.Text);

            if(contest != null)
            {
                MainWindow.ContestList.Remove(contest);

                contestCmbx.Items.Remove(contestName.Text);

                //TODO: Put this in a different method, repetitive
                contestName.Text = "Empty";
                contestTitle.Content = "Empty";

                filePathBox.Text = "Choose File to Upload";
                btnUploadFile.IsEnabled = true;

                MainWindow.ContestantList.Clear();
                MainWindow.prizeList.Clear();

                prizeBoard.Items.Clear();
                contestantsListView.Items.Clear();

                Properties.Settings.Default.Reset();
                mainWindow.saveContestsToSettings(MainWindow.ContestList);
            }

            

        }
    }

}

