using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinaleOfCSharpBook
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while (year == 0)
            {
                Console.WriteLine("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("There was an error in your input. Please try again with a valid number.");
                    year = 0;
                }
            }
            while (month == 0)
            {
                Console.WriteLine("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("This is an invalid number, please    select a month from 1 to 12");
                        month = 0;
                    }

                }
                catch (FormatException)
                {
                    Console.WriteLine("There was an error in your input. Please try again with a valid number.");
                    month = 0;
                }
            }
            myStaff = fr.ReadFile();
            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("Enter hours worked for {0}\n", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());

                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month,year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();

        }
    }


    class Staff
    {
        private float hourlyRate;
        private int hWorked;
        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }
        public int HoursWorked {
            get { return hWorked; }

            set
            {
                if (value > 0)
                {
                    hWorked = value;
                }
                else
                {
                    hWorked = 0;
                }
            }

        }

        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return $"NameOfStaff = {NameOfStaff}, hourlyRate = {hourlyRate} , HoursWorked = {hWorked}, BasicPay = {BasicPay}, TotalPay = {TotalPay}.";
        }
    }

    class Manager : Staff
    {
        private const float managerHourlyRate = 50;
        public int Allowance { get; private set; }

        public Manager(string name) : base(name, managerHourlyRate)
        {}

        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;
            if(HoursWorked == 160)
            {
                TotalPay += Allowance;
            }
        }
        public override string ToString()
        {
            return $"NameOfStaff = {NameOfStaff}, ManagerHourlyRate = {managerHourlyRate} , HoursWorked = {HoursWorked}, BasicPay = {BasicPay}, Allowance = {Allowance}, TotalPay = {TotalPay}.";
        }

    }

    class Admin : Staff
    {
        private const float overTimeRate = 15.5f;
        private const float adminHourlyRate = 30f;
        public float Overtime { get; private set; }

        public  Admin(string name) : base(name, adminHourlyRate)
        {}

        public override void CalculatePay()
        {
            base.CalculatePay();
            if(HoursWorked > 160)
            {
                Overtime = overTimeRate * (HoursWorked - 160);
            }
        }
        public override string ToString()
        {
            return $"NameOfStaff = {NameOfStaff}, AdminHourlyRate = {adminHourlyRate} , HoursWorked = {HoursWorked}, BasicPay = {BasicPay}, Overtime = {Overtime}, TotalPay = {TotalPay}.";
        }
    }

    class FileReader
    {
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                while (!sr.EndOfStream)
                {
                    result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (result[1].Equals("Manager"))
                    {
                        Manager manager = new Manager(result[0]);
                        myStaff.Add(manager);
                    }
                    else if (result[1].Equals("Admin"))
                    {
                        Admin admin = new Admin(result[0]);
                        myStaff.Add(admin);
                    }
                }
                sr.Close();
            }
            else
            {
                Console.WriteLine("The file does not exist!");
            }
            return myStaff;
            
        }
    }

    class PaySlip
    {
        private int month;
        private int year;

        enum MonthsOfYear {jAN=1, FEB=2, MAR=3, APR=4, MAY=5, JUN=6, JUL=7, AUG=8, SEP=9, OCT=10, NOV=11, DEC=12  }

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff) {

            string path;

            foreach(Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";

                using(StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("==============================================");
                    sw.WriteLine("Name of Staff: {0}, ", f.NameOfStaff);
                    sw.WriteLine("Hours worked: {0}", f.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:c}", f.BasicPay);
                    if (f.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: {0:c}",((Manager)f).Allowance);
                    }
                    else if (f.GetType() == typeof(Admin))
                    {
                        sw.WriteLine("Overtime:{0:c} ", ((Admin)f).Overtime);
                    }
                    sw.WriteLine("");
                    sw.WriteLine("=======================================");
                    sw.WriteLine("Total Pay: {0:c}", f.TotalPay);
                    sw.WriteLine("========================================");
                    sw.Close();
                }
            }

        }

        public void GenerateSummary(List<Staff> myStaff)
        {


            var result =
                from hours in myStaff
                where hours.HoursWorked < 10
                orderby hours.NameOfStaff ascending
                select new { hours.NameOfStaff, hours.HoursWorked };
            string path = "summary.txt";
            using(StreamWriter sw = new StreamWriter(path))
            {

                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");
                foreach (var f in result) {
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked );
                }
                sw.Close();
            }
        }
        public override string ToString()
        {
            return "month = " + month + " year = " + year; 
        }
    }
}
