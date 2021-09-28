using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{
    public class Ticket<T>
    {
        private T key;
        private int weight; 
        private int ticketSum;
       
        private static Random random = new Random();
        
        //key will be full name
        public Ticket(T key, int weight)
        {
            this.key = key;
            this.weight = weight;
        }
        
        
        /// <summary>
        /// get probabilities of each contestant
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public static List<Ticket<T>> GetProbabilities(List<Ticket<T>> tickets)
        {
            int tSum = 0;
            int count = tickets.Count();
            for(int i = 0; i < count; i++)
            {
                tSum += tickets[i].weight;
   
            }

            //Console.WriteLine("\nTotal Number of Tickets: " + tSum);
            foreach(Ticket<T> c in tickets)
            {
                double probability = Convert.ToDouble(c.weight) / Convert.ToDouble(tSum);
                
                Console.WriteLine("Name: " + c.key + " | Chances of winning: " + probability * 100 + "% | Tickets: " + c.weight);
            }
 
            return tickets;
        }

        public static T Pick(List<Ticket<T>> tickets)
        {
            //sum of tickets
            int tSum = 0;

            //number of contestants
            int count = tickets.Count();
            //MainWindow.totalTickets = count.ToString();
            //Console.WriteLine("Ticket Count : " + count);

            //totals number of tickets
            for (int i = 0; i < count; i++)
            {
                //will iterate through total number of tickets
                //each contestant will have a set segment in the total number of tickets
                int ticketSum2;
                tSum += tickets[i].weight;
                
                tickets[i].ticketSum = tSum;

                //verification
                if(i > 0)
                {
                    ticketSum2 = tickets[i - 1].ticketSum; 
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
            Ticket<T> winner = tickets.FirstOrDefault(n => n.ticketSum >= winningodd);
            if (winner == null) throw new Exception("No winner, check algorithm");

            if (!SettingsWindow.allowMultipleWins)
            {
                tickets.Remove(winner);
            }
            else
            {
                winner.weight -= 1;
            }

            foreach (Ticket<T> t in tickets)
            {
                Console.WriteLine("Contestant Name: " + t.key + " Tickets: " + t.weight);
            }

            return winner.key;
        }
    }
}
