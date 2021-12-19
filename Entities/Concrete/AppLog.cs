
using Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
   public class AppLog:IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public int ResponseNumber { get; set; }
        public string LogDetails { get; set; }
        public int TargetAppId { get; set; }
    }
}
