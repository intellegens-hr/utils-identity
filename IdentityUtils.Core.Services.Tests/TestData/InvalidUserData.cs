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
                  Username = "testusersomething@test.hr",
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
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}