using System;

namespace MedScheduler
{
    public class Appointment
    {
        public string Id { get; }
        public string PatientName { get; }
        public string ProviderName { get; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public string Room { get; }

        public Appointment(string id, string patientName, string providerName,
                           DateTime start, DateTime end, string room)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(patientName))
                throw new ArgumentException("Patient name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Provider name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be null or empty.");
            if (end <= start)
                throw new ArgumentException("End time must be after start time.");

            Id = id;
            PatientName = patientName;
            ProviderName = providerName;
            Start = start;
            End = end;
            Room = room;
        }

        public void Reschedule(DateTime newStart, DateTime newEnd)
        {
            if (newEnd <= newStart)
                throw new ArgumentException("End time must be after start time.");

            Start = newStart;
            End = newEnd;
        }

        public override string ToString()
        {
            return $"[ {Id} ] {Start:HH:mm}–{End:HH:mm} {ProviderName} Room {Room}";
        }
    }
}
