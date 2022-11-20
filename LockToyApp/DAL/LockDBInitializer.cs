using LockToyApp.DBEntities;

namespace LockToyApp.DAL
{
    public static class LockDBInitializer
    {
        public static void IntializeDB(LockDBContext context)
        {


            var normalDoor1 = new Door { CompanyName = "companyA", Location = "frontDoor", DoorID = new Guid("87851188-1d8d-4330-875b-ddbdd876b867") };
            var normalDoor2 = new Door { CompanyName = "companyB", Location = "frontDoor", DoorID = new Guid("55d0d901-2db9-4c47-b858-78334b8fc043") };

            var secretDoor1 = new Door { CompanyName = "companyA", Location = "backDoor", DoorID = new Guid("4c903e6a-89e7-44ea-8490-3443c9dc9614") };
            var secretDoor2 = new Door { CompanyName = "companyB", Location = "backDoor", DoorID = new Guid("3e4e19f2-5cee-4b23-b423-c98f5073ef89") };
            var secretDoor3 = new Door { CompanyName = "companyC", Location = "backDoor", DoorID = new Guid("64a6af7a-2c8c-42e9-8ba6-19a4dfe62f86") };
           

            var normalUser = new User()
            {
                UserName = "User",
                UserType = Models.UserType.NormalUser,
                Token = "NkphZkw2NjVuVHRUQUNLR+DCtWNwQUBWUlbrmKKeRgKj5paN",
                CreatedDate = DateTime.Now
            };

            var adminUser = new User()
            {
                UserName = "Admin",
                UserType = Models.UserType.Administrator,
                Token = "NkphZkw2NjVuVHRUQUNLR6GzlfnbeeiHbNmxtkeCPMyODsE8",
                CreatedDate = DateTime.Now,
            };

            var registrations = new UserRegistration[]
            {
                new UserRegistration
                {
                    User = adminUser,
                    Door = normalDoor1,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = adminUser,
                    Door = normalDoor2,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = adminUser,
                    Door = secretDoor1,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = adminUser,
                    Door = secretDoor2,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = adminUser,
                    Door = secretDoor3,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = normalUser,
                    Door = normalDoor1,
                    RegistrationDate = DateTime.Now
                },
                new UserRegistration
                {
                    User = normalUser,
                    Door = normalDoor2,
                    RegistrationDate = DateTime.Now
                }

            };
            context.Database.EnsureCreated();

            if (context.Users.Any() && context.Doors.Any())
            {
                return;
            }

            var allDoors = new List<Door> { normalDoor1, normalDoor2, secretDoor1, secretDoor2, secretDoor3 };
            var allUsers = new List<User> { normalUser, adminUser };
            context.Users.AddRange(allUsers);
            context.Doors.AddRange(allDoors);
            context.SaveChanges();

            context.UserRegistrations.AddRange(registrations);
            context.SaveChanges();

        }
    }
}
