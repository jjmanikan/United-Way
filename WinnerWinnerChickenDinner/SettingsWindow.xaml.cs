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


namespace WinnerWinnerChickenDinner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        
        public SettingsWindow()
        {
            InitializeComponent();
            //prizeBoard.ItemsSource = MainWindow.prizeList;
        }

        private void AddPrizeBtn(object sender, RoutedEventArgs e)
        {
            ShowDialog("Name new Prize:", "NEW PRIZE");
        }

        public void AddNewPrize(string prize)
        {
            //MainWindow.prizeList.Add(new PrizeBoardItem { PrizeName = prize, Winner = "" });
            //SettingsWindow();
            prizeBoard.Items.Add(new PrizeBoardItem { PrizeName = prize });
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

        //saves all the added prizes into static list to be populating scoreboard in maind window
        private void savePrizesBtn(object sender, RoutedEventArgs e)
        {
            
        }

        //save the state when window is closing for the next time it is opened sp all changes are still there
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }

}

