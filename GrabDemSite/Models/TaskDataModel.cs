using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrabDemSite.Models
{
    public class TaskDataModel
    {
        public TaskDataModel() { }
        public TaskDataModel(string id,int count, UserDataModel user, int levelOfTask,bool newAccount) 
        {
            Id = id;
            Count = count;
            User = user;
            LevelOfTask = levelOfTask;
            NewAccount = newAccount;
        }
        [Key]
        public string Id { get; set; }
        public int Count { get; set; }
        public UserDataModel User { get; set; }
        public int LevelOfTask { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateStarted { get; set; }
        public bool NewAccount { get; set; }
    }
}
