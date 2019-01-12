using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Pluralsight.OutsideInTDD.WebAPI;
using Pluralsight.OutsideInTDD.WebAPI.Controllers;
using Pluralsight.OutsideInTDD.WebAPI.Domain;
using Simple.Data;
using System;
using System.Dynamic;
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
            //var server = new TestServer(new WebHostBuilder().UseStartup<Startup>)
            var expected = new JournalEntry
            {
                Time = DateTimeOffset.Now,
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
                Time = DateTimeOffset.Now,
                Distance = 5000,
                Duration = TimeSpan.FromMinutes(24)
            };

            controller.Post(expected);
            var actual = controller.Get();
            
            Assert.Equal(new JournalEntry[] { expected }, actual);
        }

        [Fact]
        [UseDatabase]
        public void GetRootReturnsCorrectEntryFromDatabase()
        {
            var expected = new JournalEntry();
            expected.Time = DateTimeOffset.Parse("1/11/2019 7:15:50");
            expected.Distance = 5000;
            expected.Duration = TimeSpan.FromMinutes(24);
            expected.UserId = 1;

            var controller = new JournalController();
            var actual = controller.Get();
            actual[0].Time = expected.Time;
            Assert.Contains(expected, actual);
        }
    }
}
