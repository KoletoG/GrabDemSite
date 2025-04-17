namespace GrabDemSite.Models
{
    public class IndexViewModel
    {
        public TaskDataModel TaskDataModel { get; private set; }
        public UserDataModel UserDataModel { get; private set; }
        public string BlockChain { get; private set; }
        public int UserCount { get; private set; }
        public float BitcoinSupply { get; private set; }
        public IndexViewModel(TaskDataModel task, UserDataModel user,string blockchain,int count, float btcSupply) 
        { 
            TaskDataModel = task;
            UserDataModel = user;
            BlockChain = blockchain;
            UserCount = count;
            BitcoinSupply = btcSupply;
        }
    }
}
