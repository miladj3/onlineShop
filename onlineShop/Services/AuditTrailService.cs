using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using onlineShop.Data;
using onlineShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace onlineShop.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditTrailService(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<ChangeLog> RetrieveAndLogChanges()
        {
            // defaults
            string userId = "[N/A]";
            var changeLogs = new List<ChangeLog>();
            ChangeLog changeLogRecord = new ChangeLog();

            //try to user and userId
            var user = _httpContextAccessor.HttpContext.User;

            if (user != null)
                userId = _userManager.GetUserId(user);

            // get all modified entries
            var modifiedEntities = _context.ChangeTracker.Entries()
                .Where(r => r.State != EntityState.Unchanged).ToList();

            //loop through each change
            foreach (var modifiedEntity in modifiedEntities)
            {
                var entityName = modifiedEntity.Entity.GetType().Name;

                // try to get key by attribute
                var entityKey = modifiedEntity.Entity
                    .GetType()
                    .GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0);

                // if key not found by attribute, try to get "Id" prop
                if (entityKey == null)
                    entityKey = modifiedEntity.Entity
                        .GetType()
                        .GetProperties()
                        .FirstOrDefault(p => p.Name == "Id");

                // try to retrieve key value
                var entityKeyValue = (entityKey != null) ? entityKey.GetValue(modifiedEntity.Entity) : "[N/A]";

                switch (modifiedEntity.State)
                {
                    case EntityState.Added:

                        changeLogRecord = new ChangeLog()
                        {
                            EntityName = entityName,
                            PrimaryKeyValue = entityKeyValue.ToString(),
                            OldValue = "[ADDED]",
                            NewValue = "[ADDED]",
                            DateChanged = DateTime.Now,
                            EmployeeId = userId,
                            ChangeType = EntityState.Deleted
                        };

                        changeLogs.Add(changeLogRecord);
                        _context.ChangeLogs.Add(changeLogRecord);
                        break;

                    case EntityState.Deleted:

                        changeLogRecord = new ChangeLog()
                        {
                            EntityName = entityName,
                            PrimaryKeyValue = entityKeyValue.ToString(),
                            OldValue = "[DELETED]",
                            NewValue = "[DELETED]",
                            DateChanged = DateTime.Now,
                            EmployeeId = userId,
                            ChangeType = EntityState.Deleted
                        };

                        changeLogs.Add(changeLogRecord);
                        _context.ChangeLogs.Add(changeLogRecord);
                        break;

                    case EntityState.Modified:

                        foreach (var prop in modifiedEntity.OriginalValues.Properties)
                        {
                            string oldValue;
                            string newValue;

                            try
                            {
                                oldValue = modifiedEntity.GetDatabaseValues().GetValue<object>(prop).ToString();
                            }
                            catch (Exception)
                            {
                                oldValue = null;
                            }

                            if (modifiedEntity.CurrentValues[prop] != null)
                            {
                                newValue = modifiedEntity.CurrentValues[prop].ToString();
                            }
                            else
                            {
                                newValue = null;
                            }

                            if (oldValue != newValue)
                            {

                                changeLogRecord = new ChangeLog()
                                {
                                    EntityName = entityName,
                                    PrimaryKeyValue = entityKeyValue.ToString(),
                                    PropertyName = prop.Name,
                                    OldValue = oldValue,
                                    NewValue = newValue,
                                    DateChanged = DateTime.Now,
                                    EmployeeId = userId,
                                    ChangeType = EntityState.Modified
                                };

                                changeLogs.Add(changeLogRecord);
                                _context.ChangeLogs.Add(changeLogRecord);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            return changeLogs;
        }
    }
}
