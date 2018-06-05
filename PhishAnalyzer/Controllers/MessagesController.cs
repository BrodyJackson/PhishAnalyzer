using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MsgReader.Outlook;
using PhishAnalyzer.Data;
using PhishAnalyzer.Models;

namespace PhishAnalyzer.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Message.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .SingleOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("UploadFiles")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files) 
            //Create([Bind("ID,sender,subject,body,recieved")] Message message)
        {
            var filePath = Path.GetTempFileName();
            Random rnd = new Random(); 
            foreach (var formFile in files)
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    using (var msg = new MsgReader.Outlook.Storage.Message(filePath))
                    {
                        int id = rnd.Next(); 
                        var from = msg.GetEmailSender(false, false);
                        var sentOn = msg.SentOn;
                        var sentTo = msg.GetEmailRecipients(Storage.Recipient.RecipientType.To, false, false); 
                        var sentTocc = msg.GetEmailRecipients(Storage.Recipient.RecipientType.Cc, false, false);
                        var subject = msg.Subject;
                        var htmlBody = msg.BodyHtml;
                        var email = new Message(id, from, sentOn, sentTo, sentTocc, subject, htmlBody);
                        Console.WriteLine(email);

                        if (ModelState.IsValid)
                        {
                            _context.Add(email);
                            await _context.SaveChangesAsync();
                        }
                    }                
                }
            return RedirectToAction(nameof(Index));
            /*return View(message)*/;
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.SingleOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,sender,subject,body,recieved")] Message message)
        {
            if (id != message.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .SingleOrDefaultAsync(m => m.ID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Message.SingleOrDefaultAsync(m => m.ID == id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.ID == id);
        }
    }
}
