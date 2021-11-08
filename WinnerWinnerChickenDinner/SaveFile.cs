using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{
    
    public class SaveFile
    {

        public static void SaveToFile(string name, List<Contestant> contestantList, List<PrizeBoardItem> prizeList, string currentPrize, List<Ticket<string>> tickets, int ticketsSum, double winningTicket, string winnername)
        {
            DateTime date = DateTime.Now;
            System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\..\..\Logs");
            string path = Directory.GetCurrentDirectory() + @"\..\..\Logs\" + name + ".txt";
            int count = 1;
            string[] formatTitles = { "NAME", "DATE CREATED", "PRIZES", "" };

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("==============================================================================================================");
                    sw.WriteLine("NAME               :" + name);
                    sw.WriteLine("DATE CREATED       :" + date);
                    sw.Write("PRIZES             :");
                    foreach (PrizeBoardItem item in prizeList)
                    {
                        if (count == 1)
                        {
                            sw.WriteLine("#" + count + " " + item.PrizeName);
                        }
                        else
                        {
                            sw.WriteLine("                    #" + count + " " + item.PrizeName);
                        }
                        count++;

                    }
                    sw.WriteLine("==============================================================================================================");
                    SaveContestRun(sw, contestantList, tickets, currentPrize, ticketsSum, winningTicket, winnername);
                    sw.Close();
                }

            }
            else
            {
                StreamWriter sw = File.AppendText(path);
                SaveContestRun(sw, contestantList, tickets, currentPrize, ticketsSum, winningTicket,winnername);
                //add info of the round: contestants, their numbers, prize and winning number
                sw.Close();
            }

        }

        public static void SaveContestRun(StreamWriter sw, List<Contestant> contestantList, List<Ticket<string>> tickets,  string currentPrize, int sum, double winningTicket, string winnername)
        {
            int count = 1;
            sw.WriteLine("==============================================================================================================");
            sw.WriteLine("PRIZE:                " + currentPrize);
            sw.WriteLine("TOTAL TICKETS:        " + sum);
            sw.WriteLine("WINNING TICKET:       " + winningTicket);
            sw.WriteLine("WINNER:               " + winnername);
            sw.WriteLine("LIST OF CONTESTANTS:  \n");
            var header = String.Format("{0,10}{1,7}{2,8}{3,15}{4,10}{5,15}{6,40}{7,15}{8,30}",
                "ID", "Tickets", "Prefix", "First Name", "Middle Name", "Last Name", "Full Name", "Phone Number", "Email");
            sw.WriteLine(header);
            sw.WriteLine("--------------------------------------------------------------------------------------------------------------");


            foreach (Contestant contestant in contestantList)
            {
                var eachCont = String.Format("{0,10}{1,7}{2,8}{3,15}{4,10}{5,15}{6,40}{7,15}{8,30}",
                    count,
                    contestant.Tickets,
                    contestant.Prefix,
                    contestant.FirstName,
                    contestant.MiddleName,
                    contestant.LastName,
                    contestant.FullName,
                    contestant.PhoneNumber,
                    contestant.Email);
                count++;
                sw.WriteLine(eachCont);
                eachCont = "";

            }
            count = 1;
            sw.WriteLine("--------------------------------------------------------------------------------------------------------------");
        }
    }
}