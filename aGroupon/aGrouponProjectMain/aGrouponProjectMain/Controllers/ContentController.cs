﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aGrouponClasses.Repositories;
using aGrouponClasses.Models;
using aGrouponClasses.Utils;

namespace aGrouponProjectMain.Controllers
{
    public class ContentController : Controller
    {
        private IContentRepository _contentRepository;
        private ICategoryRepository _categoryRepository;
        public ContentController(IContentRepository contentRepository,ICategoryRepository categoryRepository) {
            _contentRepository = contentRepository;
            _categoryRepository = categoryRepository;
        }

        public ContentController()
            : this(new ContentRepositoryEF(),new CategoryRepositoryEF()) {
        }

        [Authorize(Roles="admin")]
        public ActionResult AdminIndex()
        {
            IQueryable contentList = _contentRepository.All;
            return View(contentList);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create() {
            ViewBag.categoryData = _categoryRepository.GetListByIDCategoryType((int)Enums.enmCategoryTypes.Content);
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create(tContent contentData) {
            if (ModelState.IsValid)
            {
                contentData.DateAdded = DateTime.Now;
                _contentRepository.InsertOrUpdate(contentData);
                _contentRepository.Save();
                return Json(new {
                    objectAddedName = contentData.Name,
                    valid = true,
                    Message = "Content was added Succesfully"
                });
            } else {
                return Json(new {
                    objectAddedName = "",
                    valid = false,
                    Message = "Fill All Fields please"
                });
            }
        }

        public ActionResult Edit(int id) {
            tContent currentCat = _contentRepository.Find(id);
            ViewBag.categoryData = _categoryRepository.GetListByIDCategoryType((int)Enums.enmCategoryTypes.Content);
            return View(currentCat);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteContent(int id) {
            _contentRepository.Delete(id);
            _contentRepository.Save();
            return Json(new {
                valid = true,
                Message = "Content was deleted Succesfully",
                redirect = "/Content/AdminIndex"
            });
        }

        [ChildActionOnly]
        public ActionResult GetContentForMainSitePartial(string Code)
        {
            tContent contentData = _contentRepository.FindByCode(Code);
            if(contentData == null)
            {
                contentData = new tContent {Name = "Not Found", Description = "Not Found"};
            }
            return PartialView(contentData);
        }

        public ActionResult ContentDetails(int id) {
            tContent contentData = _contentRepository.Find(id);
            if (contentData == null) {
                contentData = new tContent { Name = "Not Found", Description = "Not Found" };
            }
            ViewBag.OtherContent = _contentRepository.GetListByIDCategory(contentData.IDCategory).Where(t => t.ShowInMenuFlag == true).ToList();
            return View(contentData);
        }

        [ChildActionOnly]
        public ActionResult ContentsByCategoryMenu(int IDCategory)
        {
            List<tContent> groupsMain = _contentRepository.GetListByIDCategory(IDCategory).Where(t=>t.ShowInMenuFlag == true).ToList();
            return PartialView(groupsMain);
        }

        public ActionResult ContentsByCategory(int? id)
        {
            if (id == null) id = 0;
            return View();
        }
    }
}
