﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aGrouponClasses.Models;

namespace aGrouponClasses.Repositories {
    public class ContentRepositoryEF : IContentRepository {
        aGrouponModelsDataContext context = new aGrouponModelsDataContext();
        public IQueryable<tContent> All {
            get { return context.tContents; }
        }

        public tContent Find(int id) {
            return context.tContents.Where(t => t.IDContent.Equals(id)).FirstOrDefault();
        }

        public tContent FindByCode(string Code) {
            return context.tContents.Where(t => t.Code.Equals(Code)).FirstOrDefault();
        }

        public void InsertOrUpdate(tContent user) {
            if (user.IDContent == default(int)) {
                // New entity
                context.tContents.InsertOnSubmit(user);
            } else {
                // Existing entity
                tContent userToUpdate = Find(user.IDContent);
                if (userToUpdate != null && userToUpdate.IDContent > 0) {
                    userToUpdate.Name = user.Name;
                    userToUpdate.IDCategory = user.IDCategory;
                    userToUpdate.Code = user.Code;
                    userToUpdate.Description = user.Description;
                    userToUpdate.ShowInMenuFlag = user.ShowInMenuFlag;
                }
            }
        }

        public void Delete(int id) {
            tContent user = Find(id);
            if (user != null && user.IDContent > 0)
                context.tContents.DeleteOnSubmit(user);
        }

        public void Save() {
            context.SubmitChanges();
        }

        public List<tContent> GetListByIDCategory(int IDCategory)
        {
            return context.tContents.Where(t => t.IDCategory.Equals(IDCategory)).ToList();
        }
    }

    public interface IContentRepository {
        IQueryable<tContent> All { get; }
        tContent Find(int id);
        tContent FindByCode(string Code);
        List<tContent> GetListByIDCategory(int IDCategory);
        void InsertOrUpdate(tContent user);
        void Delete(int id);
        void Save();
    }
}
