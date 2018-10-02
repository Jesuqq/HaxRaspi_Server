using System;
using System.Collections.Generic;
using System.Linq;
using HaxRaspi_Server.Entities;
using HaxRaspi_Server.Helpers;

namespace HaxRaspi_Server.Services
{
    public interface IInterfaceService
    {
        Interface CreateOrUpdate(Interface iface);
        List<Interface> GetAllInterfaces();
        Interface GetByName(string name);
    }

    public class InterfaceService : IInterfaceService
    {
        private DataContext _dataContext;
        private TimeSpan _disconnectedTimeSpan = new TimeSpan(0, 0, 30);
        private TimeSpan _expirationTimeSpan = new TimeSpan(24, 0, 0);

        #region Public Methods
        public InterfaceService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public Interface CreateOrUpdate(Interface iface)
        {
            // Update if exists
            var existingIface = _dataContext.Interfaces.Where(i => i.Name.Equals(iface.Name)).FirstOrDefault();
            if (existingIface != null)
            {
                existingIface.Broadcast = iface.Broadcast;
                existingIface.Connected = iface.Connected;
                existingIface.IP = iface.IP;
                existingIface.IPv6 = iface.IPv6;
                existingIface.MAC = iface.MAC;
                existingIface.Name = iface.Name;
                existingIface.Updated = DateTimeOffset.Now;
                _dataContext.Interfaces.Update(existingIface);
                _dataContext.SaveChanges();
                return existingIface;
            }
            // Create new iface if does not exist
            iface.Updated = DateTimeOffset.Now;
            _dataContext.Interfaces.Add(iface);
            _dataContext.SaveChanges();
            return iface;
        }

        public List<Interface> GetAllInterfaces()
        {
            try
            {
                var ifaces = _dataContext.Interfaces.ToList();
                List<Interface> filteredList = new List<Interface>();
                ifaces.ForEach(i => filteredList.Add(CheckTimeSpans(i)));
                filteredList.RemoveAll(i => i == null);
                return filteredList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while fetching all ifaces: " + e.ToString());
                return null;
            }
        }

        public Interface GetByName(string name)
        {
            try
            {
                var iface = _dataContext.Interfaces.Where(i => i.Name.Equals(name)).FirstOrDefault();
                return iface;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while searching iface by name: " + e.ToString());
                return null;
            }
        }

        #endregion
        #region Private Methods

        private Interface CheckTimeSpans(Interface iface)
        {
            // If ExpirationTimeSpan has passed, but state was not yet disconnected, dont return and update
            if ((DateTimeOffset.Now - iface.Updated).TotalMinutes > _expirationTimeSpan.TotalMinutes)
            {
                iface.Connected = false;
                iface = Update(iface);
                return null;
            }

            // If DisconnectedTimeSpan has passed, set connected flag to false and update
            if (((DateTimeOffset.Now - iface.Updated).TotalMinutes > _disconnectedTimeSpan.TotalMinutes) && iface.Connected)
            {
                iface.Connected = false;
                iface = Update(iface);
                return iface;
            }
            return iface;
        }

        private Interface Update(Interface iface)
        {
            try
            {
                var oldIface = _dataContext.Interfaces.Find(iface.Id);
                if (oldIface == null) return null;
                _dataContext.Entry(oldIface).CurrentValues.SetValues(iface);
                _dataContext.SaveChanges();
                return iface;
            }
            catch (Exception e)
            {
                Console.Write("Exception while updating: " + e.Message + ", " + e.ToString());
                return null;
            }
        }
        #endregion
    }
}