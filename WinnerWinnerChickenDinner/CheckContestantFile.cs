using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;

namespace WinnerWinnerChickenDinner
{
    class CheckContestantFile
    {

        List<String> expectedColumnHeadings = new List<string>
        {
            { "Number of tickets purchased" },
            { "Prefix" },
            { "First Name" },
            { "Middle Name" },
            { "Last Name" },
            { "Full Name" },
            { "Phone Number" },
            { "Email Address" }
        };

        Dictionary<String, KeyValuePair<int, int>> foundColumnHeadings = new Dictionary<string, KeyValuePair<int,int>>();

        public bool checkFileHeadings(Range range)
        {
            int numberOfUsedColumns = range.Columns.Count;
            int numberOfUsedRows = range.Rows.Count;

            int headingsRow = 1;
            int currentColumn = 1;
            bool headingsRowFound = false;
            String contentsOfCurrentCell = String.Empty;

            // We want to first figure out where the headings start
            while (headingsRowFound == false && headingsRow <= numberOfUsedRows)
            {
                contentsOfCurrentCell = (range.Cells[headingsRow, currentColumn] as Range).Text;
                foreach (String currentExpectedHeading in expectedColumnHeadings)
                {
                    if (contentsOfCurrentCell.Trim() == currentExpectedHeading) 
                        headingsRowFound = true;
                }
                if (headingsRowFound == false) 
                    headingsRow++;
            }

            // Populate headings dictionary
            while (currentColumn <= numberOfUsedColumns)
            {
                contentsOfCurrentCell = (range.Cells[headingsRow, currentColumn] as Range).Text;
                foreach (String currentExpectedHeading in expectedColumnHeadings)
                {
                    if (contentsOfCurrentCell.Trim() == currentExpectedHeading)
                    {
                        foundColumnHeadings.Add(currentExpectedHeading, new KeyValuePair<int, int>(headingsRow, currentColumn));
                        if (foundColumnHeadings.Count == expectedColumnHeadings.Count)
                            return true; // We found all the columns that we need, so exit
                        break;
                    }
                }
                currentColumn++;
            }
            return false; // Looks like we didn't find all the columns that we were looking for
        }

        public void clearConestantDictionary()
        {
            foundColumnHeadings.Clear();
        }
    }
}
