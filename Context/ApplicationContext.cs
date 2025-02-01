using System;
using Microsoft.EntityFrameworkCore;
using webApiPeople.Models;

namespace webApiPeople.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    public DbSet<Person> Persons { get; set; }
}
