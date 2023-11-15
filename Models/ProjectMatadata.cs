using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActivityProjectManage.Models
{
    public class ProjectMatadata
    {
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> StartProjectDate { get; set; }


        [DataType(DataType.Date)]
        public Nullable<System.DateTime> EndProjectDate { get; set; }
    }
    [MetadataType(typeof(ProjectMatadata))]
    public partial class Project
    {
        public string StartAndEndDate
        {
            get 
                {
                var startdt = string.Format("{0:dd/MM/yyyy}", StartProjectDate);
                var enddt = string.Format("{0:dd/MM/yyyy}", EndProjectDate);
                var StartAndEndDate = startdt + " - " + enddt;
                return StartAndEndDate;
            }
        }
    }
}