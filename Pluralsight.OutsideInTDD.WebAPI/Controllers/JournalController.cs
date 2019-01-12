using Microsoft.AspNetCore.Mvc;
using Pluralsight.OutsideInTDD.WebAPI.Data;
using Pluralsight.OutsideInTDD.WebAPI.Domain;

namespace Pluralsight.OutsideInTDD.WebAPI.Controllers
{
    public class JournalController : ControllerBase
    {
        [HttpGet]
        public JournalEntry[] Get()
        {
            return new Repository().GetForUser("foo");
        }

        [HttpPost]
        public JournalEntry Post(JournalEntry journalEntry)
        {
            return journalEntry;
        }
    }
}