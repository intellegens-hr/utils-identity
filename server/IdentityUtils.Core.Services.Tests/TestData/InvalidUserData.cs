using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using System.Collections;
using System.Collections.Generic;

namespace IdentityUtils.Core.Services.Tests.TestData
{
    public class InvalidUserData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new UserDto
                {
                  Username = "",
                  Password = "",
                  Email = ""
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = $"testusersomething@test.hr",
                  Password = "",
                  Email = ""
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = "testusersomething@test.hr",
                  Password = "",
                  Email = "testusersomething@test.hr"
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = "testusersomething@test.hr",
                  Password = "a",
                  Email = "testusersomething@test.hr"
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = null,
                  Password = null,
                  Email = null
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = $"okemail{new string('X', 300)}@intellegens.hr",
                  DisplayName = "test@intellegens.hr",
                  Password = "FairlyOkPassword34#!",
                  Email = "okemail@intellegens.hr"
                }
            };

            yield return new object[] {
                new UserDto
                {
                  Username = "okemail@intellegens.hr",
                  DisplayName = $"{new string('X', 200)}@intellegens.hr",
                  Password = "FairlyOkPassword34#!",
                  Email = "okemail@intellegens.hr"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}