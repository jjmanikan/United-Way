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
        
        public Ticket(T key, int weight)
        {
            this.key = key;             //full name of contestant
            this.weight = weight;       //number of tickets bought by contestant
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

            foreach(Ticket<T> c in tickets)
            {
                double probability = Convert.ToDouble(c.weight) / Convert.ToDouble(tSum);
            }
 
            return tickets;
        }

        public static T Pick(List<Ticket<T>> tickets)
        {
            //sum of tickets
            int tSum = 0;

            //number of contestants
            int count = tickets.Count();

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
            }
            MainWindow.totalTickets = tSum;
            //random number in total number of tickets
            double winningodd = random.NextDouble() * tSum;
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
