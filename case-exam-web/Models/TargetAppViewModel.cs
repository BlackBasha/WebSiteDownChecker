using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace case_exam_web.Models
{
    public class TargetAppViewModel
    {

        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Application Name")]
        public string AppName { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        [Display(Name = "Check Interval")]
        public int CheckInterval { get; set; }
    }
}
