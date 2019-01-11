using Microsoft.Extensions.Configuration;
using Pluralsight.OutsideInTDD.WebAPI.Controllers;
using Pluralsight.OutsideInTDD.WebAPI.Domain;
using System;
using Xunit;

namespace Pluralsight.OutsideInTDD
{
    public class JournalControllerTests
    { 

        [Fact]
        [UseDatabase]
        public void GettingJournalEntriesReturnsAnArrayOfJournalEntries()
        {
            var controller = new JournalController();

            var result = controller.Get();

            Assert.IsType<JournalEntry[]>(result);
        }

        [Fact]
        [UseDatabase]
        public void PostingAJournalEntryCreatesAJournalEntry()
        {
            var controller = new JournalController();

            var expected = new JournalEntry
            {
                Time = DateTime.Now,
                Distance = 5000,
                Duration = TimeSpan.FromMinutes(24)
            };

            var result = controller.Post(expected);

            Assert.IsType<JournalEntry>(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        [UseDatabase]
        public void GettingJournalEntriesAfterPostingReturnsPostedJournalEntry()
        {
            var controller = new JournalController();
            var expected = new JournalEntry
            {
                Time = DateTime.Now,
                Distance = 5000,
                Duration = TimeSpan.FromMinutes(24)
            };

            controller.Post(expected);
            var actual = controller.Get();
            
            Assert.Equal(new JournalEntry[] { expected }, actual);
        }
    }
}