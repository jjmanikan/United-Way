using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{
    [Serializable()]
    public class Contest
    {
        public string ContestName{get; set;}
        public List<Contestant> Contestants { get; set; }
    }
}
