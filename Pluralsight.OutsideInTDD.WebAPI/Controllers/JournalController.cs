using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pluralsight.OutsideInTDD.WebAPI.Domain;
using System.Collections.Generic;

namespace Pluralsight.OutsideInTDD.WebAPI.Controllers
{
    public class JournalController : Controller
    {
        private readonly static List<JournalEntry> entries = new List<JournalEntry>();
        
        [HttpGet]
        public JournalEntry[] Get()
        {
            return entries.ToArray();
        }

        [HttpPost]
        public JournalEntry Post(JournalEntry journalEntry)
        {
            entries.Add(journalEntry);
            return journalEntry;
        }
    }
}