using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;

namespace UEBase
{

    public class CLogin : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        [DllImport("ADVAPI32.dll", EntryPoint = "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        public bool Login(string username, string password)
        {
            IntPtr token = IntPtr.Zero;
            //пробиваем по домену
            return LogonUser(username, ConfigurationManager.AppSettings["Domain"], password, 2, 0, ref token);
        }
    }



    public class CUser
    {
        public string uname { get; set; }
        public string fio { get; set; }
        public string divisions { get; set; }
        public string office { get; set; }
        public bool admin { get; set; }
        public bool superAdmin { get; set; } // супер администратор, может всё
        public bool registered { get; set; } // пользователь может быть заведен в адмике а может быть и простым. Если простой - показываем только технику за ним. 
    }

    public class ADuser
    {
        public string name { get; set; }
        public string reply { get; set; }
        public string samAccountName { get; set; }
        public string office { get; set; }
        public string placement { get; set; }
    }

    public class ADconnector
    {
        private readonly PrincipalContext pc;

        public ADconnector()
        {
            pc = new PrincipalContext(ContextType.Domain);
        }

        static string GetProperty(Principal principal, string property)
        {
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            return directoryEntry.Properties.Contains(property) ? directoryEntry.Properties[property].Value.ToString() : string.Empty;
        }

        public List<ADuser> GetUser(string name)
        {
            List<ADuser> result = new List<ADuser>();
            List<UserPrincipal> sp = new List<UserPrincipal>
            {
                new UserPrincipal(pc) {DisplayName = $"{name}*", EmailAddress = "*", Description="stat"}
            };
            List<Principal> results = new List<Principal>();
            foreach (PrincipalSearcher searcher in sp.Select(item => new PrincipalSearcher(item)))
            {
                results.AddRange(searcher.FindAll());
            }
            foreach (Principal one in results)
            {
                result.Add(new ADuser { name = one.DisplayName, reply = GetProperty(one, "autoReplyMessage"), samAccountName = one.SamAccountName, office = GetProperty(one, "streetAddress"), placement=GetProperty(one,"autoReplyMessage") });
            }
            return result;
        }

        public ADuser GetByUname(string uname)
        {

            ADuser result = new ADuser();
            List<UserPrincipal> sp = new List<UserPrincipal>
            {
                new UserPrincipal(pc) { SamAccountName = $"{uname}", EmailAddress = "*", Description="stat"}
            };
            List<Principal> results = new List<Principal>();
            foreach (PrincipalSearcher searcher in sp.Select(item => new PrincipalSearcher(item)))
            {
                results.AddRange(searcher.FindAll());
            }
            foreach (Principal one in results)
            {
                result = new ADuser { name = one.DisplayName, reply = GetProperty(one, "autoReplyMessage"), samAccountName = one.SamAccountName, office = GetProperty(one, "streetAddress"), placement = GetProperty(one, "autoReplyMessage") };
            }
            return result;
        }
    }
}