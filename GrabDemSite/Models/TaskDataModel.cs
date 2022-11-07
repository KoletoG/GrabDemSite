using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace GrabDemSite.Models
{
    public class TaskDataModel
    {
        [MinLength(3)]
        [MaxLength(30)]
        public string? Title { get; set; }
    }
}
