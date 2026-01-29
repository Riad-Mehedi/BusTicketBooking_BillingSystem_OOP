using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BusTicketBookingSystem
{
    // ============================================================
                            // VALIDATION 
    // ============================================================
    public static class ValidationHelper
    {
        // Valid formats: 01XXXXXXXXX (11 digits starting with 01)
        // Valid operators: 013, 014, 015, 016, 017, 018, 019
        public static bool IsValidBDMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return false;

            // Remove any spaces or dashes
            mobileNumber = mobileNumber.Replace(" ", "").Replace("-", "");

            // Check if it matches BD mobile pattern
            // Pattern: 01[3-9]XXXXXXXX
            string pattern = @"^01[3-9]\d{8}$";
            return Regex.IsMatch(mobileNumber, pattern);
        }

        // Email validation
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Basic email pattern
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        public static void DisplayMobileNumberError()
        {
            Console.WriteLine("\nInvalid mobile number!");
            Console.WriteLine("Valid format: 01XXXXXXXXX (11 digits)");
            Console.WriteLine("Must start with: 013, 014, 015, 016, 017, 018, or 019");
            Console.WriteLine("Example: 01712345678");
        }

        public static void DisplayEmailError()
        {
            Console.WriteLine("\nInvalid email address!");
            Console.WriteLine("Valid format: example@domain.com");
        }
    }

    // ============================================================
                             // ENUMS
    // ============================================================
    public enum BusType
    {
        Economy,    // 40 seats (10 rows x 4 columns: A, B, C, D)
        Business    // 20 seats (10 rows x 2 columns: A, B)
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid
    }

    // ============================================================
                           // MODEL CLASSES
    // ============================================================

    public class User
    {
        public int UserId { get; private set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public List<Ticket> Tickets { get; private set; }

        public User(int userId, string name, string mobileNumber, string email)
        {
            UserId = userId;
            Name = name;
            MobileNumber = mobileNumber;
            Email = email;
            Tickets = new List<Ticket>();
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"ID: {UserId} | Name: {Name} | Mobile: {MobileNumber} | Email: {Email}");
        }
    }

    public class Bus
    {
        public int BusId { get; private set; }
        public string CoachNumber { get; set; }
        public BusType BusType { get; set; }
        public int TotalSeats { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public char[] ColumnLetters { get; private set; }

        public Bus(int busId, string coachNumber, BusType busType)
        {
            BusId = busId;
            CoachNumber = coachNumber;
            BusType = busType;
            
            // Set rows and columns based on bus type
            if (busType == BusType.Business)
            {
                Rows = 10;   
                Columns = 2;    
                ColumnLetters = new char[] { 'A', 'B' };
                TotalSeats = 20; 
            }
            else // Economy
            {
                Rows = 10;      
                Columns = 4;    
                ColumnLetters = new char[] { 'A', 'B', 'C', 'D' };
                TotalSeats = 40; 
            }
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Bus ID: {BusId} | Coach: {CoachNumber} | Type: {BusType} | Seats: {TotalSeats}");
        }
    }

    public class Schedule
    {
        public int ScheduleId { get; private set; }
        public Bus Bus { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> BookedSeats { get; private set; }
        public List<string> PaidSeats { get; private set; }

        public Schedule(int scheduleId, Bus bus, string departureCity, string arrivalCity,
                       DateTime departureDateTime, decimal ticketPrice)
        {
            ScheduleId = scheduleId;
            Bus = bus;
            DepartureCity = departureCity;
            ArrivalCity = arrivalCity;
            DepartureDateTime = departureDateTime;
            TicketPrice = ticketPrice;
            BookedSeats = new List<string>();
            PaidSeats = new List<string>();
        }

        public bool IsSeatAvailable(string seatNumber)
        {
            return !BookedSeats.Contains(seatNumber);
        }

        public bool IsValidSeat(string seatNumber)
        {
            if (string.IsNullOrEmpty(seatNumber) || seatNumber.Length < 2)
                return false;

            // Extract row number and column letter
            string rowPart = seatNumber.Substring(0, seatNumber.Length - 1);
            char columnPart = seatNumber[seatNumber.Length - 1];

            if (!int.TryParse(rowPart, out int row))
                return false;

            // Check if row is valid
            if (row < 1 || row > Bus.Rows)
                return false;

            // Check if column is valid based on bus type
            if (!Bus.ColumnLetters.Contains(columnPart))
                return false;

            return true;
        }

        public bool BookSeat(string seatNumber)
        {
            seatNumber = seatNumber.ToUpper();

            if (!IsValidSeat(seatNumber))
            {
                Console.WriteLine("Invalid seat number!");
                if (Bus.BusType == BusType.Business)
                {
                    Console.WriteLine("Business bus seats: 1A-10A, 1B-10B");
                }
                else
                {
                    Console.WriteLine("Economy bus seats: 1A-10A, 1B-10B, 1C-10C, 1D-10D");
                }
                return false;
            }

            if (!IsSeatAvailable(seatNumber))
            {
                Console.WriteLine("Seat is already booked. Please choose another seat or try again after 10 minutes.");
                return false;
            }

            BookedSeats.Add(seatNumber);
            return true;
        }

        public void MarkSeatAsPaid(string seatNumber)
        {
            if (!PaidSeats.Contains(seatNumber))
            {
                PaidSeats.Add(seatNumber);
            }
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Schedule ID: {ScheduleId} | From {DepartureCity} to {ArrivalCity} | Date: {DepartureDateTime:yyyy-MM-dd HH:mm} | Price: {TicketPrice} tk");
        }

        public void DisplayDetailedInfo()
        {
            Console.WriteLine("\n--- Schedule Details ---");
            Console.WriteLine($"Schedule ID: {ScheduleId}");
            Console.WriteLine($"Bus ID: {Bus.BusId} | Coach Number: {Bus.CoachNumber} | Type: {Bus.BusType}");
            Console.WriteLine($"From: {DepartureCity} to {ArrivalCity}");
            Console.WriteLine($"Departure Time: {DepartureDateTime:yyyy-MM-dd HH:mm} | Taken: {TicketPrice} tk");
            Console.WriteLine();
            DisplaySeatLayout();
        }

        public void DisplaySeatLayout()
        {
            Console.WriteLine("Seat Layout (X = booked, [ ] = available):");
            Console.WriteLine();

            for (int row = 1; row <= Bus.Rows; row++)
            {
                for (int col = 0; col < Bus.Columns; col++)
                {
                    string seatNumber = $"{row}{Bus.ColumnLetters[col]}";
                    
                    if (PaidSeats.Contains(seatNumber))
                    {
                        // Paid seat - show with X
                        if (Bus.BusType == BusType.Business)
                        {
                            // Business: wider spacing for 2 columns
                            Console.Write($"[X:{seatNumber}]  ");
                        }
                        else
                        {
                            Console.Write($"[X:{seatNumber}] ");
                        }
                    }
                    else if (BookedSeats.Contains(seatNumber))
                    {
                        // Booked but not paid
                        if (Bus.BusType == BusType.Business)
                        {
                            Console.Write($"[.{seatNumber}]  ");
                        }
                        else
                        {
                            Console.Write($"[.{seatNumber}] ");
                        }
                    }
                    else
                    {
                        // Available seat
                        if (Bus.BusType == BusType.Business)
                        {
                            Console.Write($"[.{seatNumber}]  ");
                        }
                        else
                        {
                            Console.Write($"[.{seatNumber}] ");
                        }
                    }
                }
                
                if (row < 10)
                {
                    Console.WriteLine($"  Row {row}");
                }
                else
                {
                    Console.WriteLine($" Row {row}");
                }
            }
        }
    }

    public class Ticket
    {
        public int TicketId { get; private set; }
        public User User { get; set; }
        public Schedule Schedule { get; set; }
        public string SeatNumber { get; set; }
        public DateTime BookingDate { get; private set; }

        public Ticket(int ticketId, User user, Schedule schedule, string seatNumber)
        {
            TicketId = ticketId;
            User = user;
            Schedule = schedule;
            SeatNumber = seatNumber;
            BookingDate = DateTime.Now;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Ticket ID: {TicketId} | Coach No: {Schedule.Bus.CoachNumber} | Journey: {Schedule.DepartureDateTime:yyyy-MM-dd} | Seat: {SeatNumber}");
        }
    }

    public class Invoice
    {
        public int InvoiceId { get; private set; }
        public Ticket Ticket { get; set; }
        public User User { get; set; }
        public decimal Amount { get; set; }
        public DateTime InvoiceDate { get; private set; }
        public PaymentStatus PaymentStatus { get; set; }

        public Invoice(int invoiceId, Ticket ticket, User user, decimal amount)
        {
            InvoiceId = invoiceId;
            Ticket = ticket;
            User = user;
            Amount = amount;
            InvoiceDate = DateTime.Now;
            PaymentStatus = PaymentStatus.Unpaid;
        }

        public void MarkAsPaid()
        {
            PaymentStatus = PaymentStatus.Paid;
            Ticket.Schedule.MarkSeatAsPaid(Ticket.SeatNumber);
            Console.WriteLine($"\nInvoice {InvoiceId} marked as PAID.");
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Invoice ID: {InvoiceId} | Ticket ID: {Ticket.TicketId} | Amount: {Amount} tk | Date: {InvoiceDate:yyyy-MM-dd} | Paid: {(PaymentStatus == PaymentStatus.Paid ? "Yes" : "No")}");
        }
    }

    // ============================================================
                       // DATA STORE 
    // ============================================================
    public static class DataStore
    {
        public static List<User> Users = new List<User>();
        public static List<Bus> Buses = new List<Bus>();
        public static List<Schedule> Schedules = new List<Schedule>();
        public static List<Ticket> Tickets = new List<Ticket>();
        public static List<Invoice> Invoices = new List<Invoice>();

        public static int NextUserId = 1;
        public static int NextBusId = 1;
        public static int NextScheduleId = 1;
        public static int NextTicketId = 1;
        public static int NextInvoiceId = 1;
    }

    // ============================================================
                       // MANAGER CLASSES 
    // ============================================================

    public class UserManager
    {
        public void CreateUser()
        {
            Console.WriteLine("\n--- Create User ---");

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\nName cannot be empty!");
                return;
            }

            // Get and validate mobile number
            string mobile;
            while (true)
            {
                Console.Write("Enter Mobile Number (e.g., 01712345678): ");
                mobile = Console.ReadLine();

                if (ValidationHelper.IsValidBDMobileNumber(mobile))
                {
                    mobile = mobile.Replace(" ", "").Replace("-", "");
                    break;
                }
                else
                {
                    ValidationHelper.DisplayMobileNumberError();
                    Console.Write("\nTry again? (yes/no): ");
                    string retry = Console.ReadLine()?.ToLower();
                    if (retry != "yes" && retry != "y")
                    {
                        return;
                    }
                }
            }

            // Get and validate email
            string email;
            while (true)
            {
                Console.Write("Enter Email (e.g., user@example.com): ");
                email = Console.ReadLine();

                if (ValidationHelper.IsValidEmail(email))
                {
                    break;
                }
                else
                {
                    ValidationHelper.DisplayEmailError();
                    Console.Write("\nTry again? (yes/no): ");
                    string retry = Console.ReadLine()?.ToLower();
                    if (retry != "yes" && retry != "y")
                    {
                        return;
                    }
                }
            }

            // Create user
            User user = new User(DataStore.NextUserId++, name, mobile, email);
            DataStore.Users.Add(user);

            Console.WriteLine($"\nUser created successfully! User ID: {user.UserId}");
        }

        public void ShowAllUsers()
        {
            Console.WriteLine("\n--- Users ---");

            if (DataStore.Users.Count == 0)
            {
                Console.WriteLine("No users available.");
                return;
            }

            foreach (var user in DataStore.Users)
            {
                user.DisplayInfo();
            }
        }

        public User FindUserById(int userId)
        {
            return DataStore.Users.FirstOrDefault(u => u.UserId == userId);
        }
    }

    public class BusManager
    {
        public void CreateBus()
        {
            Console.WriteLine("\n--- Create Bus ---");

            Console.Write("Enter Coach Number: ");
            string coachNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(coachNumber))
            {
                Console.WriteLine("\nCoach number cannot be empty!");
                return;
            }

            Console.WriteLine("Select Bus Type:");
            Console.WriteLine("1. Economy (40 seats - 10 rows x 4 columns)");
            Console.WriteLine("2. Business (20 seats - 10 rows x 2 columns)");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            BusType busType = choice == "2" ? BusType.Business : BusType.Economy;

            Bus bus = new Bus(DataStore.NextBusId++, coachNumber, busType);
            DataStore.Buses.Add(bus);

            Console.WriteLine($"\nBus created successfully! Bus ID: {bus.BusId}");
            Console.WriteLine($"Seat Configuration: {bus.Rows} rows x {bus.Columns} columns");
        }

        public void ShowAllBuses()
        {
            Console.WriteLine("\n--- Buses ---");

            if (DataStore.Buses.Count == 0)
            {
                Console.WriteLine("No buses available.");
                return;
            }

            foreach (var bus in DataStore.Buses)
            {
                bus.DisplayInfo();
            }
        }

        public Bus FindBusById(int busId)
        {
            return DataStore.Buses.FirstOrDefault(b => b.BusId == busId);
        }
    }

    public class ScheduleManager
    {
        public void CreateSchedule()
        {
            Console.WriteLine("\n--- Create Schedule ---");

            if (DataStore.Buses.Count == 0)
            {
                Console.WriteLine("No buses available! Please create a bus first.");
                return;
            }

            Console.WriteLine("\nAvailable Buses:");
            foreach (var bus in DataStore.Buses)
            {
                bus.DisplayInfo();
            }

            Console.Write("\nEnter Bus ID: ");
            if (!int.TryParse(Console.ReadLine(), out int busId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            Bus selectedBus = DataStore.Buses.FirstOrDefault(b => b.BusId == busId);
            if (selectedBus == null)
            {
                Console.WriteLine("Invalid Bus ID!");
                return;
            }

            Console.Write("Enter Departure City: ");
            string depCity = Console.ReadLine();

            Console.Write("Enter Arrival City: ");
            string arrCity = Console.ReadLine();

            Console.Write("Enter Departure Date (yyyy-MM-dd HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime depDateTime))
            {
                Console.WriteLine("Invalid date format!");
                return;
            }

            Console.Write("Enter Ticket Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price!");
                return;
            }

            Schedule schedule = new Schedule(DataStore.NextScheduleId++, selectedBus, depCity, arrCity, depDateTime, price);
            DataStore.Schedules.Add(schedule);

            Console.WriteLine($"\nSchedule created successfully! Schedule ID: {schedule.ScheduleId}");
        }

        public void ShowAllSchedules()
        {
            Console.WriteLine("\n--- Schedules ---");

            if (DataStore.Schedules.Count == 0)
            {
                Console.WriteLine("No schedules available.");
                return;
            }

            foreach (var schedule in DataStore.Schedules)
            {
                schedule.DisplayInfo();
            }
        }

        public void ShowScheduleDetails()
        {
            Console.Write("\nEnter Schedule ID to view details: ");
            if (!int.TryParse(Console.ReadLine(), out int scheduleId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            Schedule schedule = DataStore.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);

            if (schedule == null)
            {
                Console.WriteLine("Schedule not found!");
                return;
            }

            schedule.DisplayDetailedInfo();
        }
    }

    public class BookingManager
    {
        public void BookTicket()
        {
            Console.WriteLine("\n--- Book Ticket ---");

            if (DataStore.Users.Count == 0)
            {
                Console.WriteLine("No users available! Please create a user first.");
                return;
            }

            Console.Write("Enter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            User selectedUser = DataStore.Users.FirstOrDefault(u => u.UserId == userId);
            if (selectedUser == null)
            {
                Console.WriteLine("Invalid User ID!");
                return;
            }

            if (DataStore.Schedules.Count == 0)
            {
                Console.WriteLine("No schedules available! Please create a schedule first.");
                return;
            }

            Console.WriteLine("\nAvailable Schedules:");
            foreach (var schedule in DataStore.Schedules)
            {
                schedule.DisplayInfo();
            }

            Console.Write("\nEnter Schedule ID: ");
            if (!int.TryParse(Console.ReadLine(), out int scheduleId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            Schedule selectedSchedule = DataStore.Schedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
            if (selectedSchedule == null)
            {
                Console.WriteLine("Invalid Schedule ID!");
                return;
            }

            Console.WriteLine();
            selectedSchedule.DisplaySeatLayout();

            // Show valid seat range based on bus type
            if (selectedSchedule.Bus.BusType == BusType.Business)
            {
                Console.WriteLine("\nValid seats for Business: 1A-10A, 1B-10B");
            }
            else
            {
                Console.WriteLine("\nValid seats for Economy: 1A-10A, 1B-10B, 1C-10C, 1D-10D");
            }

            Console.Write($"\nEnter Seat Number (e.g., 1A, 2B): ");
            string seatNumber = Console.ReadLine()?.ToUpper();

            if (!selectedSchedule.BookSeat(seatNumber))
            {
                return;
            }

            Ticket ticket = new Ticket(DataStore.NextTicketId++, selectedUser, selectedSchedule, seatNumber);
            DataStore.Tickets.Add(ticket);
            selectedUser.Tickets.Add(ticket);

            Invoice invoice = new Invoice(DataStore.NextInvoiceId++, ticket, selectedUser, selectedSchedule.TicketPrice);
            DataStore.Invoices.Add(invoice);

            Console.WriteLine("\nTicket booked successfully!");
            Console.WriteLine("\n--- Ticket ---");
            ticket.DisplayInfo();
            Console.WriteLine("\n--- Invoice ---");
            invoice.DisplayInfo();
        }

        public void ShowUserTickets()
        {
            Console.Write("\nEnter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            User user = DataStore.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            var paidInvoices = DataStore.Invoices
                .Where(inv => inv.User.UserId == userId && inv.PaymentStatus == PaymentStatus.Paid)
                .ToList();

            Console.WriteLine($"\n--- Tickets ---");

            if (paidInvoices.Count == 0)
            {
                Console.WriteLine("No paid tickets found for this user.");
                return;
            }

            foreach (var invoice in paidInvoices)
            {
                invoice.Ticket.DisplayInfo();
            }
        }
    }

    public class InvoiceManager
    {
        public void ShowUserInvoices()
        {
            Console.Write("\nEnter user ID: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            User user = DataStore.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            var userInvoices = DataStore.Invoices.Where(inv => inv.User.UserId == userId).ToList();

            Console.WriteLine($"\n--- Invoices ---");

            if (userInvoices.Count == 0)
            {
                Console.WriteLine("No invoices found for this user.");
                return;
            }

            foreach (var invoice in userInvoices)
            {
                invoice.DisplayInfo();
            }
        }

        public void PayInvoice()
        {
            Console.Write("\nEnter Invoice ID: ");
            if (!int.TryParse(Console.ReadLine(), out int invoiceId))
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            Invoice invoice = DataStore.Invoices.FirstOrDefault(inv => inv.InvoiceId == invoiceId);

            if (invoice == null)
            {
                Console.WriteLine("Invoice not found!");
                return;
            }

            if (invoice.PaymentStatus == PaymentStatus.Paid)
            {
                Console.WriteLine("This invoice is already paid!");
                return;
            }

            Console.WriteLine("\n--- Invoice ---");
            invoice.DisplayInfo();

            Console.Write("\nConfirm payment? (yes/no): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm == "yes" || confirm == "y")
            {
                invoice.MarkAsPaid();
            }
            else
            {
                Console.WriteLine("Payment cancelled.");
            }
        }
    }

    // ============================================================
                             // MAIN PROGRAM
    // ============================================================
    class Program
    {
        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            BusManager busManager = new BusManager();
            ScheduleManager scheduleManager = new ScheduleManager();
            BookingManager bookingManager = new BookingManager();
            InvoiceManager invoiceManager = new InvoiceManager();

            while (true)
            {
                Console.WriteLine("\n--- Bus Ticket Booking System ---");
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. Show Users");
                Console.WriteLine("3. Create Bus");
                Console.WriteLine("4. Show Buses");
                Console.WriteLine("5. Create Schedule");
                Console.WriteLine("6. Show Schedules");
                Console.WriteLine("7. Show Schedule Details");
                Console.WriteLine("8. Book Ticket");
                Console.WriteLine("9. Show Invoices of a user");
                Console.WriteLine("10. Pay Invoice");
                Console.WriteLine("11. Show Tickets of a User");
                Console.WriteLine("12. Exit");

                Console.Write("\nEnter choice: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            userManager.CreateUser();
                            break;
                        case "2":
                            userManager.ShowAllUsers();
                            break;
                        case "3":
                            busManager.CreateBus();
                            break;
                        case "4":
                            busManager.ShowAllBuses();
                            break;
                        case "5":
                            scheduleManager.CreateSchedule();
                            break;
                        case "6":
                            scheduleManager.ShowAllSchedules();
                            break;
                        case "7":
                            scheduleManager.ShowScheduleDetails();
                            break;
                        case "8":
                            bookingManager.BookTicket();
                            break;
                        case "9":
                            invoiceManager.ShowUserInvoices();
                            break;
                        case "10":
                            invoiceManager.PayInvoice();
                            break;
                        case "11":
                            bookingManager.ShowUserTickets();
                            break;
                        case "12":
                            Console.WriteLine("\nThank you for using the system!");
                            return;
                        default:
                            Console.WriteLine("\nInvalid choice! Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Please try again with valid input.");
                }
            }
        }
    }
}
