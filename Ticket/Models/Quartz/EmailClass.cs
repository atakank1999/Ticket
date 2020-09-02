using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Quartz;

namespace Ticket.Models.Quartz
{
    public class EmailClass : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(dataMap.GetString("Email")));
            if (int.Parse(dataMap.GetString("Seconds"))<10)
            {
                message.Subject = dataMap.GetString("Title") + "başlıklı görevinizin süresi dolmuştur";
            }
            else
            {
            message.Subject = dataMap.GetString("Title") +"başlıklı görevinize "+ dataMap.GetString("Time")+" saat kaldı !";

            }
            message.Body = "Görevinizin içeriği şöyledir : \n"+dataMap.GetString("Body");
            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
            Console.WriteLine("alo");
        }
    }
}