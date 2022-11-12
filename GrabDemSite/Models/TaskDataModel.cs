using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrabDemSite.Models
{
    public class TaskDataModel
    {
        [Key]
        public string Id { get; set; }
        public int Count { get; set; }
        public UserDataModel User { get; set; }
        public int LevelOfTask { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateStarted { get; set; }

    }
}
