using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using aGrouponClasses.Models;
using aGrouponClasses.Utils;

namespace B2B.Models
{ 
    public class USERRepositoryEF : IUSERRepository
    {
        aGrouponModelsDataContext context = new aGrouponModelsDataContext();

        public IQueryable<tUser> All
        {
			get { return context.tUsers; }
        }

        public tUser Find(int id)
        {
            return context.tUsers.Where(t=>t.IDUser.Equals(id)).FirstOrDefault();
        }

        public void InsertOrUpdate(tUser user)
        {
            if (user.IDUser == default(int)) {
                // New entity
                context.tUsers.InsertOnSubmit(user);
            } else {
                // Existing entity
                //context.USERs.Attach(user);
                //context.Entry(user).State = EntityState.Modified;
                tUser userToUpdate = Find(user.IDUser);
                if(userToUpdate!=null && userToUpdate.IDUser>0)
                {
                    userToUpdate.IDRole = user.IDRole;
                    userToUpdate.Password = user.Password;
                    userToUpdate.LastLoginDate = user.LastLoginDate;
                    userToUpdate.Approved = user.Approved;
                    userToUpdate.IDPartnerCategory = user.IDPartnerCategory;
                    userToUpdate.IDCity = user.IDCity;
                }
            }
            context.SubmitChanges();
        }

        public void Delete(int id)
        {
            tUser user = Find(id);
            if(user!= null && user.IDUser>0)
                context.tUsers.DeleteOnSubmit(user);
            context.SubmitChanges();
        }

        public void Save()
        {
            context.SubmitChanges();
        }
		
		//public List<USER> GetListSorted(string sort_by, string sort_dir)
        //{
        //    var mObject = from t in context.USERs
        //                    select t;

        //    if (sort_by != string.Empty)
        //    {
        //        var lObjects = mObject.SortBy(sort_by + sort_dir);
        //        var sortedObjects = from t in lObjects
        //                            select t;
        //        return sortedObjects.ToList<USER>();
        //    }
        //    else
        //    {
        //        var lObjects = mObject;
        //        var sortedObjects = from t in lObjects
        //                            select t;
        //        return sortedObjects.ToList<USER>();
        //    }
        //}


        public tUser GetSingleByUsernamePassword(string username, string password)
        {
            return context.tUsers.Where(x => x.UserName == username && x.Password == password).FirstOrDefault();
        }

        public tUser GetSingleByEmail(string email)
        {
            return context.tUsers.Where(x => x.UserName == email).FirstOrDefault();
        }

        public tUser GetSingleByUserName(string username)
        {
            return context.tUsers.Where(x => x.UserName == username).FirstOrDefault();
        }

        public List<tUser> GetListByIDRole(int idRole)
        {
            return context.tUsers.Where(x => x.tRole.IDRole == idRole).ToList();
        }

        public List<tUser> GetListPartnerByIDCity(int IDCity) {
            return context.tUsers.Where(x => x.IDCity.Equals(IDCity) && x.IDRole.Equals((int)Enums.enmRoles.Partner)).ToList();
        }
    }

	public interface IUSERRepository
    {
		IQueryable<tUser> All { get; }
		tUser Find(int id);
		void InsertOrUpdate(tUser user);
        void Delete(int id);
        void Save();

		//List<USER> GetListSorted(string sort_col,string sort_dir);

        tUser GetSingleByUserName(string username);

        tUser GetSingleByUsernamePassword(string username, string password);

        tUser GetSingleByEmail(string email);

	    List<tUser> GetListByIDRole(int idRole);
        List<tUser> GetListPartnerByIDCity(int IDCity);
    }
}