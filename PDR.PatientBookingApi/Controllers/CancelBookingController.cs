using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelBookingController : ControllerBase
    {
        private readonly PatientBookingContext _context;

        public CancelBookingController(PatientBookingContext context)
        {
            _context = context;
        }

		[HttpPost()]
		public IActionResult CancelBooking(CanBooking canBooking)
		{
			// get doctors appointments for this patient
			var bookings = _context.Order.Where(x => (x.DoctorId == canBooking.DoctorId) && (x.PatientId == canBooking.PatientId) && !x.AppointmentCancelled);

			// checking booking exists
			var appointments = bookings.Where(b => (canBooking.StartTime > b.StartTime && canBooking.StartTime < b.EndTime) || canBooking.EndTime > b.StartTime && canBooking.EndTime < b.EndTime).ToList();

			// check if appointment found
			if (appointments.Count() == 0)
			{
				return StatusCode(400);
			}

			var itemToRemove = _context.Order.SingleOrDefault(x => x.Id == appointments[0].Id);

			if (itemToRemove != null)
			{
				itemToRemove.AppointmentCancelled = true;
				_context.SaveChanges();
			}

			return StatusCode(200);
		}

		public class CanBooking
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
        }

    }
}