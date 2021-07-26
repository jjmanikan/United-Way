using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinnerWinnerChickenDinner
{

    [Serializable()]
    public class Contestant
    {
   

        public string Tickets { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        //private string tickets;
        //private string prefix;
        //private string firstname;
        //private string middlename;
        //private string lastname;
        //private string fullname;
        //private string phonenumber;
        //private string email;

        //public Contestant()
        //{
        //}

        //public Contestant(string tickets, string prefix, string firstname, string middlename, string lastname, string fullname, string phonenumber, string email)
        //{
        //    this.tickets = tickets;
        //    this.prefix = prefix;
        //    this.firstname = firstname;
        //    this.middlename = middlename;
        //    this.lastname = lastname;
        //    this.fullname = fullname;
        //}

        //public string Tickets
        //{
        //    get { return tickets; }
        //    set { tickets = value; }
        //}
        
        //public string Prefix
        //{
        //    get { return prefix; }
        //    set { prefix = value; }

        //}

        //public string FirstName
        //{
        //    get { return firstname; }
        //    set { firstname = value; }

        //}

        //public string MiddleName
        //{
        //    get { return middlename; }
        //    set { middlename = value; }

        //}
        //public string LastName
        //{
        //    get { return lastname; }
        //    set { lastname = value; }

        //}

        //public string FullName
        //{
        //    get { return fullname; }
        //    set { fullname = value; }

        //}

        //public string PhoneNumber
        //{
        //    get { return phonenumber; }
        //    set { phonenumber = value; }

        //}

        //public string Email
        //{
        //    get { return email; }
        //    set { email = value; }

        //}
    }
}
