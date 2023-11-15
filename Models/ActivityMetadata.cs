using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ActivityProjectManage.Models
{
    public class ActivityMetadata
    {
    }

    [MetadataType(typeof(ActivityMetadata))]
    public partial class Activity
    {
        public bool isdeleteActivity { get; set; }
        public bool isaddActivity { get; set; }

    }

}