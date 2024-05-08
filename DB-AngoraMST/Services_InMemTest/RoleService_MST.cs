using DB_AngoraLib.Models;
using DB_AngoraLib.Services.RoleService;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class RoleService_MST
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly RoleService _roleService;

        public RoleService_MST()
        {
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _roleService = new RoleService(_roleManagerMock.Object, _userManagerMock.Object);
        }

        [TestMethod]
        public async Task CreateRoleAsync_ShouldCreateRole_WhenRoleDoesNotExist()
        {
            // Arrange
            string roleName = "Admin";
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleName)).ReturnsAsync(false);

            // Act
            await _roleService.CreateRoleAsync(roleName);

            // Assert
            _roleManagerMock.Verify(rm => rm.CreateAsync(It.Is<IdentityRole>(r => r.Name == roleName)), Times.Once);
        }
    }
}
