using AlphaApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;

namespace AlphaApplication.Controllers
{
    public class HomeController : Controller
    {
        MeetingRoomContext mrc = new MeetingRoomContext();
        [Authorize]
        public ActionResult Index()
        {
            IList<string> roles = new List<string>();
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
                roles = userManager.GetRoles(user.Id);
            ViewBag.IsOfficeManager = false;
            foreach (var role in roles)
                if (role == "OFFICEMANAGER")
                {
                    ViewBag.IsOfficeManager = true;
                    break;
                }
            var rooms = mrc.MeetingRooms;
            ViewBag.MeetingRooms = rooms;
            var rr = mrc.RoomReservations;
            ViewBag.RoomReservations = rr;
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            ViewBag.Users = users;
            return View();
        }

        [Authorize]
        public ActionResult RoomDescription(int Id = -1)
        {
            if (Id == -1)
                foreach (var mr in mrc.MeetingRooms.ToList())
                {
                    ViewBag.RoomId = mr.Id;
                    break;
                }
            else
                ViewBag.RoomId = Id;
            var rooms = mrc.MeetingRooms;
            ViewBag.MeetingRooms = rooms;
            var reservations = mrc.RoomReservations;
            ViewBag.RoomReservations = reservations;
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            ViewBag.Users = users;
            IList<string> roles = new List<string>();
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
                roles = userManager.GetRoles(user.Id);
            ViewBag.IsOfficeManager = false;
            foreach (var role in roles)
                if (role == "OFFICEMANAGER")
                {
                    ViewBag.IsOfficeManager = true;
                    break;
                }
            return View();
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        public ActionResult AddRoom()
        {
            ViewBag.inscription = "";
            return View();
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        [HttpPost]
        public ActionResult AddRoom(MeetingRoom meetingroom)
        {
            if (meetingroom.NumberRoom <= 0)
            {
                ViewBag.inscription = "Неправильно введен номер комнаты";
                return View();
            }
            foreach (var r in mrc.MeetingRooms)
                if (r.NumberRoom == meetingroom.NumberRoom)
                {
                    ViewBag.inscription = "Комната с таким номером существует";
                    return View();
                }
            if (meetingroom.CountSeats <= 0)
            {
                ViewBag.inscription = "Неправильно указано количество сидений";
                return View();
            }
            ViewBag.inscription = "Комната №" + meetingroom.NumberRoom + " успешно создана";
            mrc.MeetingRooms.Add(meetingroom);
            mrc.SaveChanges();
            return View();
        }

        [Authorize]
        public ActionResult ChooseData(int Id = -1)
        {
            ViewBag.error = "";
            if (Id == -1)
                foreach (var mr in mrc.MeetingRooms.ToList())
                {
                    ViewBag.RoomId = mr.Id;
                    break;
                }
            else
                ViewBag.RoomId = Id;
            ViewBag.UserId = User.Identity.GetUserId();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChooseData(RoomReservation roomReservation)
        {
            roomReservation.Confirmation = false;
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.RoomId = roomReservation.RoomId;
            if (roomReservation.TimeStart < DateTime.Now)
            {
                ViewBag.error = "Неккоректное начальное время";
                return View();
            }
            if (roomReservation.TimeStart > roomReservation.TimeEnd)
            {
                ViewBag.error = "Неккоректно введены данные";
                return View();
            }
            foreach (var rr in mrc.RoomReservations.ToList())
            {
                if (roomReservation.RoomId == rr.RoomId && rr.Confirmation)
                    if (!((roomReservation.TimeStart < rr.TimeStart && roomReservation.TimeEnd <= rr.TimeStart)
                        || (roomReservation.TimeStart >= rr.TimeEnd && roomReservation.TimeEnd > rr.TimeEnd)))
                    {
                        ViewBag.error = "Данное время занято";
                        return View();
                    }
            }
            mrc.RoomReservations.Add(roomReservation);
            mrc.SaveChanges();
            return RedirectToAction("RoomDescription", "Home", roomReservation.RoomId);
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        [HttpPost]
        public PartialViewResult Confirm(int Id=0)
        {
            var rooms = mrc.MeetingRooms;
            ViewBag.MeetingRooms = rooms;
            var roomReservations = mrc.RoomReservations;
            ViewBag.RoomReservations = roomReservations;
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            ViewBag.Users = users;
            if (Id == 0)
            {
                ViewBag.message = "";
                return PartialView();
            }
            foreach (var rr in mrc.RoomReservations.ToList())
            {

                if (rr.Id == Id && !(rr.Confirmation))
                {
                    foreach (var roomReservation in mrc.RoomReservations.ToList())
                        if (rr.RoomId == roomReservation.RoomId && roomReservation.Confirmation &&
                            !((rr.TimeStart < roomReservation.TimeStart && rr.TimeEnd <= roomReservation.TimeStart) ||
                            (rr.TimeStart >= roomReservation.TimeEnd && rr.TimeEnd > roomReservation.TimeEnd)))
                        {
                            ViewBag.message = "Данное время забронировано";
                            return PartialView();
                        }
                    rr.Confirmation = true;
                    break;
                }
            }
            mrc.SaveChanges();
            ViewBag.message = "Успешно добавлено";
            return PartialView();
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        [HttpPost]
        public PartialViewResult Delete(int Id=0)
        {
            var rooms = mrc.MeetingRooms;
            ViewBag.MeetingRooms = rooms;
            var roomReservations = mrc.RoomReservations;
            ViewBag.RoomReservations = roomReservations;
            List<ApplicationUser> users = new List<ApplicationUser>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                users = db.Users.ToList();
            }
            ViewBag.Users = users;
            ViewBag.message = "Заявка успешно удалена";
            RoomReservation roomReservation = new RoomReservation();
            foreach (var rr in mrc.RoomReservations)
            {
                if (rr.Id == Id)
                {
                    roomReservation = rr;
                    break;
                }
            }
            mrc.RoomReservations.Remove(roomReservation);
            mrc.SaveChanges();
            return PartialView();
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        public ActionResult InformationEditing(int Id = -1)
        {
            MeetingRoom meetingRoom = null;
            ViewBag.RoomId = Id;
            ViewBag.inscription = "";
            foreach (var mr in mrc.MeetingRooms.ToList())
            {
                if (Id == -1)
                {
                    meetingRoom = mr;
                    break;
                }
                if (mr.Id == Id)
                {
                    meetingRoom = mr;
                    break;
                }
                meetingRoom = mr;
            }
            return View(meetingRoom);
        }

        [Authorize(Roles = "OFFICEMANAGER")]
        [HttpPost]
        public ActionResult InformationEditing(MeetingRoom meetingRoom)
        {
            ViewBag.RoomId = meetingRoom.Id;
            if (meetingRoom.NumberRoom <= 0)
            {
                ViewBag.inscription = "Неправильно введен номер комнаты";
                return View(meetingRoom);
            }
            if (meetingRoom.CountSeats <= 0)
            {
                ViewBag.inscription = "Неправильно указано количество сидений";
                return View(meetingRoom);
            }
            ViewBag.inscription = "Комната №" + meetingRoom.NumberRoom + " успешно отредактирована";
            mrc.Entry(meetingRoom).State = EntityState.Modified;
            mrc.SaveChanges();
            return View(meetingRoom);
        }
        [Authorize(Roles = "OFFICEMANAGER")]
        public ActionResult AddManager(string UserEmail = null)
        {
            ViewBag.inscription = "";
            IList<string> roles = new List<string>();
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            List<string> NameUsers = new List<string>();
            if (UserEmail != null)
            {
                bool check = true;
                bool checkrole = true;
                foreach (var users in userManager.Users.ToList())
                    if (UserEmail == users.Email)
                    {
                        roles = userManager.GetRoles(users.Id);
                        foreach (var role in roles.ToList())
                            if (role == "OFFICEMANAGER")
                            {
                                checkrole = false;
                                break;
                            }
                        if (!checkrole)
                            break;
                        userManager.AddToRole(users.Id, "OFFICEMANAGER");
                        ViewBag.inscription = "Пользователь " + users.Email + " теперь является менеджером";
                        check = false;
                        break;
                    }
                if(!checkrole)
                    ViewBag.inscription = "Этот пользователь уже является менеджером";
                else if(check)
                    ViewBag.inscription = "Такого пользователя не существует";
            }
            foreach (var users in userManager.Users.ToList())
            {

                roles = userManager.GetRoles(users.Id);
                bool check = false;
                foreach (var role in roles.ToList())
                    if (role == "OFFICEMANAGER")
                    {
                        check = true;
                        break;
                    }
                if (!check)
                    NameUsers.Add(users.Email.ToString());
            }
            ViewBag.NameUsers = NameUsers;
            return View();
        }
    }
}