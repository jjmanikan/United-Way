/*
 * FILE:            MainWindow.xaml.cs
 * DESCRIPTION:     This file contains the functionality for the Main Contest Window which is invoked once the user clicks on "Save and Continue"
 *                      in the Settings Window.
 */



using Ganss.Excel;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace WinnerWinnerChickenDinner
{



    public partial class MainWindow : System.Windows.Window
    {


        //string output = "{0,-20}\t{1,-40}";
        // int prizecount = 0;
        public static string filePath = "";
        //int initializecount = 0;
        public static int totalTickets;
        public static string currentPrize = "";
        public static double winningTicket;

        public static List<PrizeBoardItem> prizeList = new List<PrizeBoardItem>();
        private Random _rnd = new Random(DateTime.UtcNow.Millisecond);
        public static List<Contestant> ContestantList = new List<Contestant>();
        public static List<Ticket<string>> TicketsList = new List<Ticket<string>>();
        public static List<ContestN> ContestList = new List<ContestN>();
        public static string contestTitle = "";


        public MainWindow()
        {
            Console.WriteLine("Total Tickets after ini " + totalTickets);
            InitializeComponent();

            Properties.Settings.Default.Upgrade();
            //Properties.Settings.Default.Save(); - keeping for testing
            try
            {
                //populate contestant list from the saved settings
                ContestantList = loadContestants();
            }
            catch (SerializationException e)
            {
                Console.WriteLine(" Contestant Exception: " + e.Message);
            }

            try
            {
                //populate prize list from saved settings
                prizeList = loadPrizes();
                FillPrizeBoard();

            }
            catch (SerializationException e)
            {
                Console.WriteLine("Prize Exception: " + e.Message);
            }



            TicketsList.Clear();

            //skip first line since its the header
            //for every contestant, retrieve full name and the number of tickets and add to a list that is used for determining the winner
            foreach (Contestant c in ContestantList)
            {
                Ticket<string> contestant = new Ticket<string>(c.FullName, Int32.Parse(c.Tickets));
                TicketsList.Add(contestant);
            }

            Console.WriteLine("Current total tickets after getting settings in main " + MainWindow.totalTickets);
        }

        private List<ContestN> loadContests()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.ContestList)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<ContestN>)bf.Deserialize(ms);
            }
        }

        //retrieve the prizes from saved property settings
        List<PrizeBoardItem> loadPrizes()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.PrizeList)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<PrizeBoardItem>)bf.Deserialize(ms);
            }

        }



        //retrieve the list of current contestants from saved property settings
        List<Contestant> loadContestants()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.ContestantList)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<Contestant>)bf.Deserialize(ms);
            }
        }

        //saves current prizes (with winner if it has been chosen) to property settings so that it can be retreived next time this window is opened
        public void savePrizesToSettings(List<PrizeBoardItem> prizeList)
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

        //saves current contestant list to property settings so that it can be retreived next time this window is opened
        public void saveContestantsToSettings(List<Contestant> contestantList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, contestantList);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.ContestantList = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        public void updateContestToSettings(List<ContestN> contestList)
        {

        }

        public void saveContestToSettings(List<ContestN> contestList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, contestList);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.ContestList = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Accesses the excel file chosen by the user and walks through each cell populating each contestant information
        /// </summary>
        public void ImportContestants()
        {
            Console.WriteLine("Importing Contestants");
            Microsoft.Office.Interop.Excel.Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Range range;

            int rCnt;   //row at which the first contestant is
            int rw = 0;
            int cl = 0;

            //string fullname;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            //TODO: Change to dynamic asset folder
            xlWorkBook = xlApp.Workbooks.Open(filePath);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;

            // until final row is reached, walk through each column of each row, populating contestants' info
            for (rCnt = 3; rCnt <= rw; rCnt++)
            {

                Contestant contestant = new Contestant();

                // try/catch block for each value in case the cell is empty. Otherwise, add value to the contestant info
                try
                {
                    contestant.Tickets = (string)(range.Cells[rCnt, 1] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.Tickets = " ";
                }

                try
                {
                    contestant.Prefix = (string)(range.Cells[rCnt, 2] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.Prefix = " ";
                }

                try
                {
                    contestant.FirstName = (string)(range.Cells[rCnt, 3] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.FirstName = " ";
                }

                try
                {
                    contestant.MiddleName = (string)(range.Cells[rCnt, 4] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.MiddleName = " ";
                }

                contestant.LastName = (string)(range.Cells[rCnt, 5] as Range).Value2.ToString();

                try
                {
                    contestant.FullName = (string)(range.Cells[rCnt, 6] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.FullName = " ";
                }

                try
                {
                    contestant.PhoneNumber = (string)(range.Cells[rCnt, 7] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.PhoneNumber = " ";
                }
                try
                {
                    contestant.Email = (string)(range.Cells[rCnt, 8] as Range).Value2.ToString();
                }
                catch (RuntimeBinderException e)
                {
                    contestant.Email = " ";
                }

                // add contestant to the list
                ContestantList.Add(contestant);

            }


            Console.WriteLine("Count is : " + ContestantList.Count);


            //Closes workbook, excel will continue to run in the background if you don't
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);


            //grab each contestant's full name and tickets purcharsed from the original complete list and add it to list used for determining the winner
            foreach (Contestant c in ContestantList)
            {
                Ticket<string> contestant = new Ticket<string>(c.FullName, Int32.Parse(c.Tickets));
                TicketsList.Add(contestant);
                Console.WriteLine(c.FullName);
            }

        }



        /// <summary>
        /// fill prize board (UI component) with prizes added by user in Settings
        /// </summary>
        public void FillPrizeBoard()
        {
            this.lst_PrizeBoard.ItemsSource = prizeList;
        }

        /// <summary>
        /// When the user clicks on "Roll" button, the functions checks for whether the prize has been selected and invokes the algorithm that 
        /// determines who from the TicketList will be the winner. Once the winner has been chosen, the result is logged into text file. 
        /// Additiionally, if the user has selected to not allow for multiple wins, the winner is removed from the current contestant list.
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //error check that
            if (lst_PrizeBoard.SelectedItem == null)
            {
                errorMain.Content = "Please select a Prize";
            }
            else
            {
                PrizeBoardItem selectedPrize = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
                if (selectedPrize.Winner == "")
                {
                    errorMain.Content = "";
                    try
                    {
                        // determines how many rolls there will be
                        int rollCount = 50 + _rnd.Next(ContestantList.Count);

                        int index = 0;
                        Random randomname = new Random();

                        //rolling effect
                        for (int i = 0; i < rollCount; i++)
                        {
                            // get random index in list of contestant
                            //index = i % UpdatedList.Count;
                            index = randomname.Next(0, ContestantList.Count);

                            txt_WheelName.Text = ContestantList[index].FullName;

                            //TODO: Change to make more bearable  with larger contestant lists
                            //delay that gets longer and longer on each roll
                            var delay = 250 * i / rollCount;


                            String clickWheelSoundFile = Directory.GetCurrentDirectory() + @"\..\..\Assets\click_wheel.wav";
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(clickWheelSoundFile);

                            player.Play();

                            //wait
                            await Task.Delay(delay);
                        }

                        //display each contestants probability of winning
                        currentPrize = selectedPrize.PrizeName;
                        //final roll for winner, only roll that matters
                        string winnername = Ticket<string>.Pick(TicketsList);
                        txt_WheelName.Text = "Congratulations " + winnername + "!\nYou Won " + currentPrize;


                        Console.WriteLine("Winner: " + winnername);


                        selectedPrize.Winner = winnername;

                        SaveFile.SaveToFile(contestTitle, ContestantList, prizeList, currentPrize, TicketsList, totalTickets, winningTicket, winnername);
                        lst_PrizeBoard.Items.Refresh();

                        String congratsSoundFile = Directory.GetCurrentDirectory() + @"\..\..\Assets\dingding.wav";
                        System.Media.SoundPlayer player2 = new System.Media.SoundPlayer(congratsSoundFile);

                        player2.Play();


                        //remove winner from list after they win
                        if (!SettingsWindow.allowMultipleWins)
                        {
                            ContestantList.RemoveAll(x => x.FullName == winnername);
                            Console.WriteLine("Removing Content " + winnername);
                        }

                        // update the lists to saved settings for future rounds
                        savePrizesToSettings(prizeList);
                        saveContestantsToSettings(ContestantList);

                    }
                    catch (ArgumentOutOfRangeException r)
                    {
                        Console.WriteLine("Exception: " + r.Message);
                    }
                }
                //display an error message if the prize selected to be rolled for already has a winner associated with it
                else
                {
                    errorMain.Content = $"{selectedPrize.Winner} already won {selectedPrize.PrizeName}! Select another prize!";
                }
            }

        }

        /// <summary>
        /// captures which prize the user has selected to be rolled for
        /// </summary>
        private void lst_PrizeBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrizeBoardItem selectedItem = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
        }

        //Button to invoke the Settings window. Upon clicking, the current Main window is closed.
        private void BtnOpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow openSettings = new SettingsWindow();
            openSettings.Owner = null;

            openSettings.Show();
            this.Close();
        }

        //Does not seem to currently get the job done - needs to be fixed
        private void KillApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void BtnInfo(object sender, RoutedEventArgs e)
        {
            String htmlPath = Directory.GetCurrentDirectory() + @"\..\..\Assets\helpPage.html";
            Process.Start(htmlPath);
        }
    }


}
