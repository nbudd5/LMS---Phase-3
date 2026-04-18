using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo("LMSControllerTests")]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var students =
                from c in db.Courses
                join cl in db.Classes on c.CourseId equals cl.CourseId
                join e in db.EnrollmentGrades on cl.ClassId equals e.ClassId
                join s in db.Students on e.UId equals s.UId
                where c.Abbreviation == subject && c.CNumber == num
                && cl.SemesterSeason == season && cl.SemesterYear == year
                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    dob = s.Dob,
                    grade = e.Grade,
                };

            return Json(students.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var assignments =
                from c in db.Courses
                join cl in db.Classes on c.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.AcId equals a.AcId
                where c.Abbreviation == subject && c.CNumber == num &&
                cl.SemesterSeason == season && cl.SemesterYear == year &&
                (ac.AcName == category || category == null)
                select new
                {
                    aname = a.AName,
                    cname = ac.AcName,
                    due = a.DueDate,
                    submissions = (from s in db.Submissions
                                   where s.AId == a.AId
                                   select s).Count()
                };

            return Json(assignments.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var asCategories =
                from c in db.Courses
                join cl in db.Classes on c.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                where c.Abbreviation == subject && c.CNumber == num
                && cl.SemesterSeason == season && cl.SemesterYear == year
                select new
                {
                    name = ac.AcName,
                    weight = ac.GradingWeight
                };

            return Json(asCategories.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            if (string.IsNullOrWhiteSpace(category) || catweight < 0 || catweight > 100) // maybe check for catwight
                return Json(new { success = false });

            bool catExists =
                (from c in db.Courses
                 join cl in db.Classes on c.CourseId equals cl.CourseId
                 join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                 where c.Abbreviation == subject && c.CNumber == num
                 && cl.SemesterSeason == season && cl.SemesterYear == year
                 && ac.AcName == category
                 select c).Any();

            if (catExists)
                return Json(new { success = false });

            AssignmentCategory assignmentCat = new AssignmentCategory();
            assignmentCat.GradingWeight = (byte)catweight;
            assignmentCat.AcName = category;
            assignmentCat.ClassId =
                (from c in db.Courses
                 join cl in db.Classes on c.CourseId equals cl.CourseId
                 where c.Abbreviation == subject && c.CNumber == num
                 && cl.SemesterSeason == season
                 select cl.ClassId).First();
            db.AssignmentCategories.Add(assignmentCat);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            if (string.IsNullOrWhiteSpace(asgname))
                return Json(new { success = false });

            bool asgExists =
               (from c in db.Courses
                join cl in db.Classes on c.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.AcId equals a.AcId
                where c.Abbreviation == subject && c.CNumber == num
                && cl.SemesterSeason == season && cl.SemesterYear == year
                && ac.AcName == category && a.AName == asgname
                select c).Any();

            if (asgExists)
                return Json(new { success = false });

            var classData =
                (from c in db.Courses
                 join cl in db.Classes on c.CourseId equals cl.CourseId
                 join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                 where c.Abbreviation == subject && c.CNumber == num && cl.SemesterSeason == season 
                 && cl.SemesterYear == year && ac.AcName == category
                 select new { ac.AcId, cl.ClassId }).First();

            Assignment assignment = new Assignment();
            assignment.AName = asgname;
            assignment.MaxPointValue = (uint)asgpoints;
            assignment.Contents = asgcontents;
            assignment.DueDate = asgdue;
            assignment.AcId = classData.AcId;
            db.Assignments.Add(assignment);

            var allEnrollments = (from e in db.EnrollmentGrades
                                  where e.ClassId == classData.ClassId
                                  select e).ToList();

            foreach (var enrollment in allEnrollments)
            {
                var studentAssignments = (from s in db.Students
                                          join e in db.EnrollmentGrades on s.UId equals e.UId
                                          join cl in db.Classes on e.ClassId equals cl.ClassId
                                          join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                                          join a in db.Assignments on ac.AcId equals a.AcId
                                          where s.UId == enrollment.UId && cl.ClassId == classData.ClassId
                                          join sub in db.Submissions on a.AId equals sub.AId into subs
                                          from currSub in subs.DefaultIfEmpty()
                                          select new
                                          {
                                              catName = ac.AcId,
                                              weight = ac.GradingWeight,
                                              score = currSub == null ? 0 : currSub.Score,
                                              maxScore = a.MaxPointValue
                                          }).ToList();

                UpdateGrade(enrollment, studentAssignments);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var asgSubmissions =
                from c in db.Courses
                join cl in db.Classes on c.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.AcId equals a.AcId
                join sub in db.Submissions on a.AId equals sub.AId
                join s in db.Students on sub.UId equals s.UId
                where c.Abbreviation == subject && c.CNumber == num
                && cl.SemesterSeason == season && cl.SemesterYear == year
                && ac.AcName == category && a.AName == asgname
                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    time = sub.SubmissionTime,
                    score = sub.Score
                };

            return Json(asgSubmissions.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var submission =
                (from c in db.Courses
                 join cl in db.Classes on c.CourseId equals cl.CourseId
                 join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                 join a in db.Assignments on ac.AcId equals a.AcId
                 join sub in db.Submissions on a.AId equals sub.AId
                 join s in db.Students on sub.UId equals s.UId
                 where c.Abbreviation == subject && c.CNumber == num
                 && cl.SemesterSeason == season && cl.SemesterYear == year
                 && ac.AcName == category && a.AName == asgname && s.UId == uid
                 select new
                 {
                     Submission = sub,
                     ClassId = cl.ClassId
                 }).First();

            submission.Submission.Score = (uint)score;

            var assignments =
                (from s in db.Students
                 join e in db.EnrollmentGrades on s.UId equals e.UId
                 join cl in db.Classes on e.ClassId equals cl.ClassId
                 join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                 join a in db.Assignments on ac.AcId equals a.AcId
                 where s.UId == uid && cl.ClassId == submission.ClassId
                 join sub in db.Submissions on a.AId equals sub.AId into subs
                 from currSub in subs.DefaultIfEmpty()
                 select new
                 {
                     catName = ac.AcId,
                     weight = ac.GradingWeight,
                     score = currSub == null ? 0 : currSub.Score,
                     maxScore = a.MaxPointValue
                 }).ToList();

            var enrollment = (from e in db.EnrollmentGrades
                              where e.UId == uid && e.ClassId == submission.ClassId
                              select e).First();

            UpdateGrade(enrollment, assignments);

            db.SaveChanges();

            return Json(new { success = true });
        }

        private void UpdateGrade(EnrollmentGrade enrollment, IEnumerable<dynamic> assignments)
        {
            var agnList = assignments.ToList();
            if (agnList.Count == 0)
            {
                enrollment.Grade = "--";
                return;
            }

            double totalScaledScore = 0;
            double sumOfWeights = 0;


            var categoryIds = (from d in agnList select d.catName).Distinct().ToList();

            foreach (var catId in categoryIds)
            {
                double catEarned = 0;
                double catMax = 0;
                int catWeight = 0;

                foreach (var row in agnList)
                {
                    if (row.catName == catId)
                    {
                        catEarned += (double)row.score;
                        catMax += (double)row.maxScore;
                        catWeight = row.weight;
                    }
                }

                if (catMax > 0)
                {
                    totalScaledScore += (catEarned / catMax) * catWeight;
                    sumOfWeights += catWeight;
                }
            }

            if (sumOfWeights > 0)
            {
                double finalPercentage = totalScaledScore * (100.0 / sumOfWeights);
                enrollment.Grade = GetLetterGrade(finalPercentage);
            }
        }
        private string GetLetterGrade(double X)
        {
            if (X >= 93) return "A";
            else if (X >= 90) return "A-";
            else if (X >= 87) return "B+";
            else if (X >= 83) return "B";
            else if (X >= 80) return "B-";
            else if (X >= 77) return "C+";
            else if (X >= 73) return "C";
            else if (X >= 70) return "C-";
            else if (X >= 67) return "D+";
            else if (X >= 63) return "D";
            else if (X >= 60) return "D-";
            else return "E";
        }

        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var classes =
                from p in db.Professors
                join cl in db.Classes on p.UId equals cl.ProfId
                join c in db.Courses on cl.CourseId equals c.CourseId
                where p.UId == uid
                select new
                {
                    subject = c.Abbreviation,
                    number = c.CNumber,
                    name = c.CName,
                    season = cl.SemesterSeason,
                    year = cl.SemesterYear
                };
            return Json(classes.ToArray());
        }



        /*******End code to modify********/
    }
}

