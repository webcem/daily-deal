﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aGrouponClasses.Models;
using aGrouponClasses.Repositories;
using aGrouponClasses.Utils;
using B2B.BLL;
using B2B.Models;
using System.ComponentModel.DataAnnotations;

namespace aGrouponProjectMain.Controllers {
    public class PartnerController : Controller {
        private readonly IUSERRepository _userRepository;
        private readonly IDealRepository _dealRepository;
        private readonly ICouponRepository _coupoRepository;

        public PartnerController(
            IUSERRepository userRepository, IDealRepository dealRepository, ICouponRepository couponRepository) {
            _userRepository = userRepository;
            _dealRepository = dealRepository;
            _coupoRepository = couponRepository;
        }

        public PartnerController()
            : this(new USERRepositoryEF(), new DealRepositoryEF(), new CouponRepositoryEF()) {
        }
        //
        // GET: /Partner/
        [Authorize(Roles = "Partner")]
        public ActionResult Index() {
            int UserID = (int)MembershipHelper.GetCurrenUser().ProviderUserKey;
            List<tDeal> dealsOfThisPartner = _dealRepository.GetListByIDPartner(UserID);
            return View(dealsOfThisPartner);
        }

        public ActionResult PartnerDealDetails(int? IDDeal) {
            IDDeal = IDDeal ?? 0;
            tDeal dealData = _dealRepository.Find((int)IDDeal);
            return View(dealData);
        }

        [Authorize(Roles = "Partner")]
        public ActionResult CouponsList() {
            int UserID = (int)MembershipHelper.GetCurrenUser().ProviderUserKey;
            List<tCoupon> coupons = _coupoRepository.GetListByIDPartnerUsed(UserID);
            return View(coupons);
        }

        [Authorize(Roles = "Partner")]
        public ActionResult CouponConsume() {
            ViewBag.SubmitButtonTest = "Consume";
            return View();
        }

        [Authorize(Roles = "Partner")]
        public ActionResult CouponVerify() {
            ViewBag.SubmitButtonTest = "Verify";
            return View();
        }

        [Authorize(Roles = "Partner")]
        [HttpPost]
        public ActionResult CouponConsume(CouponConsumeModel couponData) {
            ViewBag.SubmitButtonTest = "Consume";
            if (ModelState.IsValid) {
                //tCoupon coupon = _coupoRepository.FindByCustomNo(couponData.CouponNo);
                //if (coupon == null) {
                //    ModelState.AddModelError("Coupon", "Coupon Not Found!");
                //    return View(couponData);
                //}
                //int UserID = (int)MembershipHelper.GetCurrenUser().ProviderUserKey;
                //if (coupon.IDPartner != UserID) {
                //    ModelState.AddModelError("Coupon", "User is Wrong For this Coupon!");
                //    return View(couponData);
                //}
                //if (coupon.Code != couponData.CouponCode) {
                //    ModelState.AddModelError("Coupon", "Coupon Code Not Valid!");
                //    return View(couponData);
                //}
                tCoupon coupon = ValidateCoupon(couponData);
                if (!ModelState.IsValid)
                    return View(couponData);
                coupon.ConsumeStatus = (int)Enums.enmCouponConsumeStatus.Consumed;
                coupon.tOrders.FirstOrDefault().RefundStatus = (int)Enums.enmRefundStatus.OrderSuccessful;
                _coupoRepository.InsertOrUpdate(coupon);
                _coupoRepository.Save();
                ViewBag.CurrentMessage = "Coupon Consumed Succesfully!";
            }
            return View(couponData);
        }

        [Authorize(Roles = "Partner")]
        [HttpPost]
        public ActionResult CouponVerify(CouponConsumeModel couponData) {
            ViewBag.SubmitButtonTest = "Verify";
            if (ModelState.IsValid) {
                ValidateCoupon(couponData);
                if (!ModelState.IsValid)
                    return View(couponData);
                ViewBag.CurrentMessage = "Coupon Data is OK !";
            }
            return View(couponData);
        }

        private tCoupon ValidateCoupon(CouponConsumeModel couponData) {
            tCoupon coupon = _coupoRepository.FindByCustomNo(couponData.CouponNo);
            if (coupon == null) {
                ModelState.AddModelError("Coupon", "Coupon Not Found!");
                return coupon;
            }
            int UserID = (int)MembershipHelper.GetCurrenUser().ProviderUserKey;
            if (coupon.IDPartner != UserID) {
                ModelState.AddModelError("Coupon", "User is Wrong For this Coupon!");
                return coupon;
            }
            if (coupon.Code != couponData.CouponCode) {
                ModelState.AddModelError("Coupon", "Coupon Code Not Valid!");
                return coupon;
            }
            return coupon;
        }

        public ActionResult PartnersByCategory(int? IDCity) {
            if (IDCity == null) IDCity = 0;
            List<tUser> partners = _userRepository.GetListPartnerByIDCity((int)IDCity);
            return View(partners);
        }

        public ActionResult Details(int? id) {
            if (id == null) id = 0;
            tUser userData = _userRepository.Find((int)id);

            ViewBag.DealsByPartner = _dealRepository.GetListByIDPartner((int) id);
            return View(userData);
        }

    }

    public class CouponConsumeModel {
        [Required]
        public string CouponNo { get; set; }
        public string CouponCode { get; set; }
    }
}
