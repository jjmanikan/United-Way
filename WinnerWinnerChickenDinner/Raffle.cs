using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{
    class Raffle
    {
        private int ticketSum;

        private static Random random = new Random();

        public static void Pick(List<Contestant> contestants)
        {
            //sum of tickets
            int tSum = 0;

            //number of contestants
            int count = contestants.Count();
            //MainWindow.totalTickets = count.ToString();
            //Console.WriteLine("Ticket Count : " + count);

            //totals number of tickets
            for (int i = 0; i < count; i++)
            {
                //will iterate through total number of tickets
                //each contestant will have a set segment in the total number of tickets
                int ticketSum2;
                tSum += Int32.Parse(contestants[i].Tickets);

                //contestants[i].ticketSum = tSum;

                //verification
                if (i > 0)
                {
                    //ticketSum2 = contestants[i - 1].ticketSum;
                }
                else
                {
                    ticketSum2 = 0;
                }

                //Console.WriteLine("Name: " + tickets[i].key + " Tickets: " + tickets[i].weight + " Number Range Section: Between " + ticketSum2 + " and " + tickets[i].ticketSum);
            }
            MainWindow.totalTickets = tSum;
            //random number in total number of tickets
            double winningodd = random.NextDouble() * tSum;
            //Console.WriteLine("Winning Number: " + winningodd);
            MainWindow.winningTicket = winningodd;


            //picks closest ticket( by greater than i.e. if WinningOdd is 10 a contestant with a ticketSum of 10 or greater will win and is the closest))
            //Contestant winner = contestants.FirstOrDefault(n => n.ticketSum >= winningodd);
            //if (winner == null) throw new Exception("No winner, check algorithm");

            //if (!SettingsWindow.allowMultipleWins)
            //{
            //    contestants.Remove(winner);
            //}
            //else
            //{
            //    //winner.weight -= 1;
            //}

            

        }
    }
}
