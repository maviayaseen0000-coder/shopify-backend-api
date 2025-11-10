using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace ShopifyBackendAPI.Controllers
{
	[ApiController]
	[Route("send-quote")]
	public class QuoteController : ControllerBase
	{
		[HttpPost]
		[DisableRequestSizeLimit] // allow file upload
		public async Task<IActionResult> SendQuote()
		{
			var form = await Request.ReadFormAsync();

			// ✅ Read form fields
			string email = form["email"];
			string fullName = form["fullName"];
			string businessName = form["businessName"];
			string specifics = form["specifics"];
			string placement = form["placement"];
			string mockupType = form["mockupType"];
			string budget = form["budget"];
			string timeline = form["timeline"];
			string phone = form["phone"];
			string details = form["details"];

			var file = form.Files["designFile"]; // uploaded file

			// ✅ Build email
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(fullName, email));
			message.To.Add(new MailboxAddress("Admin", Environment.GetEnvironmentVariable("SMTP_USER")));
			message.Subject = "New Quote Request From Shopify";

			var builder = new BodyBuilder
			{
				HtmlBody = $@"
                    <h2>New Quote Request</h2>
                    <p><strong>Name:</strong> {fullName}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Phone:</strong> {phone}</p>
                    <p><strong>Business:</strong> {businessName}</p>
                    <p><strong>Specifics:</strong> {specifics}</p>
                    <p><strong>Placement:</strong> {placement}</p>
                    <p><strong>Mockup Type:</strong> {mockupType}</p>
                    <p><strong>Budget:</strong> {budget}</p>
                    <p><strong>Timeline:</strong> {timeline}</p>
                    <p><strong>Details:</strong> {details}</p>
                "
			};

			// ✅ Add file attachment
			if (file != null && file.Length > 0)
			{
				using var ms = new MemoryStream();
				await file.CopyToAsync(ms);
				builder.Attachments.Add(file.FileName, ms.ToArray());
			}

			message.Body = builder.ToMessageBody();

			// ✅ Send email using Gmail SMTP
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

			var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
			var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");

			await smtp.AuthenticateAsync(smtpUser, smtpPass);
			await smtp.SendAsync(message);
			await smtp.DisconnectAsync(true);

			return Ok(new { success = true });
		}

		// ✅ Test endpoint to check API is running
		[HttpGet("test")]
		public IActionResult Test() => Ok("API is running!");
	}
}
