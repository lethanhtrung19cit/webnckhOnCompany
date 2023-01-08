using DuAnQLNCKH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
 
namespace DuAnQLNCKH.Controllers
{
    public class TopicOfLectureController : Controller
    {
        // GET: DeTaiGV
        DHTDTTDNEntities1 qLNCKHDHTDTD = new DHTDTTDNEntities1();
        TopicOfLectureModel dtgv = new TopicOfLectureModel();
        List<TopicOfStudent> studentList = new List<TopicOfStudent>();
        List<Subject> subjects = new DHTDTTDNEntities1().Subjects.ToList();
        List<Extension> extensions = new DHTDTTDNEntities1().Extensions.ToList();
        List<TopicOfLecture> topicOfLectures = new DHTDTTDNEntities1().TopicOfLectures.ToList();
        List<TopicOfStudent> topicOfStudents = new DHTDTTDNEntities1().TopicOfStudents.ToList();
        List<Information> information = new DHTDTTDNEntities1().Information.ToList();
        List<PointTable> pointTables = new DHTDTTDNEntities1().PointTables.ToList();
        List<Faculty> faculties = new DHTDTTDNEntities1().Faculties.ToList();
        List<ProgressLe> progressLes = new DHTDTTDNEntities1().ProgressLes.ToList();
         List<Author> authors = new DHTDTTDNEntities1().Authors.ToList();
        List<Models.Type> types = new DHTDTTDNEntities1().Types.ToList();
         
