using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;

namespace Layer.Domain.Interfaces
{
	public interface IEmailSender
	{
		Task<Response> SendEmailAsync(string email, string subject, string message);
	}
}
