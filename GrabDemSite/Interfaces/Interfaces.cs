using GrabDemSite.Models;

namespace GrabDemSite.Interfaces
{
    public interface ITransactionDataModel
    {
        string Id { get; set; }
        UserDataModel User { get; set; }
        bool IsConfirmed { get; set; }
        DateTime DateCreated { get; set; }
        double Money { get; set; }
    }
}
