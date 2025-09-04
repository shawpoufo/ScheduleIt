namespace Domain.Common
{
    /// <summary>
    /// Represents the current status of an appointment
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>
        /// Appointment is scheduled and confirmed
        /// </summary>
        Scheduled = 1,
        
        /// <summary>
        /// Appointment has been canceled
        /// </summary>
        Canceled = 2,
        
        /// <summary>
        /// Appointment has been completed
        /// </summary>
        Completed = 3,
        
        /// <summary>
        /// Appointment is in progress
        /// </summary>
        InProgress = 4,
        
        /// <summary>
        /// Appointment was missed (no-show)
        /// </summary>
        NoShow = 5
    }
}
