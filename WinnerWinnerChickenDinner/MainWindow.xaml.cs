using Ganss.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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




namespace WinnerWinnerChickenDinner
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {


        string output = "{0,-20}\t{1,-40}";
        int prizecount = 0;
        public static string filePath = "";
        int initializecount = 0;

        public static List<PrizeBoardItem> prizeList = new List<PrizeBoardItem>();
        private Random _rnd = new Random(DateTime.UtcNow.Millisecond);
        public static List<Contestant> ContestantList = new List<Contestant>();
        //List<Contestant> UpdatedList = new List<Contestant>();
        public static List<Ticket<string>> TicketsList = new List<Ticket<string>>();

        public MainWindow()
        {
            InitializeComponent();

            
            Properties.Settings.Default.Upgrade();
            //Properties.Settings.Default.Save();
            Console.WriteLine("Mainwindow2: " + Properties.Settings.Default.ContestantListURL);
            prizeList = loadPrizes();
            if (Properties.Settings.Default.ContestantListURL != "" || Properties.Settings.Default.ContestantListURL !="Choose a file..." && initializecount == 0)
            {
                FillPrizeBoard();
                ImportContestants();
                if(initializecount == 0)
                {
                    initializecount++;
                }
                Console.WriteLine("Ini Count: " + initializecount);
            }

            
            foreach(PrizeBoardItem p in prizeList)
            {
                Console.WriteLine(p.PrizeName);
            }
            
            
            //testing purposes
            /*ContestantList.Add(new Contestant() { Tickets = 10, Prefix = "", FirstName = "Justine", MiddleName = "Kyle Soriano", LastName = "Manikan", FullName = "Justine Kyle Soriano Manikan", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "Js", MiddleName = "", LastName = "Man", FullName = "Js Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "AAAA", MiddleName = "", LastName = "Man", FullName = "AAAA Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "2211", MiddleName = "", LastName = "Man", FullName = "2211 Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            */



            //UpdateList();

            //testing
            //Console.WriteLine("Hello");
            /*foreach( Contestant contestant in ContestantList)
            {
                Console.WriteLine(contestant.Tickets + contestant.FullName);
            }*/
        }


        List<PrizeBoardItem> loadPrizes()
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.PrizeList)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (List<PrizeBoardItem>)bf.Deserialize(ms);
            }
            
        }

        /// <summary>
        /// import contestants from excel
        /// </summary>
        public void ImportContestants()
        {
            
            Console.WriteLine("Calling this method");
            Microsoft.Office.Interop.Excel.Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Range range;

            int rCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            //absolute path, change when neccessary
            //TODO: Change to dynamic asset folder
            xlWorkBook = xlApp.Workbooks.Open(Properties.Settings.Default.ContestantListURL);
            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;

            for (rCnt = 1; rCnt <= rw; rCnt++)
            {

                Contestant contestant = new Contestant();
                contestant.Tickets = (string)(range.Cells[rCnt, 1] as Range).Value2.ToString();
                contestant.Prefix = (string)(range.Cells[rCnt, 2] as Range).Value2.ToString();
                contestant.FirstName = (string)(range.Cells[rCnt, 3] as Range).Value2.ToString();
                contestant.MiddleName = (string)(range.Cells[rCnt, 4] as Range).Value2.ToString();
                contestant.LastName = (string)(range.Cells[rCnt, 5] as Range).Value2.ToString();
                contestant.FullName = (string)(range.Cells[rCnt, 6] as Range).Value2.ToString();
                contestant.PhoneNumber = (string)(range.Cells[rCnt, 7] as Range).Value2.ToString();
                contestant.Email = (string)(range.Cells[rCnt, 8] as Range).Value2.ToString();
                ContestantList.Add(contestant);
            }


            Console.WriteLine("Count is : " + ContestantList.Count);


            //Closes workbook, excel will continue to run in the background if you don't
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //skip first line since its the header
            foreach (Contestant c in ContestantList.Skip(1))
            {
                Ticket<string> contestant = new Ticket<string>(c.FullName, Int32.Parse(c.Tickets));
                TicketsList.Add(contestant);
            }
        }



        /// <summary>
        /// fill prize board with test prizes
        /// </summary>
        public void FillPrizeBoard()
        {
            this.lst_PrizeBoard.ItemsSource = prizeList;
            //this.lst_PrizeBoard.Items.Clear();


        }

        /// <summary>
        /// Rolls button  click function, roll for winner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("Total Count is :" + ContestantList.Count);
            //error check that
            PrizeBoardItem selectedPrize = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
            if (selectedPrize.Winner == "")
            {
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

                        //TODO: change from absolute path to assets
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\justi\source\repos\WinnerWinnerChickenDinner\WinnerWinnerChickenDinner\Assets\click_wheel.wav");
                        player.Play();

                        //wait
                        await Task.Delay(delay);
                    }

                    //display each contestants probability of winning
                    //Ticket<string>.GetProbabilities(TicketsList);

                    //final roll for winner, only roll that matters
                    string winnername = Ticket<string>.Pick(TicketsList);
                    txt_WheelName.Text = winnername;

                    Console.WriteLine("Winner: " + winnername);

                    //string winnername = UpdatedList[index].FullName;


                    selectedPrize.Winner = winnername;
                    lst_PrizeBoard.Items.Refresh();
                    System.Windows.MessageBox.Show($"and... The winner is... {winnername}");

                    //remove winner from list after they win
                    ContestantList.RemoveAll(x => x.FullName == winnername);

                    /*foreach (Contestant x in UpdatedList)
                    {
                        Console.WriteLine(x.FullName);
                    } */


                }
                catch (ArgumentOutOfRangeException r)
                {
                    Console.WriteLine("Exception: " + r.Message);
                    //throw new ArgumentOutOfRangeException("", r);
                }
            }
            else
            {
                System.Windows.MessageBox.Show($"{selectedPrize.Winner} already won {selectedPrize.PrizeName}! Select another prize!");
            }



        }


        /// <summary>
        /// old way of finding winner
        /// function to update list iterating through each contestants number of tickets
        /// </summary>
        public void UpdateList()
        {
            foreach (Contestant contestant in ContestantList.Skip(1))
            {
                int ts = Int32.Parse(contestant.Tickets);
                for (int i = 0; i < ts; i++)
                {
                    Console.WriteLine(contestant.FullName);
                    //UpdatedList.Add(new Contestant() { Tickets = "1", Prefix = contestant.Prefix, FirstName = contestant.FirstName, MiddleName = contestant.MiddleName, LastName = contestant.LastName, FullName = contestant.FullName, PhoneNumber = contestant.PhoneNumber, Email = contestant.Email });
                }
            }
        }

        /// <summary>
        /// selection change function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lst_PrizeBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrizeBoardItem selectedItem = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
            //lst_PrizeBoard.SelectedItems[0].Col1.Text;
            //txt_CurrentPrize.Text = selectedItem.PrizeName;
        }


        /// <summary>
        /// add new prize
        /// </summary>
        /// <param name="prize"></param>
        /// 

        //public void AddNewPrize(string prize)
        //{
        //    lst_PrizeBoard.Items.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });
        //}
        //static


        //public string ShowDialog(string text, string caption)
        //{
        //    Form prompt = new Form()
        //    {
        //        Width = 300,
        //        Height = 150,
        //        BackColor = System.Drawing.Color.MistyRose,
        //        FormBorderStyle = FormBorderStyle.FixedDialog,
        //        Text = caption,
        //        StartPosition = FormStartPosition.CenterParent
        //    };

        //    System.Windows.Forms.Label txtLabel = new System.Windows.Forms.Label() { Left = 50, Top = 20, Text = text};
        //    System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 200 };
        //    System.Windows.Forms.Button button = new System.Windows.Forms.Button() { Text = "Add and Go Back", Left = 50, Width = 110, Top = 70, DialogResult = System.Windows.Forms.DialogResult.OK};
        //    System.Windows.Forms.Button add = new System.Windows.Forms.Button()
        //    {
        //        Text="+",
        //        Width = 50,
        //        Top = 70,
        //        Left = 160
        //    };

        //    add.Font = new System.Drawing.Font(button.Font.FontFamily, 20);
        //    button.Click += (sender, e) => {
        //        string t = textBox.Text;
        //        AddNewPrize(t);
        //        prompt.Close(); };
        //    add.Click += (sender, e) =>
        //    {
        //        string t = textBox.Text;
        //        AddNewPrize(t);
        //        textBox.Text = "";

        //    };
        //    prompt.Controls.Add(textBox);
        //    prompt.Controls.Add(button);
        //    prompt.Controls.Add(add);
        //    prompt.Controls.Add(txtLabel);
        //    prompt.AcceptButton = button;

        //    return prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK ? textBox.Text : "";

        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    ShowDialog("Name new Prize:", "NEW PRIZE");
        //}


        private void BtnOpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow openSettings = new SettingsWindow();
            //SettingsWindow w1 = new SettingsWindow();
            openSettings.Owner = null;
            //openSettings.AppMainWindow = this;

            openSettings.Show();
            this.Close();
        }



        public static void AddList(List<PrizeBoardItem> var_list)
        {

        }

        private void KillApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }


}
