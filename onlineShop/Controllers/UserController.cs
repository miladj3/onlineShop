using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using onlineShop.Contracts;
using onlineShop.Filters;
using onlineShop.Models;
using onlineShop.Repositories;
using onlineShop.Services;
using onlineShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBreadcrumbNavBuilder _breadcrumbNavBuilder;
        private readonly IUserRepository _userRepository;
        private readonly IAuditTrailService _auditTrailService;

        public UserController(
            UserManager<AppUser> userManager, 
            IConfiguration configuration,
            IBreadcrumbNavBuilder breadcrumbNavBuilder,
            IUserRepository userRepository,
            IAuditTrailService auditTrailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _breadcrumbNavBuilder = breadcrumbNavBuilder;
            _userRepository = userRepository;
            _auditTrailService = auditTrailService;
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Users/Customers")]
        public async Task<IActionResult> ManageCustomers()
        {
            Predicate<int> selectionCondition = roleNumber => roleNumber == 0;
            var userList = await GetUserList(selectionCondition);

            TempData["content"] = "customers";

            _breadcrumbNavBuilder.CreateForNode("CPanelCustomersView", new { }, this);

            return View("ManageUsers", userList);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Users/Employees")]
        public async Task<IActionResult> ManageEmployees()
        {
            Predicate<int> selectionCondition = roleNumber => roleNumber > 0;
            var userList = await GetUserList(selectionCondition);

            TempData["content"] = "employees";

            _breadcrumbNavBuilder.CreateForNode("CPanelEmployeesView", new { }, this);

            return View("ManageUsers", userList);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("/ControlPanel/Users/Edit/{id}")]
        public async Task<IActionResult> AdminViewEdit(string id)
        {
            var user = _userRepository.GetUserInclById(id);

            var vm = new UserViewModel
            {
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                EmailConfirmed = user.EmailConfirmed,
                Orders = user.Orders,
                PhoneNumber = user.PhoneNumber,
                RegisteredOn = user.RegisteredOn,
                UserId = user.Id,
                IsBlocked = user.IsBlocked,
                AllowRoleChange = User.IsInRole("Admin"),
                ChangeHistory = user.ChangeHistory.Select(uch=>uch.ChangeLog).ToList()
            };

            vm.Roles = new List<UserRoleViewModel>();

            var AllRoles = _userRepository.GetAllRoles();
            foreach (var role in AllRoles)
            {
                var isAssigned = await _userManager.IsInRoleAsync(user, role.Name);
                vm.Roles.Add(new UserRoleViewModel {IsAssigned = isAssigned, Rolename = role.Name.ToString() });
            }

            var navNodeName = _userRepository.UserRoleExists(user.Id) ? "CPanelEmployeeEdit" : "CPanelCustomerEdit";
            _breadcrumbNavBuilder.CreateForNode(navNodeName, new { userId = user.Id, userName = user.Email.ToString() }, this);

            return View("AdminViewEdit", vm);
        }

        [ServiceFilter(typeof(DemoRestrictAdmin))]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Save(UserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                _breadcrumbNavBuilder.CreateForNode("CPanelCustomerEdit", new { userId = vm.UserId, userName = vm.Email.ToString() }, this);
                return View("AdminViewEdit", vm);
            }

            var admin = await _userManager.GetUserAsync(User);
            var user = _userRepository.GetUserInclById(vm.UserId);

            // prevent any other admins from editing superUser
            var superUserName = _configuration.GetSection("SuperUser")["Username"];
            var currentUser = await _userManager.GetUserAsync(User);

            if (string.Equals(superUserName, user.UserName, StringComparison.OrdinalIgnoreCase) && currentUser.UserName != superUserName)
                return BadRequest("SuperUsers can be modified only by themselves.");

            user.Email = vm.Email;
            user.Firstname = vm.Firstname;
            user.Lastname = vm.Lastname;
            user.EmailConfirmed = vm.EmailConfirmed;
            user.PhoneNumber = vm.PhoneNumber;
            user.IsBlocked = vm.IsBlocked;
            user.UserName = vm.Email;

            // Retireve changes and log
            var changeLogs = _auditTrailService.RetrieveAndLogChanges();

            foreach (var changeLog in changeLogs)
                user.ChangeHistory.Add(new UserChangeLog { ChangeLog = changeLog, User = user });

            //second validation to prevent non-admins from changing roles
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                // apply changes in role
                foreach (var roleVm in vm.Roles)
                {
                    var IsInRole = await _userManager.IsInRoleAsync(user, roleVm.Rolename);

                    if (IsInRole != roleVm.IsAssigned)
                    {
                        if (roleVm.IsAssigned)
                        {
                            await _userManager.AddToRoleAsync(user, roleVm.Rolename);

                            // create changelog manually to avoid using [AutoSaveChanges = false] when dealing with userManager
                            user.ChangeHistory.Add(new UserChangeLog
                            {
                                User = user,
                                ChangeLog = new ChangeLog
                                {
                                    ChangeType = EntityState.Added,
                                    DateChanged = DateTime.UtcNow,
                                    Employee = admin,
                                    EntityName = "AppUser",
                                    NewValue = roleVm.Rolename,
                                    OldValue = "[ADDED]",
                                    PrimaryKeyValue = user.Id,
                                    PropertyName = "Role"
                                }
                            });
                        }
                        else
                        {
                            await _userManager.RemoveFromRoleAsync(user, roleVm.Rolename);

                            // create changelog manually to avoid using [AutoSaveChanges = false] when dealing with userManager
                            user.ChangeHistory.Add(new UserChangeLog
                            {
                                User = user,
                                ChangeLog = new ChangeLog
                                {
                                    ChangeType = EntityState.Added,
                                    DateChanged = DateTime.Now,
                                    Employee = admin,
                                    EntityName = "AppUser",
                                    NewValue = "[DELETED]",
                                    OldValue = roleVm.Rolename,
                                    PrimaryKeyValue = user.Id,
                                    PropertyName = "Role"
                                }
                            });
                        }
                    }
                }
            }

            _userRepository.SaveChanges();

            return RedirectToAction("AdminViewEdit", new { id = user.Id });
        }

       private async Task<List<UserViewModel>> GetUserList(Predicate<int> selectionCondition)
        {
            var usersInRole = new List<UserViewModel>();

            var allUsers = _userManager.Users.Include(u => u.Orders).AsNoTracking().ToList();

            foreach (var user in allUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                if (selectionCondition(userRoles.Count))
                {
                    var userInRole = new UserViewModel
                    {
                        UserId = user.Id,
                        Firstname = user.Firstname,
                        Lastname = user.Lastname,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        RegisteredOn = user.RegisteredOn,
                        Orders = user.Orders
                    };

                    foreach (var role in userRoles)
                    {
                        userInRole.Roles.Add(new UserRoleViewModel { Rolename = role, IsAssigned = true });
                    }

                    usersInRole.Add(userInRole);
                }

            }

            return usersInRole;
        }

    }
}
