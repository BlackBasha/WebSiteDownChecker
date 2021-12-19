using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
  public  class TargetApp:IEntity
    {
        [Key]
        public int Id { get; set; }
        public string AppName { get; set; }

        public string Url { get; set; }

        public int CheckInterval { get; set; }
    }
}
