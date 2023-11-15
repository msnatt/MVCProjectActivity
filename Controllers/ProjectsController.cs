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
            //return View(project);
            return RedirectToAction("Index");

        }

        private void SaveactivityToDB(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                item.CreateDate = DateTime.Now;
                item.UpdateDate = DateTime.Now;
                item.IsDeleted = false;
                db.Activities.Add(item);

                if (item.Activity1 == null)
                {

                }
                else
                {
                    SaveactivityToDB(item.Activity1);
                }
            }
            //db.SaveChanges();
        }

        // GET: Projects/Edit/5
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
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProjectName,StartProjectDate,EndProjectDate,CreateDate,UpdateDate,IsDeleted")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.UpdateDate = DateTime.Now;

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            //db.Projects.Remove(project);
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
        public ActionResult AddActivity(Project project)
        {
            Activity activity = new Activity();
            activity.ProjectID = project.ID;
            project.Activities.Add(activity);


            return View("Create", project);
        }
        public ActionResult EditActivity(Project project)
        {
            return View("Create", project);
        }
        public ActionResult DeleteActivity(Project project)
        {
            FindActivityForDelete(project.Activities);
            return View("Create", project);
        }

        private void FindActivityForDelete(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                if (item.isdeleteActivity == true)
                {
                    item.IsDeleted = true;
                    foreach (var item2 in item.Activity1)
                    {
                        item2.IsDeleted = true;
                    }
                }
                else
                {
                    NextFindActivity(item.Activity1);
                }
            }
        }

        public ActionResult AddSubActivity(Project project)
        {
            NextFindActivity(project.Activities);
            return View("Create", project);
        }
        private void NextFindActivity(ICollection<Activity> activities)
        {
            foreach (var item in activities)
            {
                if (item.isaddActivity == true)
                {
                    var x = new Activity();
                    x.HeaderID = item.ID;
                    x.ProjectID = item.ProjectID;
                    item.Activity1.Add(x);
                }
                else
                {
                    NextFindActivity(item.Activity1);
                }
            }
        }
    }
}
