using System;

namespace WinnerWinnerChickenDinner
{
    [Serializable()]
    public class PrizeBoardItem
    {
        public string PrizeName { get; set; }
        public string Winner { get;  set; }
    }
}