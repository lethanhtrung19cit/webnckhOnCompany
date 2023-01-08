using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAnQLNCKH.Models
{
    public class TopicOfLectureModel
    {
        DHTDTTDNEntities1 dHTDTTDNEntities = new DHTDTTDNEntities1();
        //string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        private SqlConnection con;
        public virtual string IdTy { get; set; }
        public void connection()
        {
            string constr = @"Data Source=DESKTOP-ECMGDNK\SQLEXPRESS;initial catalog=nckh_dhdn;integrated security=True";
            con = new SqlConnection(constr);

        }
        public bool checkemail(string UserName)
        {
            var email = dHTDTTDNEntities.Information.Where(x => x.Email == UserName).Select(x => x.Email).FirstOrDefault();
            var name = dHTDTTDNEntities.Information.Where(x => x.Email == UserName).Select(x => x.NameLe).FirstOrDefault();
            if (email==null || name==null)
            {
                return false;
            }
            return true;
        }    
        public string IdTp()
        {
            string IdTp = null;
            List<string> ID =  qLNCKHDHTDTD.TopicOfLectures.Select(x => x.IdTp.Substring(5, x.IdTp.Length - 5)).ToList();
            if (ID.Count==0)
            {
                IdTp = "DTGV01";
            }
            else
            {
                List<int> IDINT = ID.OrderByDescending(x => int.Parse(x)).Select(x => int.Parse(x)).ToList();
                 IdTp = "DTGV0" + (IDINT[0] + 1);
            }
            
            
            return IdTp;
        }
        public string IdLe(string username)
        {
            connection();
            con.Open();
            string sql = string.Format("select IdLe from Information where Email='" + username + "'"); ;
            SqlCommand a = new SqlCommand(sql, con);
            string a1 = (string)a.ExecuteScalar();
            con.Close();
            return a1;
        }
        //public SelectList getType1()
        //{
        //    IEnumerable<SelectListItem> typeList = (from m in qLNCKHDHTDTD.PointTables select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Grade, Value = m.IdP.ToString() });
        //    return new SelectList(typeList, "Value", "Text", IdTy);

        //}
        //public SelectList getDetailType()
        //{
        //    IEnumerable<SelectListItem> detailtypeList = new List<SelectListItem>();
        //    if (!string.IsNullOrEmpty(IdTy))
        //    {

        //        detailtypeList = (from m in qLNCKHDHTDTD.PointTables where m.IdTy == IdTy select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.NameP, Value = m.IdP.ToString() });
        //    }
        //    return new SelectList(detailtypeList, "Value", "Text", IdTy);

        //}
        public DateTime dateLec(string IdTp)
        {
            return dHTDTTDNEntities.ProgressLes.Where(x=>x.IdTp==IdTp).Max(x => x.Date);
        }
        public DateTime dateLecEx(string IdTp)
        {
            return dHTDTTDNEntities.Extensions.Where(x=>x.IdTp==IdTp).Max(x => x.Times);
        }
        //public DateTime dateStu(string IdTp)
        //{
        //    return dHTDTTDNEntities.ProgressSts.Where(x=>x.IdTp==IdTp).Max(x => x.Date);
        //}
        public void AddTopicLecture(TopicOfLecture topicOfLecture)
        {
            qLNCKHDHTDTD.TopicOfLectures.Add(topicOfLecture);
            qLNCKHDHTDTD.SaveChanges();
        }
        public List<Author> listAu(string IdTp)
        {
            return qLNCKHDHTDTD.Authors.Where(x => x.IdTp == IdTp).ToList();
        }
        public List<TopicOfLectureView> listAuthors(string IdTp)
        {
            return (from a in qLNCKHDHTDTD.Authors 
                    join p in qLNCKHDHTDTD.PointTables on a.IdAu equals p.IdAu
                    where p.IdTp==IdTp
                    select new TopicOfLectureView
                    {
                       author=a,
                       pointTable=p
                    }).ToList();
        }
         
        public List<TopicOfLectureView> listInfo(string IdTp)
        {
            
            return (from p in qLNCKHDHTDTD.PointTables
                    join a in qLNCKHDHTDTD.Authors on p.IdTp equals a.IdTp
                    join i in qLNCKHDHTDTD.Information on a.Email equals i.Email 
                    where p.IdTp == IdTp
                    select new TopicOfLectureView
                    {
                        pointTable = p,
                        author = a,
                        information = i  
                    }
                    ).ToList();
        }
        public List<TopicOfLecture> listTopicLecture(int IdType)
        {
             return qLNCKHDHTDTD.TopicOfLectures.Where(x=>x.IdType==IdType && x.Status==3).ToList();
        }
        public SelectList listPage(int IdDetail)
        {
            return new SelectList(qLNCKHDHTDTD.ChildDetails.Where(x => x.IdDetail == IdDetail).ToList(), "IdChild", "NameChild");
        }
         
        public void AddAuthor(Author author)
        {
            qLNCKHDHTDTD.Authors.Add(author);
            qLNCKHDHTDTD.SaveChanges();
        }
        public bool AddRegisterExtension(string IdTp, DateTime Times, string Reason)
        {

            connection();
            SqlCommand com = new SqlCommand("AddRegisterExtension", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@IdTp", IdTp);
            com.Parameters.AddWithValue("@Times", Times.Date);
            com.Parameters.AddWithValue("@Reason", Reason);
            com.Parameters.AddWithValue("@Status", "chưa duyệt");
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {

                return true;

            }
            else
            {

                return false;
            }
        }
        private DHTDTTDNEntities1 qLNCKHDHTDTD = null;
        public TopicOfLectureModel()
        {
            qLNCKHDHTDTD = new DHTDTTDNEntities1();
        }
       
        public List<TopicOfLecture> listchuaduyet()
        {
            var list = qLNCKHDHTDTD.Database.SqlQuery<TopicOfLecture>("select * from TopicOfLecture where Status like N'chưa duyệt'").ToList();
            return list;
        }
       
        public List<TopicOfStudent> listchuaduyetsv()
        {
            var list = qLNCKHDHTDTD.Database.SqlQuery<TopicOfStudent>("select * from TopicOfStudent where Status like N'chưa duyệt'").ToList();
            return list;
        }
        public List<TopicOfStudent> listStudentAll()
        {
            var list = qLNCKHDHTDTD.Database.SqlQuery<TopicOfStudent>("select * from TopicOfStudent where Status like N'đã duyệt'").ToList();
            return list;
        }
        public List<TopicOfLecture> listAll()
        {
            var list = qLNCKHDHTDTD.Database.SqlQuery<TopicOfLecture>("select * from TopicOfLecture where Status like N'đã duyệt'").ToList();
            return list;
        }
        //public DeTaiGV GetByMaDT(string maDT)
        //{
        //    qLNCKHDHTDTD.DeTaiGVs.SingleOrDefault.
        //}
        public TopicOfLecture ViewDetail(string IdTp)
        {
            return qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
        }
        public bool xoa(string IdTp)
        {
            try
            { 
                var topic = qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
                qLNCKHDHTDTD.TopicOfLectures.Remove(topic);

                qLNCKHDHTDTD.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
           
          
        }
        public bool xetduyet1(string IdTp)
        {
            try
            {


                //detai.TrangThai = dtgv1.TrangThai;                 
                qLNCKHDHTDTD.Database.ExecuteSqlCommand("update TopicOfLecture set Status like N'đã duyệt' where IdTp=@IdTp",
                     new SqlParameter("@IdTp", IdTp)
                    );

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
       
        //public int xetduyet(int ID)
        //{
        //    int i;
        //    using (SqlConnection con = new SqlConnection(cs))
        //    {
        //        con.Open();
        //        SqlCommand com = new SqlCommand("DeleteEmployee", con);
        //        com.CommandType = CommandType.StoredProcedure;
        //        com.Parameters.AddWithValue("@Id", ID);
        //        i = com.ExecuteNonQuery();
        //    }
        //    return i;
        //}
    }
}