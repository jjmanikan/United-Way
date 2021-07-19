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
            this.key = key;
            this.weight = weight;
        }
        
     
        public static List<Ticket<T>> GetProbabilities(List<Ticket<T>> tickets)
        {
            int tSum = 0;
            int count = tickets.Count();
            for(int i = 0; i < count; i++)
            {
                tSum += tickets[i].weight;
   
            }

            Console.WriteLine("\nTotal Number of Tickets: " + tSum);
            foreach(Ticket<T> c in tickets)
            {
                double probability = Convert.ToDouble(c.weight) / Convert.ToDouble(tSum);
                
                Console.WriteLine("Name: " + c.key + " | Chances of winning: " + probability * 100 + "% | Tickets: " + c.weight);
            }
 
            return tickets;
        }

        public static T Pick(List<Ticket<T>> tickets)
        {
            int tSum = 0;
            int count = tickets.Count();

            for (int i = 0; i < count; i++)
            {
                tSum += tickets[i].weight;
                tickets[i].ticketSum = tSum;
            }

            double divSpot = random.NextDouble() * tSum;
            Ticket<T> winner = tickets.FirstOrDefault(n => n.ticketSum >= divSpot);
            if (winner == null) throw new Exception("No winner, check algorithm");
            return winner.key;
        }
    }
}
