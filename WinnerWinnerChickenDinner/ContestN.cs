using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{
    [Serializable()]
    public class ContestN
    {
        public ContestN(string contestName, bool multipleWins, string filePath, List<PrizeBoardItem> prizeList, List<Contestant> contestantList)
        {
            ContestName = contestName;
            FilePath = filePath;
            MultipleWins = multipleWins;
            Prizes = prizeList;
            ContestantList = contestantList;
        }

        public string ContestName { get; set; }

        public bool MultipleWins { get; set; }

        public string FilePath { get; set; }

        public List<PrizeBoardItem> Prizes { get; set; }

        public List<Contestant> ContestantList { get; set; }

    }
}
