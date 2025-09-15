using SqlCommands.Attributes;
using SqlCommands.Commands;
using SqlCommands.Core;
using SqlCommands.Factories;

namespace SqlCommandsTest;

internal class Program
{
    static void Main(string[] args)
    {
        SqlClient sqlClient  = SqlClientFactory.CreateSqliteClient(@"DataSource=C:\Users\avaz1\Downloads\Chinook_Sqlite.sqlite");
        // List<Customer> customers = sqlClient.Select<Customer>(new Customer{ LastName = "Almeida"}).ToList();
        // List<Employee> employees = sqlClient.Select<Employee>(filter: new("Title LIKE '%Sale%'", []), maxResults: 1).ToList();
        // Employee employee = sqlClient.SelectFirst<Employee>(filter: new("Title LIKE '%Sale%'", []));
        //int result = sqlClient.Insert(new Genre { Name = "Custom" });
        // int result = sqlClient.Delete(new Genre { Id = 26 });
        // List<Genre> genres = sqlClient.Select<Genre>().ToList();
        int b = 0;
    }
}

public class Customer
{
    [SqlColumn(isPrimaryKey: true)]
    public long? CustomerId { get; set; }

    public string FirstName { get; set; }

    [SqlIgnore]
    public string LastName { get; set; }

    public string Company { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }
}

public class Employee
{
    [SqlColumn("EmployeeId", isPrimaryKey: true)]
    public long? Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Title { get; set; }
}

public class Genre
{
    [SqlColumn("GenreId", isPrimaryKey: true)]
    public long? Id { get; set; }

    public string Name { get; set; }
}

public class Award
{
    [SqlColumn("AwardId", isPrimaryKey: true, isAutoIncrement: true)]
    public long Id { get; set; }

    public string Name { get; set; }

    public DateTime CeremonyDate { get; set; }
}