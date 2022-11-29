using LockToyApp.DBEntities;
using LockToyApp.Services;
using Microsoft.Extensions.Options;
using Moq;
using ToyContracts;
using Xunit;

namespace LockToyApp.Tests
{
    public class ServiceTests
    {
        [Fact]
        public void PasswordHashTest()
        {
            // Arrange
            var expectedHash = "NkphZkw2NjVuVHRUQUNLR6sh/nMBY6M6ZydXgYsvC0TmMT8y";
            var users = new[] { new DBEntities.User() { UserName = "User", Token = "NkphZkw2NjVuVHRUQUNLR+DCtWNwQUBWUlbrmKKeRgKj5paN" } };
            var mockedIOption = new Mock<IOptionsSnapshot<Settings>>();
            mockedIOption.Setup(x => x.Value).Returns(new Settings {SaltString = "6JafL665nTtTACKG" });
            var mockedHasher = new PasswordHasher(mockedIOption.Object);

            // Execute
            var result = mockedHasher.Generate("123456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedHash, result);

        }

        [Fact]
        public void PasswordHash_SaltTooShort_ThrowExceptionTest()
        {
            // Arrange
            var expectedHash = "NkphZkw2NjVuVHRUQUNLR6sh/nMBY6M6ZydXgYsvC0TmMT8y";
            var users = new[] { new DBEntities.User() { UserName = "User", Token = "NkphZkw2NjVuVHRUQUNLR+DCtWNwQUBWUlbrmKKeRgKj5paN" } };
            var mockedIOption = new Mock<IOptionsSnapshot<Settings>>();
            mockedIOption.Setup(x => x.Value).Returns(new Settings { SaltString = "6JafL" });
            var mockedHasher = new PasswordHasher(mockedIOption.Object);

            try
            {
                // Execute
                var result = mockedHasher.Generate("123456");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.NotNull(ex);
                Assert.Equal(typeof (ArgumentException) , ex.GetType());
            }
      
        }
    }
}