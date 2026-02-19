using System;
using System.Collections.Generic;
using System.Linq;

namespace MedScheduler
{
    public class AppointmentScheduler
    {
        private readonly List<Appointment> _appointments = new();

        private readonly TimeSpan _open = new(8, 0, 0);
        private readonly TimeSpan _close = new(17, 0, 0);
        private readonly TimeSpan _minDuration = new(0, 15, 0);

        public IReadOnlyList<Appointment> Appointments => _appointments.AsReadOnly();

        public void Add(Appointment appt)
        {
            ValidateTimeRules(appt.Start, appt.End);
            EnsureNoConflicts(appt, null);

            _appointments.Add(appt);
            Logger.Info($"Added {appt}");
        }

        public bool Cancel(string id)
        {
            var appt = _appointments.FirstOrDefault(a =>
                a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (appt == null)
                return false;

            _appointments.Remove(appt);
            Logger.Info($"Cancelled {appt}");
            return true;
        }

        public void Reschedule(string id, DateTime newStart, DateTime newEnd)
        {
            var appt = _appointments.FirstOrDefault(a =>
                a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (appt == null)
                throw new KeyNotFoundException($"Appointment '{id}' not found.");

            ValidateTimeRules(newStart, newEnd);

            var temp = new Appointment(appt.Id, appt.PatientName,
                appt.ProviderName, newStart, newEnd, appt.Room);

            EnsureNoConflicts(temp, appt.Id);

            var before = appt.ToString();
            appt.Reschedule(newStart, newEnd);

            Logger.Info($"Rescheduled {before} -> {appt}");
        }

        public IEnumerable<Appointment> ListByProvider(string provider)
        {
            return _appointments
                .Where(a => a.ProviderName.Equals(provider, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Start);
        }

        public IEnumerable<Appointment> ListByDay(DateTime day)
        {
            var target = day.Date;
            return _appointments
                .Where(a => a.Start.Date == target)
                .OrderBy(a => a.Start);
        }

        public IEnumerable<Appointment> All()
        {
            return _appointments.OrderBy(a => a.Start);
        }

        private void ValidateTimeRules(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new InvalidAppointmentTimeException("End time must be after start time.");

            if ((end - start) < _minDuration)
                throw new InvalidAppointmentTimeException("Appointment must be at least 15 minutes.");

            if (start.TimeOfDay < _open || end.TimeOfDay > _close)
                throw new InvalidAppointmentTimeException("Appointment must be within business hours.");
        }

        private void EnsureNoConflicts(Appointment candidate, string? excludeId)
        {
            bool Overlaps(Appointment a, Appointment b)
                => a.Start < b.End && b.Start < a.End;

            foreach (var existing in _appointments)
            {
                if (!string.IsNullOrEmpty(excludeId) &&
                    existing.Id.Equals(excludeId, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (Overlaps(candidate, existing))
                {
                    bool providerClash = existing.ProviderName.Equals(candidate.ProviderName,
                        StringComparison.OrdinalIgnoreCase);
                    bool roomClash = existing.Room.Equals(candidate.Room,
                        StringComparison.OrdinalIgnoreCase);

                    if (providerClash || roomClash)
                        throw new DoubleBookingException("Double booking detected.");
                }
            }
        }
    }
}
