using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL
{
    public class DBInitializer : DropCreateDatabaseAlways<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            if (context.Roles.Count() < 1)
            {
                IList<Role> defaultRoles = new List<Role> {
                    new Role() { Name = "Bank Employee", Description = "Requirement employee of bank" },
                    new Role() { Name = "Client", Description = "Any client" }
                };
                context.Roles.AddRange(defaultRoles);
                context.SaveChanges();
            }

            if (context.Clients.Count() < 1)
            {
                IList<Client> fakeClients = new List<Client> {
                    new Client() { IPN = "00110022", FirstName = "Super", SecondName = "Bank",
                        Email = "superbank@gmail.com", Address = "Rivne City, Street st. 1",
                        Birthday = new DateTime(1950, 1, 1), Phone = "+380500000000"},
                    new Client() { IPN = "2200345678", FirstName = "Іван", LastName = "Іванович", SecondName = "Іванов",
                        Birthday = new DateTime(2000, 6, 20), Phone = "+380981111111"},
                    new Client() { IPN = "2300345678", FirstName = "Петро", LastName = "Петрович", SecondName = "Петров",
                        Birthday = new DateTime(1990, 1, 10), Phone = "+380972222222", Email = "petrov@gmail.com" },
                    new Client() { IPN = "2350345678", FirstName = "Василь", LastName = "Васильович", SecondName = "Васькін",
                        Birthday = new DateTime(1995, 10, 30), Phone = "+380983333333"},
                };
                context.Clients.AddRange(fakeClients);
                context.SaveChanges();
            }

            if (context.Accounts.Count() < 1)
            {
                IList<Account> fakeAccounts = new List<Account> {
                    new Account() { Number = "UA00000001234567890", Description = "SuperBank", Amount = 1000000000, ClientId = 1},
                    new Account() { Number = "UA12345678901234001", Description = "main", Amount = 300, ClientId = 2},
                    new Account() { Number = "UA12345678901234002", Description = "main", Amount = 500, ClientId = 3},
                    new Account() { Number = "UA12345678901234003", Description = "main", Amount = 800, ClientId = 3},
                    new Account() { Number = "UA12345678901234004", Description = "main", Amount = 100, ClientId = 2},
                    new Account() { Number = "UA12345678901234005", Description = "main", Amount = 1000, ClientId = 4},
                };
                context.Accounts.AddRange(fakeAccounts);
                context.SaveChanges();
            }

            if (context.Operations.Count() < 1)
            {
                var oper2to3 = new Operation()
                {
                    Description = "оплата за товар",
                    Amount = 100,
                    DateTime = new DateTime(2018, 12, 2),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA12345678901234001",
                    ToAccountNumber = "UA12345678901234002",
                    AccountId = 2
                };
                var oper2to3_2 = oper2to3;
                oper2to3_2.AccountId = 3;

                var oper2to6 = new Operation()
                {
                    Description = "переказ власних коштів",
                    Amount = 200,
                    DateTime = new DateTime(2019, 4, 18),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA12345678901234001",
                    ToAccountNumber = "UA12345678901234005",
                    AccountId = 2
                };
                var oper2to6_2 = oper2to6;
                oper2to6_2.AccountId = 6;

                var oper6to4 = new Operation()
                {
                    Description = "переказ особливих коштів))",
                    Amount = 250,
                    DateTime = new DateTime(2019, 11, 26),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA12345678901234005",
                    ToAccountNumber = "UA12345678901234003",
                    AccountId = 6
                };
                var oper6to4_2 = oper6to4;
                oper6to4_2.AccountId = 4;

                var oper3to2 = new Operation()
                {
                    Description = "переказ особливих коштів))",
                    Amount = 500,
                    DateTime = new DateTime(2020, 3, 12),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA12345678901234002",
                    ToAccountNumber = "UA12345678901234001",
                    AccountId = 3
                };
                var oper3to2_2 = oper3to2;
                oper3to2_2.AccountId = 2;

                var oper1to2 = new Operation()
                {
                    Description = "зарахування готівки",
                    Amount = 500,
                    DateTime = new DateTime(2020, 5, 16),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA00000001234567890",
                    ToAccountNumber = "UA12345678901234001",
                    AccountId = 1
                };
                var oper1to2_2 = oper1to2;
                oper1to2_2.AccountId = 2;

                var oper3to6 = new Operation()
                {
                    Description = "оплата за товар",
                    Amount = 200,
                    DateTime = new DateTime(2020, 9, 25),
                    ResultIsSuccess = true,
                    FromAccountNumber = "UA12345678901234002",
                    ToAccountNumber = "UA12345678901234005",
                    AccountId = 3
                };
                var oper3to6_2 = oper3to6;
                oper3to6_2.AccountId = 6;

                IList<Operation> fakeOperations = new List<Operation> {
                    oper2to3, oper2to3_2, oper2to6, oper2to6_2, oper3to6, oper3to6_2, oper1to2, oper1to2_2, oper3to2, oper3to2_2, oper6to4, oper6to4_2
                };
                context.Operations.AddRange(fakeOperations);
                context.SaveChanges();
            }

            if (context.Users.Count() < 1)
            {
                //        !!!!!    REAL password =  "123456"    !!!!!
                IList<User> fakeUsers = new List<User> {
                    new User() { Login = "employee", ClientId = 1, RoleId = 1,
                        PasswordSalt = "100000.ASbCYb2jwcXgA6ra+sADW6tFcYC1aBGTv4fCqI1ZivEjGA==",
                        PasswordHash = "njkioCQywtRbXgXr6/M0zF3iBQ1gm0aOMCK3orHg35FYomampqG0GEHINrde31GbOjgmOt/tq5Zv5yknS6cvUw=="},
                    new User() { Login = "ivanov", ClientId = 2, RoleId = 2,
                        PasswordSalt = "100000.3i8sz/zhPZLcbMhhixh6JW9k6ZYflkvehS9zHS3khFi+kQ==",
                        PasswordHash = "M5wJ2nswpa2ZU45eRMR8RkzyRr5n1DP+30/bXxNWgRCE/KxnDAU+qOxUnb8bNPoFN5MnHmRMWjSSIGpVqoAEng=="},
                    new User() { Login = "petrov", ClientId = 3, RoleId = 2,
                        PasswordSalt = "100000.bkeQoZATNn8q4F2teFejnkTOpnGETLKumCfUILCotyT3Ig==",
                        PasswordHash = "jpdSZNhQcIQbTERdlg3b4+HbmppSXAnW/645hjkfQh7gTrPbGrIx6KctiSeIYMzxV7LF0g4WNlnLMn+mvP2WLg=="},
                };
                context.Users.AddRange(fakeUsers);
                context.SaveChanges();
            }

            base.Seed(context);
        }

    }
}
