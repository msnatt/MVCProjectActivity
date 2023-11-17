using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActivityProjectManage.Models;

namespace ActivityProjectManage.Controllers
{
    public class ProjectsController : Controller
    {
        private ProjectActivityEntities db = new ProjectActivityEntities();

        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.Projects.Where(s => s.IsDeleted == false);
            return View(projects);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            Project project = new Project();

            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                project.IsDeleted = false;
                project.CreateDate = DateTime.Now;
                project.UpdateDate = DateTime.Now;
                db.Projects.Add(project);

                SaveactivityToDB(project.Activities);

                db.SaveChanges();

            }
            return RedirectToAction("Index");

        }

        private void SaveactivityToDB(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                if (!(bool)item.IsDeleted)
                {
                    item.UpdateDate = DateTime.Now;
                    item.CreateDate = DateTime.Now;

                    db.Activities.Add(item);

                    if (item.Activity1.Count > 0)
                    {
                        SaveactivityToDB(item.Activity1);
                    }

                }
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            project.Activities = project.Activities.Where(s => s.Level == 0).ToList();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                project.UpdateDate = DateTime.Now;

                var x = project.Activities;
                project.Activities = null;
                UpdatectivityToDB(x, 0);

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(project);
        }
        private void UpdatectivityToDB(ICollection<Activity> activities, int HeaderID)
        {
            foreach (var item in activities)
            {
                if (HeaderID != 0)
                {
                    item.HeaderID = HeaderID;
                }

                var y = item.Activity1;
                item.Activity1 = null;
                if (item.ID == 0)
                {

                    item.CreateDate = DateTime.Now;
                    item.UpdateDate = DateTime.Now;
                    db.Activities.Add(item);
                }
                else
                {
                    item.UpdateDate = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;

                }
                db.SaveChanges();

                if (y.Count > 0)
                {
                    UpdatectivityToDB(y, item.ID);
                }
            }
        }


        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            project.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // =========== Index =============
        public ActionResult EditProject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            project.Activities = project.Activities.Where(s => s.Level == 0).ToList();

            project.isAddInEdit = true;

            return View("Edit", project);
        }
        // ============ End Index =============




        // ============= Add Activity =============

        // Create
        public ActionResult AddActivity(Project project)
        {
            Activity activity = new Activity();
            activity.ProjectID = project.ID;
            activity.Level = 0;
            activity.IsDeleted = false;
            project.Activities.Add(activity);


            return View("Create", project);
        }
        // Edit
        public ActionResult AddActivityEdit(Project project)
        {
            Activity activity = new Activity();
            activity.ProjectID = project.ID;
            activity.Level = 0;
            activity.IsDeleted = false;
            project.Activities.Add(activity);


            return View("Edit", project);
        }


        //=========== Delete ============

        //Edit
        public ActionResult DeleteActivityEdit(Project project)
        {
            ModelState.Clear();
            FindActivityForDelete(project.Activities);
            return View("Edit", project);
        }

        //Create
        public ActionResult DeleteActivity(Project project)
        {
            ModelState.Clear();
            FindActivityForDelete(project.Activities);
            return View("Create", project);
        }

        private void FindActivityForDelete(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                if (item.IsDeleted == true)
                {
                    foreach (var item2 in item.Activity1)
                    {
                        item2.IsDeleted = true;
                    }
                    FindActivityForDelete(item.Activity1);
                }
            }
        }

        // ================= Add SubActivity ==================

        //Edit
        public ActionResult AddSubActivityEdit(Project project)
        {

            FindActivity(project.Activities);
            return View("Edit", project);
        }

        //Create
        public ActionResult AddSubActivity(Project project)
        {
            FindActivity(project.Activities);
            return View("Create", project);
        }
        private void FindActivity(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                if (item.isaddActivity == true)
                {
                    var x = new Activity();
                    x.HeaderID = item.ID;
                    x.ProjectID = item.ProjectID;
                    x.Level = item.Level + 1;
                    x.IsDeleted = false;
                    item.Activity1.Add(x);
                }
                else
                {
                    FindActivity(item.Activity1);
                }
            }
        }
    }
}