        public ActionResult getTypeList(int IdTy)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            List<PointTable> DetailList = qLNCKHDHTDTD.PointTables.Where(x => x.IdP == IdTy).ToList();
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "2")]
        [HttpPost]
        public ActionResult editInfo(Information model)
        {

            var data = qLNCKHDHTDTD.Information.Find(model.Email);            
            data.NameLe = model.NameLe;
            data.Phone = model.Phone;
            data.Email = model.Email;
            data.DateOfBirth = model.DateOfBirth;
            data.Address = model.Address;
            data.CMND = model.CMND;
            qLNCKHDHTDTD.Entry(data).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();

            return RedirectToAction("listInformation");
          

        }
        [Authorize(Roles = "2")]
        public ActionResult Index()
        {                
                var topicofLecture = (from t in topicOfLectures
                                      join a in authors on t.IdTp equals a.IdTp
                                      join i in information on a.Email equals i.Email
                                      join f in faculties on t.IdFa equals f.IdFa
                                      join pr in progressLes on t.IdTp equals pr.IdTp
                                      select new TopicOfLectureView
                                      {
                                          author=a,
                                           topicOfLecture = t,
                                          information = i,
                                           faculty=f,
                                          progressLe=pr
                                      }).ToList();
                ViewBag.TopicOfLecture = topicofLecture;
                var topics = (from t in topicOfStudents                              
                              join s in subjects on t.IdSu equals s.IdSu
                               select new TopicOfStudentView
                              {

                                  topicOfStudent = t,
                                  subject=s
                                 
                              }).ToList();
                ViewBag.listTopicOfStudent = topics;
            
          
            return View();

        }
        [Authorize(Roles = "1")]
        public ActionResult listAcceptanced()
        {                
            var topicofLecture = (from t in topicOfLectures
                                  join ty in types on t.IdType equals ty.IdType
                                     where t.Status==4 
                                    select new TopicOfLectureView
                                    {
                                        topicOfLecture = t,
                                        type=ty,
                                         
                                    }).ToList();
            ViewBag.listAcceptancedLec = topicofLecture;
            var topics = (from t in topicOfStudents
                             join s in subjects on t.IdSu equals s.IdSu
                             select new TopicOfStudentView
                            {                                 
                                topicOfStudent = t,
                                subject=s,                                
                            }).ToList();
            ViewBag.listAcceptancedStu = topics;                      
            return View();
        }
        public void dataMyTopic()
        {            
            var topicPending = (from t in topicOfLectures
                         join ty in types on t.IdType equals ty.IdType
                          join a in authors on t.IdTp equals a.IdTp
                         join i in information on a.Email equals i.Email
                          where i.Email == Session["UserName"].ToString() && (t.Status == 0 || t.Status == 3) 
                         select new TopicOfLectureView
                         {                              
                             topicOfLecture = t,
                             type=ty 
                         }).ToList();
            ViewBag.topicPending = topicPending;
            var topic = (from t in topicOfLectures
                         join ty in types on t.IdType equals ty.IdType
                         join a in authors on t.IdTp equals a.IdTp
                         join i in information on a.Email equals i.Email
                         where i.Email == Session["UserName"].ToString() && t.Status == 1
                         select new TopicOfLectureView
                         {                              
                             topicOfLecture = t,
                             type=ty 
                         }).ToList();
            ViewBag.topicProgress = topic;
            var topic1 = (from t in topicOfLectures
                          join ty in types on t.IdType equals ty.IdType
                          join a in authors on t.IdTp equals a.IdTp
                          join i in information on a.Email equals i.Email
                           where i.Email == Session["UserName"].ToString() && t.Status == 4
                          select new TopicOfLectureView
                          {
                              topicOfLecture = t,
                              type=ty
                          }).ToList();
            ViewBag.topicEnd = topic1;
        }
        [Authorize(Roles = "2")]
        public ActionResult myTopicLecture()
        {
            dataMyTopic();
            return View();
             
        }
        [HttpPost]
        public void ShowIdP(string id)
        {
            ViewBag.idp = id;
            
        }
        [HttpPost]
        [Authorize(Roles = "2")]
        public ActionResult CreateTopicOfLecture(HttpPostedFileBase FileDemo1, TopicOfLecture topicOfLecture, List<string> email, List<string> nameAu, List<int> Hours, byte typeRegister, int IdType=0, int HourAdmin=0, string NameAdmin=null, string EmailAdmin=null, int HourAu=0)
        {
            string IdTp = dtgv.IdTp();
            
             if (ModelState.IsValid)
             {
                 string username = Session["UserName"].ToString();
                TopicOfLectureModel topic = new TopicOfLectureModel();
                 
                if (typeRegister == 2)
                {
                    topicOfLecture.Status = 0;
                }
                else
                {
                    
                   topicOfLecture.Status = 3;
                }    
                topicOfLecture.IdTp = IdTp;
                topicOfLecture.IdType = IdType;
                if (FileDemo1!=null)
                {
                    var Extension = Path.GetExtension(FileDemo1.FileName);
                    var fileName = "fileTopic-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                    string path = Path.Combine(Server.MapPath("~/File/FileTopic"), fileName);
                    topicOfLecture.FileDemo = Url.Content(Path.Combine("~/File/FileTopic/", fileName));
                    FileDemo1.SaveAs(path);
                }
                topic.AddTopicLecture(topicOfLecture);

                ViewBag.Message = "Đăng ký công trình thành công";
                ViewBag.listtype = new SelectList(types, "IdType", "NameType");
                
                Author author = new Author();
                if (EmailAdmin==null)
                {
                    EmailAdmin= Session["UserName"].ToString();                   
                    NameAdmin = qLNCKHDHTDTD.Information.Where(x => x.Email == EmailAdmin).Select(x => x.NameLe).FirstOrDefault();
                    HourAdmin = HourAu;
                }
                else
                {
                    if (email == null)
                    {
                        email = new List<string>();
                        nameAu = new List<string>();
                        Hours = new List<int>();
                    }
                    var emailA = Session["UserName"].ToString();
                    email.Add(emailA);
                    string name = qLNCKHDHTDTD.Information.Where(x => x.Email == emailA).Select(x => x.NameLe).FirstOrDefault().ToString();
                    nameAu.Add(name);
                    Hours.Add(HourAu);
                }
                author.Email = EmailAdmin;
                author.NameAu = NameAdmin;
                author.IdTp = IdTp;
                author.Grade = 0;
                qLNCKHDHTDTD.Authors.Add(author);
                qLNCKHDHTDTD.SaveChanges();
                if (email!=null)
                {
                    for (int i = 0; i < email.Count; i++)
                    {
                        author.Email = email[i];
                        author.NameAu = nameAu[i];
                        author.IdTp = IdTp;
                        author.Grade = 1;
                        qLNCKHDHTDTD.Authors.Add(author);
                        qLNCKHDHTDTD.SaveChanges();

                    }
                }    
                
                qLNCKHDHTDTD.SaveChanges();
                PointTable pointTable = new PointTable();
                pointTable.IdAu = qLNCKHDHTDTD.Authors.Where(x => x.IdTp == IdTp && x.Grade==0).Select(x => x.IdAu).FirstOrDefault();
                pointTable.IdTp = IdTp;
                pointTable.Hours = HourAdmin;
                pointTable.Status = typeRegister;
                qLNCKHDHTDTD.PointTables.Add(pointTable);
                qLNCKHDHTDTD.SaveChanges();
                if (nameAu!=null)
                {
                    for (int i = 0; i < nameAu.Count; i++)
                    {
                        var emailAu = email[i];
                        pointTable.IdAu = qLNCKHDHTDTD.Authors.Where(x => x.Email == emailAu && x.IdTp == IdTp).Select(x => x.IdAu).FirstOrDefault();
                        pointTable.IdTp = IdTp;
                        pointTable.Hours = Hours[i];
                        pointTable.Status = typeRegister;
                        qLNCKHDHTDTD.PointTables.Add(pointTable);
                        qLNCKHDHTDTD.SaveChanges();
                    }
                }
                ViewBag.listtype = new SelectList(types, "IdType", "NameType");
                ViewBag.listFaculty = new SelectList(faculties, "IdFa", "Name");
                return View("ViewCreateTopicOfLecture");

             }

             ViewBag.listFaculty = new SelectList(faculties, "IdFa", "Name");
            return View("ViewCreateTopicOfLecture", topicOfLecture);
           
        }
        [Authorize(Roles = "2")]
        public ActionResult ViewCreateTopicOfLecture()
        {            
            string email = Session["UserName"].ToString();
            ViewBag.NameLe = qLNCKHDHTDTD.Information.Where(x => x.Email == email).Select(x=>x.NameLe).FirstOrDefault();
            ViewBag.Message = "";
            ViewBag.listtype = new SelectList(types, "IdType", "NameType");
            List<Faculty> faculties = qLNCKHDHTDTD.Faculties.ToList();
            ViewBag.listFaculty = new SelectList(faculties, "IdFa", "Name");
            return View();
        }
        public ActionResult getPoint(int IdDetail, byte Level, int IdChild=0)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            int Hour = 0;
            if (Level==0)
                Hour = int.Parse(qLNCKHDHTDTD.DetailType1.Where(x => x.IdDetail == IdDetail).Select(x => x.Hours).FirstOrDefault().ToString());           
            else
                Hour = int.Parse(qLNCKHDHTDTD.ChildDetails.Where(x => x.IdChild == IdChild).Select(x => x.Hours).FirstOrDefault().ToString());
             return Json(Hour, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPointMember(int IdDetail)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            int Hour = int.Parse(qLNCKHDHTDTD.ChildDetails.Where(x => x.IdDetail == IdDetail).OrderBy(x=>x.Hours).Select(x => x.Hours).FirstOrDefault().ToString());
             return Json(Hour, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult getHour(string IdTp)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            var pointTable1 = (from p in pointTables
                                                   join a in authors on p.IdAu equals a.IdAu
                                                   where p.IdTp == IdTp
                                                   orderby a.Grade
                                                   select new 
                                                   {
                                                       a.NameAu,
                                                       p.Hours,
                                                       a.IdAu
                                                   }
                                                   ).ToList();
 
            return Json(pointTable1, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult getDetailTypes(int IdType)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            List<DetailType1> DetailList = qLNCKHDHTDTD.DetailType1.Where(x => x.IdType == IdType).ToList();            
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getLevel(int IdType)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            int Level = byte.Parse(qLNCKHDHTDTD.Types.Where(x => x.IdType == IdType).Select(x => x.Level).FirstOrDefault().ToString());
            return Json(Level, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDetailChilds(int IdDetail)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            List<ChildDetail> ChildList = qLNCKHDHTDTD.ChildDetails.Where(x => x.IdDetail == IdDetail).ToList();
            return Json(ChildList, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "1")]
        public ActionResult chuaduyet()
        {             
            var topicofLecture = (from t in topicOfLectures
                                    join ty in types on t.IdType equals ty.IdType
                                    where t.Status == 0
                                    select new TopicOfLectureView
                                    {
                                        topicOfLecture = t,
                                        type=ty
                                    }).ToList();
            ViewBag.TopicOfLecture = topicofLecture;                                        
            return View();
         
        }
        [Authorize(Roles = "2")]
        public ActionResult detailTopicLecture(string IdTp)
        {
            var listDetail = (from t in topicOfLectures
                              join ty in types on t.IdType equals ty.IdType
                              join a in authors on t.IdTp equals a.IdTp
                              join i in information on a.Email equals i.Email
                              join f in faculties on t.IdFa equals f.IdFa
                              where t.IdTp==IdTp
                              select new TopicOfLectureView
                              {
                                  topicOfLecture = t,
                                  information = i,
                                  faculty = f,
                                  author=a,
                                  type=ty
                              }
                            ).ToList();
            ViewBag.listDetail = listDetail;
            return View();
        }
        [Authorize(Roles = "1")]
        public void rejectTopic(string IdTp)
        {             
            var topic = qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
            topic.Status = 2;            
            qLNCKHDHTDTD.Entry(topic).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();                           
        }//
        [Authorize(Roles = "1")]
        public ActionResult detailTopicSt(string IdTp)
        {
            var listDetail = (from t in topicOfStudents
                               join s in subjects on t.IdSu equals s.IdSu
                              where t.IdTp==IdTp
                              select new TopicOfLectureView
                              {
                                  topicOfStudent = t,                                  
                                  subject = s
                               }
                            ).ToList();
            ViewBag.listDetail = listDetail;
            return View();
        }         
        [HttpPost]
        [Authorize(Roles = "1")]
        public void xetduyet2(string IdTp)
        {            
            string a = IdTp;                
            var t = qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
            t.Status = 1;
            qLNCKHDHTDTD.Entry(t).State = EntityState.Modified;                
            qLNCKHDHTDTD.SaveChanges();      
        }
        [Authorize(Roles = "2")]
        public ActionResult Register1(string IdTp, int HourAdmin, int[] Hours, int[] IdAu, HttpPostedFileBase FileDemo1)
        {                                      
            var topic = qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
            topic.Status = 3;
            var Extension = Path.GetExtension(FileDemo1.FileName);
            var fileName = "fileTopic-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
            string path = Path.Combine(Server.MapPath("~/File/FileTopic"), fileName);
            topic.FileDemo = Url.Content(Path.Combine("~/File/FileTopic/", fileName));
            FileDemo1.SaveAs(path);
            qLNCKHDHTDTD.Entry(topic).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();
            string email = Session["UserName"].ToString();
            int IdAuAdmin =int.Parse(qLNCKHDHTDTD.Authors.Where(x => x.Email == email && x.IdTp == IdTp).Select(x=>x.IdAu).FirstOrDefault().ToString());
            PointTable pA = qLNCKHDHTDTD.PointTables.Where(x=>x.IdTp == IdTp && x.IdAu==IdAuAdmin).FirstOrDefault();
            pA.Hours = HourAdmin;
            qLNCKHDHTDTD.SaveChanges();
            if (Hours!=null)
            {
                for (int i = 0; i < Hours.Length; i++)
                {
                    int Au = IdAu[i];
                    int Hour = Hours[i];
                    PointTable p = qLNCKHDHTDTD.PointTables.Where(x => x.IdAu == Au && x.IdTp == IdTp).FirstOrDefault();
                    p.Hours = Hour;
                    qLNCKHDHTDTD.SaveChanges();
                }
            }    
            
             
            return RedirectToAction("myTopicLecture");
        }
        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        [Authorize(Roles = "2")]
        public ActionResult DeleteTopicLecture(string IdTp)
        {
            TopicOfLecture topic = (from c in qLNCKHDHTDTD.TopicOfLectures
                                    where c.IdTp == IdTp
                                    select c).FirstOrDefault();
            topic.Status = 5;
            qLNCKHDHTDTD.Entry(topic).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();
            var success = true;
            return Json(success);
        }        
    }
}