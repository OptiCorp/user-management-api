﻿using usermanagement.core.Models;
using usermanagement.core.Models.DTO;
using Microsoft.EntityFrameworkCore;
using usermanagement.core.Utilities;

namespace usermanagement.core.Services
{
    public class UserRoleService : IUserRoleService
    {
        public readonly UserManagementDbContext _context;
        private readonly IUserRoleUtilities _userRoleUtilities;

        public UserRoleService(UserManagementDbContext context, IUserRoleUtilities userRoleUtilities)
        {
            _context = context;
            _userRoleUtilities = userRoleUtilities;
        }

        public async Task<bool> IsUserRoleInUseAsync(UserRole userRole)
        {
            return await _context.User.AnyAsync(user => user.UserRole == userRole);
        }

        public async Task<IEnumerable<UserRole>> GetUserRolesAsync()
        {
            return await _context.UserRole.ToListAsync();
        }


        public async Task<UserRole> GetUserRoleByIdAsync(string id)
        {
            return await _context.UserRole.FirstOrDefaultAsync(userRole => userRole.Id == id);

        }

        public async Task<UserRole> GetUserRoleByUserRoleNameAsync(string userRoleName)
        {
            return await _context.UserRole.FirstOrDefaultAsync(userRole => userRole.Name == userRoleName);
        }

        public async Task<string> CreateUserRoleAsync(UserRoleCreateDto userRoleDto)
        {
            var userRole = new UserRole
            {
                Name = userRoleDto.Name,
            };

            _context.UserRole.Add(userRole);
            await _context.SaveChangesAsync();

            return userRole.Id;
        }

        public async Task UpdateUserRoleAsync(UserRoleUpdateDto updatedUserRole)
        {
            var userRole = await _context.UserRole.FirstOrDefaultAsync(userRole => userRole.Id == updatedUserRole.Id);

            if (userRole != null)
            {
                if (updatedUserRole.Name != null)
                {
                    userRole.Name = updatedUserRole.Name;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserRoleAsync(string id)
        {

            var userRole = await GetUserRoleByIdAsync(id);

            if (userRole != null)
            {
                _context.UserRole.Remove(userRole);
                await _context.SaveChangesAsync();
            }

        }


    }
}
