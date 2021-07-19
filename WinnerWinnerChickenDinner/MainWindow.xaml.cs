using Ganss.Excel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        private Random _rnd = new Random(DateTime.UtcNow.Millisecond);
        List<Contestant> ContestantList = new List<Contestant>();
        List<Contestant> UpdatedList = new List<Contestant>();

        public MainWindow()
        {
            InitializeComponent();

            //testing purposes
            /*ContestantList.Add(new Contestant() { Tickets = 10, Prefix = "", FirstName = "Justine", MiddleName = "Kyle Soriano", LastName = "Manikan", FullName = "Justine Kyle Soriano Manikan", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "Js", MiddleName = "", LastName = "Man", FullName = "Js Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "AAAA", MiddleName = "", LastName = "Man", FullName = "AAAA Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            ContestantList.Add(new Contestant() { Tickets = 5, Prefix = "", FirstName = "2211", MiddleName = "", LastName = "Man", FullName = "2211 Man", PhoneNumber = "2113442423", Email = "j@gmail.com" });
            */


            ImportContestants();
            FillPrizeBoard();
            UpdateList();

            

            //testing
            //Console.WriteLine("Hello");
            /*foreach( Contestant contestant in ContestantList)
            {
                Console.WriteLine(contestant.Tickets + contestant.FullName);
            }*/
        }

        /// <summary>
        /// import contestants from excel
        /// </summary>
        private void ImportContestants()
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            Range range;

            int rCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(@"C:\Users\justi\source\repos\WinnerWinnerChickenDinner\WinnerWinnerChickenDinner\iattend output.xlsx");
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

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }


        /// <summary>
        /// fill prize board with test prizes
        /// </summary>
        private void FillPrizeBoard()
        {
            this.lst_PrizeBoard.Items.Add(new PrizeBoardItem { PrizeName = "Cup", Winner = "" });
            this.lst_PrizeBoard.Items.Add(new PrizeBoardItem{ PrizeName = "Cup2", Winner = "" });
            this.lst_PrizeBoard.Items.Add(new PrizeBoardItem{ PrizeName = "Cup3", Winner = "" });
        }

        /// <summary>
        /// buttone lick function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int rollCount = 50 + _rnd.Next(UpdatedList.Count);

                int index = 0;
                Random currentwinner = new Random();

                for (int i = 0; i < rollCount; i++)
                {
                    // get random index in list of contestant
                    //index = i % UpdatedList.Count;
                    index = currentwinner.Next(0, UpdatedList.Count);

                    txt_WheelName.Text = UpdatedList[index].FullName;

                    //delay that gets longer and longer on each roll
                    var delay = 250 * i / rollCount;

                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\justi\source\repos\WinnerWinnerChickenDinner\WinnerWinnerChickenDinner\Assets\click_wheel.wav");
                    player.Play();

                    //wait
                    await Task.Delay(delay);
                }

                string winnername = UpdatedList[index].FullName;
                PrizeBoardItem prizewinner = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
                prizewinner.Winner = winnername;
                lst_PrizeBoard.Items.Refresh();
                System.Windows.MessageBox.Show($"and... The winner is... {UpdatedList[index].FullName}");

                UpdatedList.RemoveAll(x => x.FullName == winnername);
                
                

                foreach (Contestant x in UpdatedList)
                {
                    Console.WriteLine(x.FullName);
                }
                //Simpler randomizer
                /*Random winner = new Random();
                int i = UpdatedList.Count;
                
                int prizewinner = winner.Next(0, i);


                string winnername = UpdatedList[prizewinner].FullName;

                PrizeBoardItem currentwinner = (PrizeBoardItem)lst_PrizeBoard.SelectedItems[0];
                currentwinner.Winner = winnername;
                lst_PrizeBoard.Items.Refresh();
                
                Console.WriteLine("List Winner: " + currentwinner.Winner + "won" + currentwinner.PrizeName);
                
                //txt_FirstPlace.Text = winnername;

                Console.WriteLine("Winner:" + winnername);

                UpdatedList.RemoveAll(x => x.FullName == winnername);

                */
            }
            catch (ArgumentOutOfRangeException r)
            {
                Console.WriteLine("Exception: " + r.Message);
                //throw new ArgumentOutOfRangeException("", r);
            }
           
            
        }


        /// <summary>
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
                    UpdatedList.Add(new Contestant() { Tickets = "1", Prefix = contestant.Prefix, FirstName = contestant.FirstName, MiddleName = contestant.MiddleName, LastName = contestant.LastName, FullName = contestant.FullName, PhoneNumber = contestant.PhoneNumber, Email = contestant.Email });

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
            txt_CurrentPrize.Text = selectedItem.PrizeName;
        }


        /// <summary>
        /// add new prize
        /// </summary>
        /// <param name="prize"></param>
        public void AddNewPrize(string prize)
        {
            lst_PrizeBoard.Items.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });
        }
        //static
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

            System.Windows.Forms.Label txtLabel = new System.Windows.Forms.Label() { Left = 50, Top = 20, Text = text};
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 200 };
            System.Windows.Forms.Button button = new System.Windows.Forms.Button() { Text = "Add", Left = 50, Width = 100, Top = 70, DialogResult = System.Windows.Forms.DialogResult.OK};
            System.Windows.Forms.Button add = new System.Windows.Forms.Button()
            {
                Image = System.Drawing.Image.FromFile(@"C:\Users\justi\source\repos\WinnerWinnerChickenDinner\WinnerWinnerChickenDinner\Assets\plus.jpg"),
                Width = 50,
                Top = 70,
                Left = 150
            };
            button.Click += (sender, e) => { prompt.Close(); };
            add.Click += (sender, e) =>
            {
                string t = textBox.Text;
                AddNewPrize(t);
                
            };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(button);
            prompt.Controls.Add(add);
            prompt.Controls.Add(txtLabel);
            prompt.AcceptButton = button;

            return prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK ? textBox.Text : "";

        }

       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShowDialog("New Prize", "Name new Prize:");
        }
    }

   
}
