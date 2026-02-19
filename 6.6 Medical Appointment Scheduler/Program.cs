using System;
using System.Globalization;
using System.Linq;

namespace MedScheduler
{
    internal static class Program
    {
        private static void Main()
        {
            var scheduler = new AppointmentScheduler();

            Console.WriteLine("=== Medical Appointment Scheduler ===");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add Appointment");
                Console.WriteLine("2. Cancel Appointment");
                Console.WriteLine("3. Reschedule Appointment");
                Console.WriteLine("4. List All Appointments");
                Console.WriteLine("5. List by Provider");
                Console.WriteLine("6. List by Day");
                Console.WriteLine("7. Exit");
                Console.Write("Choose: ");

                switch ((Console.ReadLine() ?? "").Trim())
                {
                    case "1": AddAppointmentMenu(scheduler); break;
                    case "2": CancelAppointmentMenu(scheduler); break;
                    case "3": RescheduleAppointmentMenu(scheduler); break;
                    case "4": ListAllMenu(scheduler); break;
                    case "5": ListByProviderMenu(scheduler); break;
                    case "6": ListByDayMenu(scheduler); break;
                    case "7": running = false; break;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

        private static void AddAppointmentMenu(AppointmentScheduler scheduler)
        {
            try
            {
                var id = Prompt("Enter Appointment ID: ");
                var patient = Prompt("Enter Patient Name: ");
                var provider = Prompt("Enter Provider Name: ");
                var room = Prompt("Enter Room: ");
                var start = PromptDateTime("Enter Start (yyyy-MM-dd HH:mm): ");
                var end = PromptDateTime("Enter End (yyyy-MM-dd HH:mm): ");

                var appt = new Appointment(id, patient, provider, start, end, room);
                scheduler.Add(appt);

                Console.WriteLine("Appointment added.");
            }
            catch (DoubleBookingException ex)
            {
                Logger.Warn(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (InvalidAppointmentTimeException ex)
            {
                Logger.Warn(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine("Unexpected error.");
            }
        }

        private static void CancelAppointmentMenu(AppointmentScheduler scheduler)
        {
            try
            {
                var id = Prompt("Enter Appointment ID: ");
                if (!scheduler.Cancel(id))
                {
                    Logger.Warn($"Cancel failed for ID {id}");
                    Console.WriteLine("Appointment not found.");
                }
                else
                {
                    Console.WriteLine("Appointment cancelled.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine("Error cancelling appointment.");
            }
        }

        private static void RescheduleAppointmentMenu(AppointmentScheduler scheduler)
        {
            try
            {
                var id = Prompt("Enter Appointment ID: ");
                var start = PromptDateTime("Enter New Start (yyyy-MM-dd HH:mm): ");
                var end = PromptDateTime("Enter New End (yyyy-MM-dd HH:mm): ");

                scheduler.Reschedule(id, start, end);
                Console.WriteLine("Appointment rescheduled.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private static void ListAllMenu(AppointmentScheduler scheduler)
        {
            var list = scheduler.All().ToList();
            if (!list.Any())
            {
                Console.WriteLine("No appointments.");
                return;
            }

            Console.WriteLine("\n--- All Appointments ---");
            foreach (var a in list)
                Console.WriteLine(a);
        }

        private static void ListByProviderMenu(AppointmentScheduler scheduler)
        {
            var provider = Prompt("Enter Provider Name: ");
            var list = scheduler.ListByProvider(provider).ToList();

            if (!list.Any())
            {
                Console.WriteLine("No appointments found.");
                return;
            }

            Console.WriteLine($"\n--- Appointments for {provider} ---");
            foreach (var a in list)
                Console.WriteLine(a);
        }

        private static void ListByDayMenu(AppointmentScheduler scheduler)
        {
            var day = PromptDateTime("Enter Date (yyyy-MM-dd HH:mm): ").Date;
            var list = scheduler.ListByDay(day).ToList();

            if (!list.Any())
            {
                Console.WriteLine("No appointments on this day.");
                return;
            }

            Console.WriteLine($"\n--- Appointments on {day:yyyy-MM-dd} ---");
            foreach (var a in list)
                Console.WriteLine(a);
        }

        private static string Prompt(string label)
        {
            Console.Write(label);
            return (Console.ReadLine() ?? "").Trim();
        }

        private static DateTime PromptDateTime(string label)
        {
            Console.Write(label);
            var s = Console.ReadLine();

            if (!DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt))
            {
                throw new ArgumentException("Invalid date/time format.");
            }

            return dt;
        }
    }
}
